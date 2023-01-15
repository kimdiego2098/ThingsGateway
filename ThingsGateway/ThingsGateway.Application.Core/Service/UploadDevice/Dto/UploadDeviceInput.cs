
namespace ThingsGateway.Application.Core;

public class UploadDeviceInput : BaseIdInput
{

}

public class UploadDeviceNameInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public long Id { get; set; }

}

public class UploadDeviceExportInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 驱动名称
    /// </summary>
    public string PluginName { get; set; }
}


public class UploadDeviceConfigInput : BaseIdInput
{
    /// <summary>
    /// 启用
    /// </summary>
    public bool InvokeEnable { get; set; }
}
/// <summary>
/// 分页输入的搜索参数
/// </summary>

public class PageUploadDeviceInput : BasePageInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 驱动名称
    /// </summary>
    public string PluginName { get; set; }
}


public class AddUploadDeviceInput : UploadDevice
{

}


public class UpdateUploadDeviceInput : AddUploadDeviceInput
{
}

public class DeleteUploadDeviceInput : BaseIdInput
{
}