namespace ThingsGateway.Core.Service;

public class RoleInput : BaseIdInput
{
    /// <summary>
    /// 状态
    /// </summary>
    public virtual StatusEnum Status { get; set; }
}

public class PageRoleInput : BasePageInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public virtual string Code { get; set; }
}

public class AddRoleInput : SysRole
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    public override string Name { get; set; }

    /// <summary>
    /// 菜单Id集合
    /// </summary>
    public List<long> MenuIdList { get; set; }
}

public class UpdateRoleInput : AddRoleInput
{
}

public class DeleteRoleInput : BaseIdInput
{
}