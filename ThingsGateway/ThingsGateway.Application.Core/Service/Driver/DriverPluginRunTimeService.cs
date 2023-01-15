namespace ThingsGateway.Application.Core;


/// <summary>
/// 驱动插件API服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.RunTimeGroupName, Order = 200)]
public class DriverPluginRunTimeService : IDynamicApiController, IScoped
{
    private readonly SqlSugarRepository<DriverPlugin> _driverpluginRep;
    private readonly SysCacheService _sysCacheService;
    private readonly PluginService _pluginService;

    public DriverPluginRunTimeService(SqlSugarRepository<DriverPlugin> deviceRep,
        SysCacheService sysCacheService, PluginService pluginService)
    {
        _driverpluginRep = deviceRep;
        _sysCacheService = sysCacheService;
        _pluginService = pluginService;
    }

    /// <summary>
    /// 获取驱动插件分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<PluginInfo>> GetDriverPluginPage([FromQuery] PageDriverPluginInput input)
    {
        var data = await _pluginService.DriverInfos
            .WhereIF(!string.IsNullOrWhiteSpace(input.FileName?.Trim()), u => u.FileName.Contains(input.FileName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PluginName?.Trim()), u => u.PluginName.Contains(input.PluginName))
            .OrderBy(u => u.PluginName).ToPagedListAsync(input.Page, input.PageSize);

        return data;
    }



}