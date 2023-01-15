namespace ThingsGateway.Core.Service;

public class TenantOutput : SysTenant
{
    /// <summary>
    /// 租户名称
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// 管理员
    /// </summary>
    public virtual string AdminName { get; set; }

    /// <summary>
    /// 电子邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string Phone { get; set; }
}