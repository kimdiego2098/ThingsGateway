namespace ThingsGateway.Core.Service;

/// <summary>
/// 授权用户角色
/// </summary>
public class UserRoleInput
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 角色Id集合
    /// </summary>
    public List<long> RoleIdList { get; set; }
}