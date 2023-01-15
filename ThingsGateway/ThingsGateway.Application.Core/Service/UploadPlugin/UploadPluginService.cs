namespace ThingsGateway.Application.Core;
/// <summary>
/// 上传插件API服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 200)]
public class UploadPluginService : IDynamicApiController, IScoped
{
    private readonly SqlSugarRepository<UploadPlugin> _uploadpluginRep;
    private readonly SysCacheService _sysCacheService;
    private readonly PluginService _pluginService;
    public UploadPluginService(SqlSugarRepository<UploadPlugin> deviceRep,
        SysCacheService sysCacheService, PluginService pluginService)
    {
        _uploadpluginRep = deviceRep;
        _sysCacheService = sysCacheService;
        _pluginService = pluginService;
    }

    /// <summary>
    /// 获取上传插件分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<UploadPlugin>> GetUploadPluginPage([FromQuery] PageUploadPluginInput input)
    {
        var data = await _uploadpluginRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.FileName?.Trim()), u => u.FileName.Contains(input.FileName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PluginName?.Trim()), u => u.PluginName.Contains(input.PluginName))
            .OrderBy(u => u.PluginName).ToPagedListAsync(input.Page, input.PageSize);

        //不包含设备变量
        return data;
    }


    /// <summary>
    /// 获取上传插件全部上传名称列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<dynamic> GetUploadPluginInfos()
    {
        var data = await Task.Run(() =>
        {
            return (dynamic)_pluginService.UploadInfos.SelectMany(it =>
            new[]
            {
                new{Name=it.PluginName,Children=it.PluginAssemble.Select(it=>new{Name=it.AssembleName }) },
                }
            );
        });
        return data;
    }

    /// <summary>
    /// 增加上传插件配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AddUploadPlugin(AddUploadPluginInput input)
    {
        var isExist = await _uploadpluginRep.IsAnyAsync(u => u.FileName == input.FileName);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D9000);
        _pluginService.AddUpload(input.Adapt<UploadPlugin>());
        await _uploadpluginRep.Context
            .Insertable(input.Adapt<UploadPlugin>())
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除上传配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteConfig(DeleteUploadPluginInput input)
    {
        var uploadplugin = await _uploadpluginRep.GetFirstAsync(u => u.Id == input.Id);
        var config = await _uploadpluginRep.Context.Deleteable<UploadPlugin>(
it => it.Id == uploadplugin.Id)
.ExecuteCommandAsync();
        _pluginService.DeleteUpload(uploadplugin);
        _sysCacheService.Remove(uploadplugin.FileName);

    }

}