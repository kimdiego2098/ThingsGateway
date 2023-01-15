namespace ThingsGateway.Application.Core;
/// <summary>
/// 上传插件表
/// </summary>
[SugarTable("upLoad_plugin")]
[Description("上传插件表")]
[Tenant(ApplicationConst.ConfigId)]
[SystemTable]
public class UploadPlugin : EntityBase
{
    #region Public Properties
    /// <summary>
    /// 驱动名称
    /// </summary>
    [SugarColumn(ColumnDescription = "上传插件名称")]
    public string PluginName { get; set; }
    /// <summary>
    /// 驱动文件全路径
    /// </summary>
    [SugarColumn(ColumnDescription = "上传插件文件全路径")]
    public string FileName { get; set; }

    #endregion Public Properties
}
