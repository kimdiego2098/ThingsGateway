namespace ThingsGateway.Core.Service;

public class PosInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }
}

public class AddPosInput : SysPos
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "职位名称不能为空")]
    public override string Name { get; set; }
}

public class UpdatePosInput : AddPosInput
{
}

public class DeletePosInput : BaseIdInput
{
}