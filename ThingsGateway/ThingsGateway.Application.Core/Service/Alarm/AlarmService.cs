using Furion.DataEncryption;

namespace ThingsGateway.Application.Core;


/// <summary>
/// 报警服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 200)]
public class AlarmService : IDynamicApiController, IScoped
{
    private readonly SqlSugarRepository<AlarmConfig> _alarmConfigRep;
    private readonly SysCacheService _sysCacheService;
    private readonly DeviceCollectService _deviceCollectService;
    private readonly AlarmHostService _alarmService;
    public AlarmService(SqlSugarRepository<AlarmConfig> alarmConfigRep,
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
    /// 获取报警数据库配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<AlarmConfig> GetAlarmConfig()
    {
        var data = await _alarmConfigRep.AsQueryable().FirstAsync();
        data.ConnStr = DESCEncryption.Decrypt(data.ConnStr, ApplicationInfo.DESCKey);

        //不包含设备变量
        return data;
    }


    /// <summary>
    /// 保存报警数据库配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateAlarmConfig(AlarmConfig input)
    {
        input.ConnStr = DESCEncryption.Encrypt(input.ConnStr, ApplicationInfo.DESCKey);
        await _alarmConfigRep.Context
            .Updateable(input)
            .ExecuteCommandAsync();
        Interlocked.CompareExchange(ref _alarmService.IsAlarmConfigChange, 1, 0);

    }





}