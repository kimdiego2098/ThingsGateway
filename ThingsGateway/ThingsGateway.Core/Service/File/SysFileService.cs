using OnceMi.AspNetCore.OSS;

namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统文件服务
/// </summary>
[ApiDescriptionSettings(Order = 194)]
public class SysFileService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysFile> _sysFileRep;
    private readonly OSSProviderOptions _OSSProviderOptions;
    private readonly UploadOptions _uploadOptions;
    private readonly ICommonService _commonService;
    private readonly IOSSService _OSSService;

    public SysFileService(UserManager userManager,
        SqlSugarRepository<SysFile> sysFileRep,
        IOptions<OSSProviderOptions> oSSProviderOptions,
        IOptions<UploadOptions> uploadOptions,
        ICommonService commonService,
        IOSSServiceFactory ossServiceFactory)
    {
        _userManager = userManager;
        _sysFileRep = sysFileRep;
        _OSSProviderOptions = oSSProviderOptions.Value;
        _uploadOptions = uploadOptions.Value;
        _commonService = commonService;
        if (_OSSProviderOptions.IsEnable)
            _OSSService = ossServiceFactory.Create(Enum.GetName(_OSSProviderOptions.Provider));
    }

    /// <summary>
    /// 获取文件分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<SysFile>> GetFilePage([FromQuery] PageFileInput input)
    {
        return await _sysFileRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.FileName), u => u.FileName.Contains(input.FileName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                        u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<FileOutput> UploadFile([Required] IFormFile file, string path = "")
    {
        var sysFile = await HandleUploadFile(file, path);
        return new FileOutput
        {
            Id = sysFile.Id,
            Url = sysFile.Url,  // string.IsNullOrWhiteSpace(sysFile.Url) ? _commonService.GetFileUrl(sysFile) : sysFile.Url,
            SizeKb = sysFile.SizeKb,
            Suffix = sysFile.Suffix,
            FilePath = sysFile.FilePath,
        };
    }

    /// <summary>
    /// 上传多文件
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<List<FileOutput>> UploadFiles([Required] List<IFormFile> files)
    {
        var filelist = new List<FileOutput>();
        foreach (var file in files)
        {
            filelist.Add(await UploadFile(file));
        }
        return filelist;
    }

    /// <summary>
    /// 下载文件(文件流)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DownloadFile(FileInput input)
    {
        var file = await GetFile(input);
        var fileName = HttpUtility.UrlEncode(file.FileName, Encoding.GetEncoding("UTF-8"));
        if (_OSSProviderOptions.IsEnable)
        {
            var filePath = string.Concat(file.FilePath, "/", input.Id.ToString() + file.Suffix);
            var stream = await (await _OSSService.PresignedGetObjectAsync(file.BucketName.ToString(), filePath, 5)).GetAsStreamAsync();
            return new FileStreamResult(stream.Stream, "application/octet-stream") { FileDownloadName = fileName + file.Suffix };
        }
        else
        {
            var filePath = Path.Combine(file.FilePath, input.Id.ToString() + file.Suffix);
            var path = Path.Combine(App.WebHostEnvironment.WebRootPath, filePath);
            return new FileStreamResult(new FileStream(path, FileMode.Open), "application/octet-stream") { FileDownloadName = fileName + file.Suffix };
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteFile(DeleteFileInput input)
    {
        var file = await _sysFileRep.GetFirstAsync(u => u.Id == input.Id);
        if (file != null)
        {
            await _sysFileRep.DeleteAsync(file);

            if (_OSSProviderOptions.IsEnable)
            {
                await _OSSService.RemoveObjectAsync(file.BucketName.ToString(), string.Concat(file.FilePath, "/", input.Id.ToString()));
            }
            else
            {
                var filePath = Path.Combine(App.WebHostEnvironment.WebRootPath, file.FilePath, input.Id.ToString() + file.Suffix);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }
    }

    /// <summary>
    /// 获取文件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task<SysFile> GetFile([FromQuery] FileInput input)
    {
        var file = await _sysFileRep.GetFirstAsync(u => u.Id == input.Id);
        return file ?? throw Oops.Oh(ErrorCodeEnum.D8000);
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="savePath">路径</param>
    /// <returns></returns>
    private async Task<SysFile> HandleUploadFile(IFormFile file, string savePath = "")
    {
        if (file == null) throw Oops.Oh(ErrorCodeEnum.D8000);

        var path = savePath;
        if (string.IsNullOrWhiteSpace(savePath))
        {
            path = _uploadOptions.Path;
            var reg = new Regex(@"(\{.+?})");
            var match = reg.Matches(path);
            match.ToList().ForEach(a =>
            {
                var str = DateTime.Now.ToString(a.ToString().Substring(1, a.Length - 2)); // 每天一个目录
                path = path.Replace(a.ToString(), str);
            });
        }

        if (!_uploadOptions.ContentType.Contains(file.ContentType))
            throw Oops.Oh(ErrorCodeEnum.D8001);

        var sizeKb = (long)(file.Length / 1024.0); // 大小KB
        if (sizeKb > _uploadOptions.MaxSize)
            throw Oops.Oh(ErrorCodeEnum.D8002);

        var suffix = Path.GetExtension(file.FileName).ToLower(); // 后缀

        var newFile = new SysFile
        {
            Id = Yitter.IdGenerator.YitIdHelper.NextId(),
            // BucketName = _OSSProviderOptions.IsEnable ? _OSSProviderOptions.Provider.ToString() : "Local",
            // 阿里云对bucket名称有要求，1.只能包括小写字母，数字，短横线（-）2.必须以小写字母或者数字开头  3.长度必须在3-63字节之间
            // 无法使用Provider
            BucketName = _OSSProviderOptions.IsEnable ? _OSSProviderOptions.Bucket : "Local",
            FileName = Path.GetFileNameWithoutExtension(file.FileName),
            Suffix = suffix,
            SizeKb = sizeKb.ToString(),
            FilePath = path,
        };

        var finalName = newFile.Id + suffix; // 文件最终名称
        if (_OSSProviderOptions.IsEnable)
        {
            newFile.Provider = Enum.GetName(_OSSProviderOptions.Provider);
            var filePath = string.Concat(path, "/", finalName);
            await _OSSService.PutObjectAsync(newFile.BucketName, filePath, file.OpenReadStream());
            //  http://<你的bucket名字>.oss.aliyuncs.com/<你的object名字>
            //  生成外链地址 方便前端预览
            switch (_OSSProviderOptions.Provider)
            {
                case OSSProvider.Aliyun:
                    newFile.Url = $"{(_OSSProviderOptions.IsEnableHttps ? "https" : "http")}://{newFile.BucketName}.{_OSSProviderOptions.Endpoint}/{filePath}";
                    break;

                case OSSProvider.Minio:
                    // 获取Minio文件的下载或者预览地址
                    newFile.Url = await GetMinioPreviewFileUrl(newFile.BucketName, filePath); ;
                    break;
            }
        }
        else
        {
            newFile.Provider = ""; // 本地存储 Provider 显示为空
            var filePath = Path.Combine(App.WebHostEnvironment.WebRootPath, path);
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            var realFile = Path.Combine(filePath, finalName);
            //IDetector detector;
            using (var stream = File.Create(realFile))
            {
                await file.CopyToAsync(stream);
                //detector = stream.DetectFiletype();
            }
            //var realExt = detector.Extension; // 真实扩展名
            //// 二次校验扩展名
            //if (!string.Equals(realExt, suffix.Replace(".", ""), StringComparison.OrdinalIgnoreCase))
            //{
            //    var delFilePath = Path.Combine(App.WebHostEnvironment.WebRootPath, realFile);
            //    if (File.Exists(delFilePath))
            //        File.Delete(delFilePath);
            //    throw Oops.Oh(ErrorCodeEnum.D8001);
            //}

            // 生成外链
            newFile.Url = _commonService.GetFileUrl(newFile);
        }
        await _sysFileRep.AsInsertable(newFile).ExecuteCommandAsync();
        return newFile;
    }

    /// <summary>
    /// 获取Minio文件的下载或者预览地址
    /// </summary>
    /// <param name="bucketName">桶名</param>
    /// <param name="fileName">文件名</param>
    /// <returns></returns>
    private async Task<string> GetMinioPreviewFileUrl(string bucketName, string fileName)
    {
        return await _OSSService.PresignedGetObjectAsync(bucketName, fileName, 7);
    }

    /// <summary>
    /// 上传头像
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<FileOutput> UploadAvatar([Required] IFormFile file)
    {
        var sysUserRep = _sysFileRep.ChangeRepository<SqlSugarRepository<SysUser>>();
        var user = sysUserRep.GetFirst(u => u.Id == _userManager.UserId);
        // 删除当前用户已有头像
        if (!string.IsNullOrWhiteSpace(user.Avatar) && user.Avatar.EndsWith(".png"))
        {
            var fileId = Path.GetFileNameWithoutExtension(user.Avatar);
            await DeleteFile(new DeleteFileInput { Id = long.Parse(fileId) });
        }

        var res = await UploadFile(file, "Avatar");
        await sysUserRep.UpdateAsync(u => new SysUser() { Avatar = res.Url }, u => u.Id == user.Id);
        return res;
    }

    /// <summary>
    /// 上传电子签名
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<FileOutput> UploadSignature([Required] IFormFile file)
    {
        var sysUserRep = _sysFileRep.ChangeRepository<SqlSugarRepository<SysUser>>();
        var user = sysUserRep.GetFirst(u => u.Id == _userManager.UserId);
        // 删除当前用户已有电子签名
        if (!string.IsNullOrWhiteSpace(user.Signature) && user.Signature.EndsWith(".png"))
        {
            var fileId = Path.GetFileNameWithoutExtension(user.Signature);
            await DeleteFile(new DeleteFileInput { Id = long.Parse(fileId) });
        }

        var res = await UploadFile(file, "Signature");
        await sysUserRep.UpdateAsync(u => new SysUser() { Signature = res.Url }, u => u.Id == user.Id);
        return res;
    }
}