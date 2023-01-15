namespace ThingsGateway.Core.Service;

public class JobDetailInput
{
    /// <summary>
    /// 作业Id
    /// </summary>
    public string JobId { get; set; }
}

public class PageJobInput : BasePageInput
{
    /// <summary>
    /// 作业Id
    /// </summary>
    public string JobId { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    public string Description { get; set; }
}

public class AddJobDetailInput : SysJobDetail
{
    /// <summary>
    /// 作业Id
    /// </summary>
    [Required(ErrorMessage = "作业Id不能为空"), MinLength(2, ErrorMessage = "作业Id不能少于2个字符")]
    public override string JobId { get; set; }
}

public class UpdateJobDetailInput : AddJobDetailInput
{
}

public class DeleteJobDetailInput : JobDetailInput
{
}