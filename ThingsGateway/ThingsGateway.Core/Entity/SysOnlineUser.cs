namespace ThingsGateway.Core;

/// <summary>
/// 系统在线用户表
/// </summary>
[SugarTable(null, "系统在线用户表")]
[SystemTable]
public class SysOnlineUser : EntityTenantId
{
    /// <summary>
    /// 连接Id
    /// </summary>
    [SugarColumn(ColumnDescription = "连接Id")]
    public string? ConnectionId { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    [SugarColumn(ColumnDescription = "账号", Length = 32)]
    [Required, MaxLength(32)]
    public virtual string UserName { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "真实姓名", Length = 32)]
    [MaxLength(32)]
    public string? RealName { get; set; }

    /// <summary>
    /// 连接时间
    /// </summary>
    [SugarColumn(ColumnDescription = "连接时间")]
    public DateTime? Time { get; set; }

    /// <summary>
    /// 连接IP
    /// </summary>
    [SugarColumn(ColumnDescription = "连接IP", Length = 16)]
    [MaxLength(16)]
    public string? Ip { get; set; }

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
}