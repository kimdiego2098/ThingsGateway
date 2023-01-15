namespace ThingsGateway.Core;

/// <summary>
/// 系统审计日志表
/// </summary>
[SugarTable(null, "系统审计日志表")]
[SystemTable]
public class SysLogAudit : EntityBase
{
    /// <summary>
    /// 表名
    /// </summary>
    [SugarColumn(ColumnDescription = "表名", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string TableName { get; set; }

    /// <summary>
    /// 列名
    /// </summary>
    [SugarColumn(ColumnDescription = "列名", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string ColumnName { get; set; }

    /// <summary>
    /// 新值
    /// </summary>
    [SugarColumn(ColumnDescription = "新值", ColumnDataType = "longtext,text,clob")]
    public string? NewValue { get; set; }

    /// <summary>
    /// 旧值
    /// </summary>
    [SugarColumn(ColumnDescription = "旧值", ColumnDataType = "longtext,text,clob")]
    public string? OldValue { get; set; }

    /// <summary>
    /// 操作方式（新增、更新、删除）
    /// </summary>
    [SugarColumn(ColumnDescription = "操作方式")]
    public DataOpTypeEnum Operate { get; set; }

    /// <summary>
    /// 审计时间
    /// </summary>
    [SugarColumn(ColumnDescription = "审计时间")]
    public DateTime? AuditTime { get; set; }

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