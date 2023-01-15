namespace ThingsGateway.Application.Core;


/// <summary>
/// 报警服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.RunTimeGroupName, Order = 200)]
public class AlarmRunTimeService : IDynamicApiController, IScoped
{
    private readonly SqlSugarRepository<AlarmConfig> _alarmConfigRep;
    private readonly SysCacheService _sysCacheService;
    private readonly DeviceCollectService _deviceCollectService;
    private readonly AlarmHostService _alarmService;
    public AlarmRunTimeService(SqlSugarRepository<AlarmConfig> alarmConfigRep,
        SysCacheService sysCacheService,
        IServiceProvider serviceProvider
        )
    {
        _alarmConfigRep = alarmConfigRep;
        _sysCacheService = sysCacheService;
        _deviceCollectService = serviceProvider.GetBackgroundService<DeviceCollectService>();
        _alarmService = serviceProvider.GetBackgroundService<AlarmHostService>();
    }

    /// <summary>
    /// 获取设备报警变量分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<DeviceVariableCopy>> GetRealAlarmPage([FromQuery] PageDeviceVariableInput input)
    {
        var list = _deviceCollectService.DeviceCollectCores.Select(a => a.DeviceVariablesCopy).ToList();
        var listdata = list.Where(it => it != null && it.Count > 0).SelectMany(a => a).ToList();
        var runTimeData = await listdata
            ?.WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            ?.WhereIF(!string.IsNullOrWhiteSpace(input.DeviceName?.Trim()), u => _deviceCollectService.DevIdNames.FirstOrDefault(it => it.Id == u.DeviceId).Name.Contains(input.DeviceName))
            //.Where(it=> data.Items.Any(a => a.Id == it.Id))
            .Where(it => it.VariableAlarms != null && it.VariableAlarms.EventType != EventEnum.None
            && it.VariableAlarms.EventType != EventEnum.Finish)
            ?.ToPagedListAsync(input.Page, input.PageSize);
        return runTimeData;

    }


    /// <summary>
    /// 获取设备历史报警分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<AlarmHis>> GetHisAlarmPage([FromQuery] PageDeviceVariableInput input)
    {
        if (AlarmHostService._SqlSugarScope == null) throw new("报警服务未初始化");
        var data = await AlarmHostService._SqlSugarScope?.Queryable<AlarmHis>()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.DeviceName?.Trim()), u => u.DeviceName.Contains(input.DeviceName))
            .OrderBy(u => u.VariableAlarmsEventTime, OrderByType.Desc).ToPagedListAsync(input.Page, input.PageSize);

        //不包含设备变量
        return data;

    }




}