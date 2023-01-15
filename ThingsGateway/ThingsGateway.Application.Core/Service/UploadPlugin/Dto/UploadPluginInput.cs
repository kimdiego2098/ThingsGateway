

namespace ThingsGateway.Application.Core;


public class UploadPluginInput : BaseIdInput
{
}

/// <summary>
/// 分页输入的搜索参数
/// </summary>

public class PageUploadPluginInput : BasePageInput
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


public class AddUploadPluginInput : UploadPlugin
{

}


public class UpdateUploadPluginInput : AddUploadPluginInput
{
}

public class DeleteUploadPluginInput : BaseIdInput
{
}