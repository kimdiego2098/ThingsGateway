namespace ThingsGateway.Application.Core;
/// <summary>
/// 采集设备运行态API服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.RunTimeGroupName, Order = 300)]
public class DeviceRunTimeService : IDynamicApiController, IScoped
{
    private readonly DeviceCollectService _deviceCollectService;

    public DeviceRunTimeService(
        SqlSugarRepository<Device> deviceRep,
        IServiceProvider serviceProvider
        )
    {
        _deviceCollectService = serviceProvider.GetBackgroundService<DeviceCollectService>();
    }

    /// <summary>
    /// 获取设备状态分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<Device>> GetDevicePage([FromQuery] PageDeviceInput input)
    {
        //    //先获取数据库中存在的设备
        //    var data = await _deviceRep.AsQueryable()
        //.WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
        //.WhereIF(!string.IsNullOrWhiteSpace(input.PluginName?.Trim()), u => u.DriverAssembleName.Contains(input.PluginName))
        //.OrderBy(u => u.CreateTime).ToPagedListAsync(input.Page, input.PageSize);

        var runTimeData = _deviceCollectService.DeviceCollectCores
            //.Where(it => data.Items.Any(a => a.Id == it.DeviceId))
            .Select(it => it.DeviceCopy)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PluginName?.Trim()), u => u.DriverAssembleName.Contains(input.PluginName))
            .OrderBy(u => u.CreateTime);
        var data = await runTimeData.ToPagedListAsync(input.Page, input.PageSize);
        return data;
    }


}