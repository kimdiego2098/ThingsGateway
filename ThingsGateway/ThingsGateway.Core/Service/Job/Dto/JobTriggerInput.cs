namespace ThingsGateway.Core.Service;

public class JobTriggerInput
{
    /// <summary>
    /// 作业Id
    /// </summary>
    public string JobId { get; set; }

    /// <summary>
    /// 触发器Id
    /// </summary>
    public string TriggerId { get; set; }
}

public class AddJobTriggerInput : SysJobTrigger
{
    /// <summary>
    /// 作业Id
    /// </summary>
    [Required(ErrorMessage = "作业Id不能为空"), MinLength(2, ErrorMessage = "作业Id不能少于2个字符")]
    public override string JobId { get; set; }

    /// <summary>
    /// 触发器Id
    /// </summary>
    [Required(ErrorMessage = "触发器Id不能为空"), MinLength(2, ErrorMessage = "触发器Id不能少于2个字符")]
    public override string TriggerId { get; set; }
}

public class UpdateJobTriggerInput : AddJobTriggerInput
{
}

public class DeleteJobTriggerInput : JobTriggerInput
{
}