namespace ThingsGateway.Core;

/// <summary>
/// 系统差异日志表
/// </summary>
[SugarTable(null, "系统差异日志表")]
[SystemTable]
public class SysLogDiff : EntityBase
{
    /// <summary>
    /// 操作前记录
    /// </summary>
    [SugarColumn(ColumnDescription = "操作前记录", ColumnDataType = "longtext,text,clob")]
    public string? BeforeData { get; set; }

    /// <summary>
    /// 操作后记录
    /// </summary>
    [SugarColumn(ColumnDescription = "操作后记录", ColumnDataType = "longtext,text,clob")]
    public string? AfterData { get; set; }

    /// <summary>
    /// Sql
    /// </summary>
    [SugarColumn(ColumnDescription = "Sql", ColumnDataType = "longtext,text,clob")]
    public string? Sql { get; set; }

    /// <summary>
    /// 参数  手动传入的参数
    /// </summary>
    [SugarColumn(ColumnDescription = "参数", ColumnDataType = "longtext,text,clob")]
    public string? Parameters { get; set; }

    /// <summary>
    /// 业务对象
    /// </summary>
    [SugarColumn(ColumnDescription = "业务对象", ColumnDataType = "longtext,text,clob")]
    public string? BusinessData { get; set; }

    /// <summary>
    /// 差异操作
    /// </summary>
    [SugarColumn(ColumnDescription = "差异操作", ColumnDataType = "longtext,text,clob")]
    public string? DiffType { get; set; }

    /// <summary>
    /// 耗时
    /// </summary>
    [SugarColumn(ColumnDescription = "耗时")]
    public long Duration { get; set; }
}