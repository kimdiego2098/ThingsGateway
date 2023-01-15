

namespace ThingsGateway.Application.Core;
/// <summary>
/// 驱动信息表
/// </summary>
[SugarTable("driver")]
[Description("驱动信息表")]
[Tenant(ApplicationConst.ConfigId)]
[SystemTable]
public class DriverPlugin : EntityBase
{
    #region Public Properties

    /// <summary>
    /// 驱动名称
    /// </summary>
    [SugarColumn(ColumnDescription = "驱动名称")]
    public string PluginName { get; set; }
    /// <summary>
    /// 驱动文件全路径
    /// </summary>
    [SugarColumn(ColumnDescription = "驱动文件全路径")]
    public string FileName { get; set; }

    #endregion Public Properties

}
