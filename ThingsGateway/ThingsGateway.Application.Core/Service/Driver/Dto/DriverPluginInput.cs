

namespace ThingsGateway.Application.Core;


public class DriverPluginInput : BaseIdInput
{
}

/// <summary>
/// 分页输入的搜索参数
/// </summary>

public class PageDriverPluginInput : BasePageInput
{
    /// <summary>
    /// 驱动名称
    /// </summary>
    public string PluginName { get; set; }


    /// <summary>
    /// 名称
    /// </summary>
    public string FileName { get; set; }


}


public class AddDriverPluginInput : DriverPlugin
{

}


public class UpdateDriverPluginInput : AddDriverPluginInput
{
}

public class DeleteDriverPluginInput : BaseIdInput
{
}