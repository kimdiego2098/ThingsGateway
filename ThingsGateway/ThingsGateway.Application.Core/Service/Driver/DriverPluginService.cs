namespace ThingsGateway.Application.Core;


/// <summary>
/// 驱动插件API服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 200)]
public class DriverPluginService : IDynamicApiController, IScoped
{
    private readonly SqlSugarRepository<DriverPlugin> _driverpluginRep;
    private readonly SysCacheService _sysCacheService;
    private readonly PluginService _pluginService;
    public DriverPluginService(SqlSugarRepository<DriverPlugin> deviceRep,
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
    public async Task<SqlSugarPagedList<DriverPlugin>> GetDriverPluginPage([FromQuery] PageDriverPluginInput input)
    {
        var data = await _driverpluginRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.FileName?.Trim()), u => u.FileName.Contains(input.FileName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PluginName?.Trim()), u => u.PluginName.Contains(input.PluginName))
            .OrderBy(u => u.PluginName).ToPagedListAsync(input.Page, input.PageSize);

        //不包含设备变量
        return data;
    }


    /// <summary>
    /// 获取驱动插件全部驱动名称列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<dynamic> GetDriverPluginInfos()
    {
        var data = await Task.Run(() =>
        {
            return (dynamic)_pluginService.DriverInfos.SelectMany(it =>
            new[]
            {
                new{Name=it.PluginName,Children=it.PluginAssemble.Select(it=>new{Name=it.AssembleName }) },
                }
            );
        });
        return data;
    }

    /// <summary>
    /// 增加驱动插件配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AddDriverPlugin(AddDriverPluginInput input)
    {
        var isExist = await _driverpluginRep.IsAnyAsync(u => u.FileName == input.FileName);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D9000);
        _pluginService.AddDriver(input.Adapt<DriverPlugin>());
        await _driverpluginRep.Context
            .Insertable(input.Adapt<DriverPlugin>())
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除驱动配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteConfig(DeleteDriverPluginInput input)
    {
        var driverplugin = await _driverpluginRep.GetFirstAsync(u => u.Id == input.Id);
        var config = await _driverpluginRep.Context.Deleteable<DriverPlugin>(
it => it.Id == driverplugin.Id)
.ExecuteCommandAsync();
        _pluginService.DeleteDriver(driverplugin);
        _sysCacheService.Remove(driverplugin.FileName);

    }

}