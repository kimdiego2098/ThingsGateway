namespace ThingsGateway.Core.Service;

/// <summary>
/// 授权角色机构
/// </summary>
public class RoleOrgInput : BaseIdInput
{
    /// <summary>
    /// 数据范围
    /// </summary>
    public int DataScope { get; set; }

    /// <summary>
    /// 机构Id集合
    /// </summary>
    public List<long> OrgIdList { get; set; }
}