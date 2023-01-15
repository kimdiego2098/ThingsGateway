namespace ThingsGateway.Application.Core;


/// <summary>
/// 历史数据服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.RunTimeGroupName, Order = 200)]
public class HisRunTimeService : IDynamicApiController, IScoped
{
    private readonly SqlSugarRepository<HisConfig> _hisConfigRep;
    private readonly SysCacheService _sysCacheService;
    private readonly DeviceCollectService _deviceCollectService;
    private readonly HisHostService _hisService;
    public HisRunTimeService(SqlSugarRepository<HisConfig> hisConfigRep,
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
    /// 获取设备历史数据分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<HisValue>> GetHisHisPage([FromQuery] PageDeviceVariableInput input)
    {
        if (HisHostService._SqlSugarScope == null) throw new("历史服务未初始化");

        var data = await HisHostService._SqlSugarScope?.Queryable<HisValue>()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            .OrderBy(u => u.CollectTime, OrderByType.Desc).ToPagedListAsync(input.Page, input.PageSize);

        //不包含设备变量
        return data;

    }




}