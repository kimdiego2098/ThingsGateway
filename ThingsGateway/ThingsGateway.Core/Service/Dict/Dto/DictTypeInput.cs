namespace ThingsGateway.Core.Service;

public class DictTypeInput : BaseIdInput
{
    /// <summary>
    /// 状态
    /// </summary>
    public StatusEnum Status { get; set; }
}

public class PageDictTypeInput : BasePageInput
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

public class AddDictTypeInput : SysDictType
{
}

public class UpdateDictTypeInput : AddDictTypeInput
{
}

public class DeleteDictTypeInput : BaseIdInput
{
}

public class GetDataDictTypeInput
{
    /// <summary>
    /// 编码
    /// </summary>
    [Required(ErrorMessage = "字典类型编码不能为空")]
    public string Code { get; set; }
}