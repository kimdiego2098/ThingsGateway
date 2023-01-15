namespace ThingsGateway.Core.Service;

/// <summary>
/// 授权角色菜单
/// </summary>
public class RoleMenuInput : BaseIdInput
{
    /// <summary>
    /// 菜单Id集合
    /// </summary>
    public List<long> MenuIdList { get; set; }
}