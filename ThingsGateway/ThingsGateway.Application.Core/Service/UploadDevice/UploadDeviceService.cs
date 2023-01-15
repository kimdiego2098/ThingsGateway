using Magicodes.ExporterAndImporter.Excel;


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Application.Core;
/// <summary>
/// 采集设备API服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 200)]
[AllowAnonymous]
public class UploadDeviceService : IDynamicApiController, IScoped
{
    private readonly UploadService _uploadService;
    private readonly SqlSugarRepository<UploadDevice> _deviceRep;
    private readonly SqlSugarRepository<UploadDeviceProperty> _devicePropertyRep;
    private readonly SysCacheService _sysCacheService;
    private readonly UploadOptions _uploadOptions;
    public UploadDeviceService(SqlSugarRepository<UploadDevice> deviceRep,
        SqlSugarRepository<UploadDeviceProperty> devicePropertyRep,
       IOptions<UploadOptions> uploadOptions,
       IServiceProvider serviceProvider,
        SysCacheService sysCacheService)
    {
        _uploadOptions = uploadOptions.Value;

        _deviceRep = deviceRep;
        _sysCacheService = sysCacheService; _devicePropertyRep = devicePropertyRep;
        _uploadService = serviceProvider.GetBackgroundService<UploadService>();

    }

    /// <summary>
    /// 获取设备配置分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<UploadDevice>> GetDevicePage([FromQuery] PageUploadDeviceInput input)
    {
        var data = await _deviceRep.AsQueryable()
           .Includes(a => a.DeviceProperties, t => t.UploadDevice)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PluginName?.Trim()), u => u.DriverAssembleName.Contains(input.PluginName))
            .OrderBy(u => u.CreateTime).ToPagedListAsync(input.Page, input.PageSize);

        return data;
    }

    /// <summary>
    /// 增加设备配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AddDevice(AddUploadDeviceInput input)
    {
        var isExist = await _deviceRep.IsAnyAsync(u => u.Name == input.Name);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D9000);


        var result = await _deviceRep.Context
.InsertNav(input.Adapt<UploadDevice>())//这里新增设备时不包含设备属性，因为新增时未传递到worker,无法获取对应信息
.Include(z1 => z1.DeviceProperties)
//.Insertable(input.Adapt<UploadDevice>())
.ExecuteCommandAsync();


        if (!result) throw Oops.Oh(ErrorCodeEnum.Z5000);
        //var dev = await _deviceRep.GetFirstAsync(u => u.Name == input.Name);

        //这里执行动态添加设备
        //_uploadService.CreateDeviceThread(dev, true);

    }
    /// <summary>
    /// 刷新设备属性
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<List<UploadDeviceProperty>> RefreshDeviceProperty(UpdateUploadDeviceInput input)
    {

        var dev = input.Adapt<UploadDevice>();
        if (dev.DriverAssembleName.IsNullOrWhiteSpace())
            throw Oops.Oh(ErrorCodeEnum.Z5000);


        //手动刷新设备属性，这里先获取新的设备属性
        var propertys = await _uploadService.GetUploadDevicePropertysAsync(dev.DriverAssembleName);

        return propertys;

    }



    /// <summary>
    /// 更新设备配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateConfig(UpdateUploadDeviceInput input)
    {
        var isExist = await _deviceRep.IsAnyAsync(u => u.Name == input.Name && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D9000);

        var dev = input.Adapt<UploadDevice>();
        var result = await _deviceRep.Context
       .UpdateNav(dev)
       .Include(z1 => z1.DeviceProperties)
       .ExecuteCommandAsync();
        if (result)
        {
            //_uploadService.UpDeviceThread(input.Adapt<UploadDevice>(), true);
            _uploadService.SetPluginProperties(dev.Id, dev.DeviceProperties);

        }
        else
        {
            throw Oops.Oh(ErrorCodeEnum.Z5000);
        }
    }


    /// <summary>
    /// 删除设备配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteConfig(DeleteUploadDeviceInput input)
    {
        var device = await _deviceRep.GetFirstAsync(u => u.Id == input.Id);
        var config = await _deviceRep.Context.DeleteNav<UploadDevice>(
it => it.Id == device.Id)
.Include(z1 => z1.DeviceProperties)

.ExecuteCommandAsync();
        if (config)
        {
            _sysCacheService.Remove(device.Name);
            _uploadService.RemoveDeviceThread(input.Adapt<UploadDevice>());
        }
        else
        {
            throw Oops.Oh(ErrorCodeEnum.Z5000);

        }



    }


    /// <summary>
    /// 获取设备名称列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<DevIdName>> GetDeviceNames()
    {
        var sql = _deviceRep.AsQueryable()
           ;
        var sqlString = sql.ToSqlString();

        var data = await sql.ToListAsync();
        var result = data.Select(it => new DevIdName { Id = it.Id, Name = it.Name });
        return result.ToList();
    }


    /// <summary>
    /// 导出设备表
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ExportFile(UploadDeviceExportInput input)
    {
        var sql = _deviceRep.AsQueryable()
            .Includes(a => a.DeviceProperties, t => t.UploadDevice)
                        .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PluginName?.Trim()), u => u.DriverAssembleName.Contains(input.PluginName))
           ;
        var sqlString = sql.ToSqlString();
        var list = await sql.ToListAsync();
        var devices = list.Adapt<List<UploadDeviceExcel>>();
        var devicePropertys = list.SelectMany(it => it.DeviceProperties).Adapt<List<UploadDevicePropertyExcel>>();
        var exporter = new ExcelExporter();
        var result = await exporter.Append(devices)
        .SeparateBySheet()
            .Append(devicePropertys).ExportAppendDataAsByteArray();
        var fs = new MemoryStream(result);
        return new FileStreamResult(fs, "application/octet-stream") { FileDownloadName = DateTime.Now.ToString("yyyyMMddHHmmss") + "上传设备.xlsx" };
    }

    /// <summary>
    /// 导入设备(文件流)
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task ImprotFile([Required] IFormFile file)
    {
        if (file == null) throw Oops.Oh(ErrorCodeEnum.D8000);

        if (!_uploadOptions.ContentType.Contains(file.ContentType))
            throw Oops.Oh(ErrorCodeEnum.D8001);

        var sizeKb = (long)(file.Length / 1024.0); // 大小KB
        if (sizeKb > _uploadOptions.MaxSize)
            throw Oops.Oh(ErrorCodeEnum.D8002);

        //var suffix = Path.GetExtension(file.FileName).ToLower(); // 后缀
        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        ExcelImporter Importer = new ExcelImporter();
        //获取到的导入结果为一个字典类型，Key为Sheet名，Value为Sheet对应的数据
        var importDic = await Importer.ImportMultipleSheet<ImportUploadDeviceExcelAndUploadDevicePropertyExcel>(stream);
        //遍历字典，获取每个Sheet的数据
        List<UploadDevice> devs = new();
        List<UploadDevice> adddevs = new();
        List<UploadDevice> updevs = new();
        List<UploadDevicePropertyExcel> devpros = new();
        foreach (var item in importDic)
        {
            var import = item.Value;
            //导入的Sheet数据，
            if (item.Key == "上传设备表")
            {
                //多个不同类型的Sheet返回的值为object，需要进行类型转换
                devs = import.Data.Adapt<List<UploadDevice>>();

            }
            if (item.Key == "上传设备附加属性表")
            {
                devpros = import.Data.Adapt<List<UploadDevicePropertyExcel>>();
            }
        }

        //设备属性分组
        IEnumerable<IGrouping<string, UploadDevicePropertyExcel>> devpropertys = devpros.GroupBy(it => it.UploadDeviceName);

        var olddatas = await _deviceRep.AsQueryable()
            .Where(it => devs.Select(a => a.Name).Contains(it.Name))
           .ToListAsync();

        devs.ForEach(its =>
        {
            var olddev = olddatas.Where(it => it.Name == its.Name).FirstOrDefault();
            if (olddev == null)
            {
                adddevs.Add(its);
            }
            else
            {
                its.Id = olddev.Id;
                updevs.Add(its);
            }
            devpropertys.ForEach(it =>
            {
                if (its.Name == it.Key)
                {
                    var data = it.Adapt<List<UploadDeviceProperty>>();
                    //data.ForEach(it => it.DeviceId = its.Id);
                    its.DeviceProperties = data;
                }
            });
        });


        var re1 = await _deviceRep.Context
                       .UpdateNav(updevs)
           .Include(z1 => z1.DeviceProperties)
            .ExecuteCommandAsync();

        var re2 = await _deviceRep.Context
           .InsertNav(adddevs)
        .Include(z1 => z1.DeviceProperties)
        .ExecuteCommandAsync();
        foreach (var item in devs)
        {
            //await RestartDevice(new() { Id = item.Id });
        }
    }


    /// <summary>
    /// 启停设备采集，ID小于等于0时启停全部设备
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public void ConfigDeviceStatus(UploadDeviceConfigInput input)
    {
        _uploadService.ConfigDeviceThread(input.Id, input.InvokeEnable);
        _deviceRep.AsUpdateable().SetColumns(it => it.InvokeEnable == input.InvokeEnable)
              .Where(it => it.Id == input.Id || input.Id <= 0)
            .ExecuteCommand();
    }


    /// <summary>
    /// 完全重启设备，包含线程重新创建
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task RestartDevice(DeleteUploadDeviceInput input)
    {
        var devices = await _deviceRep.AsQueryable()
            .WhereIF(input.Id != 0, u => u.Id == input.Id)
           .Includes(a => a.DeviceProperties, t => t.UploadDevice)
           .ToListAsync();
        foreach (var device in devices)
        {
            _uploadService.RemoveDeviceThread(device);
            _uploadService.CreateDeviceThread(device, true);
        }
    }
}
