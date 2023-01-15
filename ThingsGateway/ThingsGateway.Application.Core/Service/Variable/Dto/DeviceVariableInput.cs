
namespace ThingsGateway.Application.Core;

public class DeviceVariableInput : BaseIdInput
{

}

public class DeviceVariableExportInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; set; }
}

public class DeviceVariableConfigInput : BaseIdInput
{
    /// <summary>
    /// 启用
    /// </summary>
    public bool InvokeEnable { get; set; }
}
/// <summary>
/// 分页输入的搜索参数
/// </summary>

public class PageDeviceVariableInput : BasePageInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; set; }


}
/// <summary>
/// 分页输入的搜索参数
/// </summary>

public class RunTimePageDeviceVariableInput : BasePageInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; set; }

    /// <summary>
    /// 变量描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 质量戳
    /// </summary>
    public string Quality { get; set; }
    /// <summary>
    /// 执行参数
    /// </summary>
    public string VariableAddress { get; set; }
}

public class AddDeviceVariableInput : DeviceVariable
{

}


public class UpdateDeviceVariableInput : AddDeviceVariableInput
{
}

public class DeleteDeviceVariableInput : BaseIdInput
{
}



public class WriteDeviceVariableInput
{
    public string Name { get; set; }
    public string WriteValue { get; set; }
}
