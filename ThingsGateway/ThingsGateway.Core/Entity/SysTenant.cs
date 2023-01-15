namespace ThingsGateway.Core;

/// <summary>
/// 系统租户表
/// </summary>
[SugarTable(null, "系统租户表")]
[SystemTable]
public class SysTenant : EntityBase
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "机构Id")]
    public long OrgId { get; set; }

    /// <summary>
    /// 主机
    /// </summary>
    [SugarColumn(ColumnDescription = "主机", Length = 128)]
    [MaxLength(128)]
    public string? Host { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    [SugarColumn(ColumnDescription = "租户类型")]
    public TenantTypeEnum TenantType { get; set; }

    /// <summary>
    /// 数据库类型
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库类型")]
    public SqlSugar.DbType DbType { get; set; }

    /// <summary>
    /// 数据库连接
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库连接", Length = 256)]
    [MaxLength(256)]
    public string? Connection { get; set; }

    /// <summary>
    /// 数据库标识
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库标识", Length = 64)]
    [MaxLength(64)]
    public string? ConfigId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 128)]
    [MaxLength(128)]
    public string? Remark { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;
}