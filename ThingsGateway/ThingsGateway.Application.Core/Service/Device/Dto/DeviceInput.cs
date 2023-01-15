
namespace ThingsGateway.Application.Core;

public class DeviceInput : BaseIdInput
{

}

public class DeviceNameInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public long Id { get; set; }

}

public class DeviceExportInput
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


public class DeviceConfigInput : BaseIdInput
{
    /// <summary>
    /// 启用
    /// </summary>
    public bool InvokeEnable { get; set; }
}
/// <summary>
/// 分页输入的搜索参数
/// </summary>

public class PageDeviceInput : BasePageInput
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


public class AddDeviceInput : Device
{

}


public class UpdateDeviceInput : AddDeviceInput
{
}

public class DeleteDeviceInput : BaseIdInput
{
}