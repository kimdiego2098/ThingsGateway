using Furion.DataEncryption;

namespace ThingsGateway.Application.Core;


/// <summary>
/// 历史数据服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 200)]
public class HisService : IDynamicApiController, IScoped
{
    private readonly SqlSugarRepository<HisConfig> _hisConfigRep;
    private readonly SysCacheService _sysCacheService;
    private readonly DeviceCollectService _deviceCollectService;
    private readonly HisHostService _hisService;
    public HisService(SqlSugarRepository<HisConfig> hisConfigRep,
        SysCacheService sysCacheService,
        IServiceProvider serviceProvider
        )
    {
        _hisConfigRep = hisConfigRep;
        _sysCacheService = sysCacheService;
        _deviceCollectService = serviceProvider.GetBackgroundService<DeviceCollectService>();
        _hisService = serviceProvider.GetBackgroundService<HisHostService>();
    }

    /// <summary>
    /// 获取历史数据库配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<HisConfig> GetHisConfig()
    {
        var data = await _hisConfigRep.AsQueryable().FirstAsync();
        data.ConnStr = DESCEncryption.Decrypt(data.ConnStr, ApplicationInfo.DESCKey);

        //不包含设备变量
        return data;
    }


    /// <summary>
    /// 保存历史数据库配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateHisConfig(HisConfig input)
    {
        input.ConnStr = DESCEncryption.Encrypt(input.ConnStr, ApplicationInfo.DESCKey);
        await _hisConfigRep.Context
            .Updateable(input)
            .ExecuteCommandAsync();
        Interlocked.CompareExchange(ref _hisService.IsHisConfigChange, 1, 0);

    }






}