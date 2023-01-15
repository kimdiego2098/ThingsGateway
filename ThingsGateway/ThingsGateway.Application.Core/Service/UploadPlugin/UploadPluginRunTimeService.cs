namespace ThingsGateway.Application.Core;


/// <summary>
/// 上传插件API服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.RunTimeGroupName, Order = 200)]
public class UploadPluginRunTimeService : IDynamicApiController, IScoped
{
    private readonly SqlSugarRepository<UploadPlugin> _uploadpluginRep;
    private readonly SysCacheService _sysCacheService;
    private readonly PluginService _pluginService;

    public UploadPluginRunTimeService(SqlSugarRepository<UploadPlugin> deviceRep,
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
    public async Task<SqlSugarPagedList<PluginInfo>> GetUploadPluginPage([FromQuery] PageUploadPluginInput input)
    {
        var data = await _pluginService.UploadInfos
            .WhereIF(!string.IsNullOrWhiteSpace(input.FileName?.Trim()), u => u.FileName.Contains(input.FileName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PluginName?.Trim()), u => u.PluginName.Contains(input.PluginName))
            .OrderBy(u => u.PluginName).ToPagedListAsync(input.Page, input.PageSize);

        return data;
    }



}