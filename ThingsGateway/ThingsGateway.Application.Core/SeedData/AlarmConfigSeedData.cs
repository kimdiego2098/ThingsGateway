

namespace ThingsGateway.Application.Core;


/// <summary>
/// 实体种子数据
/// </summary>
public class AlarmConfigSeedData : ISqlSugarEntitySeedData<AlarmConfig>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<AlarmConfig> HasData()
    {
        yield return new AlarmConfig
        {
            Id = 1,
            ConnStr = "2F0DE1D99B8385EE2431027193934D57F2E9BA293B6D1109238110D2ABAD109C1CE3AA6F1D7D948C7EDCC934AAFC8130CF8EB04236027847",
            DbType = 0,
            Enable = false,
        };
    }
}