namespace ThingsGateway.Core;

/// <summary>
/// 系统用户扩展机构表种子数据
/// </summary>
public class SysUserExtOrgSeedData : ISqlSugarEntitySeedData<SysUserExtOrg>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<SysUserExtOrg> HasData()
    {
        return new[]
        {
            new SysUserExtOrg{ Id=252885263003001, UserId=252885263003721, OrgId=252885263003731, PosId=252885263003725 },
            new SysUserExtOrg{ Id=252885263003002, UserId=252885263003724, OrgId=252885263003741, PosId=252885263003728  }
        };
    }
}