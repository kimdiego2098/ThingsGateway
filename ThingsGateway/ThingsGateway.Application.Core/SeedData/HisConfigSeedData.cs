

namespace ThingsGateway.Application.Core;


/// <summary>
/// 实体种子数据
/// </summary>
public class HisConfigSeedData : ISqlSugarEntitySeedData<HisConfig>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<HisConfig> HasData()
    {
        yield return new HisConfig
        {
            Id = 1,
            ConnStr = "6E4C273F8B34F6805460FB2464F3D639FD610931C1EED6FC039D2EB20877DC43837188B6765EAA92CCECB378BF034B9B2CFB2C767C7F49C6E22BE3D0E0166CACFD49B3F1CB3CE2C9D977BF28666D24BDC89E04776F4FEDE71612E3C4A0578B85BE7107E371A3712D30EF6B57E2F40AE9CAD45611DC4F0185361F7E62A1B5BD0C",
            DbType = 0,
            Enable = false,
        };
    }
}