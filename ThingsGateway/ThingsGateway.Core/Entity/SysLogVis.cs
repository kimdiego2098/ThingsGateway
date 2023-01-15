namespace ThingsGateway.Core;

/// <summary>
/// 系统访问日志表
/// </summary>
[SugarTable(null, "系统访问日志表")]
[SystemTable]
public class SysLogVis : EntityTenant
{
    /// <summary>
    /// 是否执行成功（Y-是，N-否）
    /// </summary>
    [SugarColumn(ColumnDescription = "是否执行成功")]
    public YesNoEnum Success { get; set; }

    /// <summary>
    /// 具体消息
    /// </summary>
    [SugarColumn(ColumnDescription = "具体消息", ColumnDataType = "longtext,text,clob")]
    public string? Message { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [SugarColumn(ColumnDescription = "IP地址", Length = 16)]
    [MaxLength(16)]
    public string? Ip { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(ColumnDescription = "地址", Length = 128)]
    [MaxLength(128)]
    public string? Location { get; set; }

    /// <summary>
    /// 浏览器
    /// </summary>
    [SugarColumn(ColumnDescription = "浏览器", Length = 128)]
    [MaxLength(128)]
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(ColumnDescription = "操作系统", Length = 128)]
    [MaxLength(128)]
    public string? Os { get; set; }

    /// <summary>
    /// 访问类型
    /// </summary>
    [SugarColumn(ColumnDescription = "访问类型")]
    public LoginTypeEnum VisType { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    [SugarColumn(ColumnDescription = "账号", Length = 32)]
    [MaxLength(32)]
    public string? Account { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "真实姓名", Length = 32)]
    [MaxLength(32)]
    public string? RealName { get; set; }
}