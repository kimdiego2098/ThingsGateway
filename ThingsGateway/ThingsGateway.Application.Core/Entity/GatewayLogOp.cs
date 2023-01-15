namespace ThingsGateway.Application.Core;

/// <summary>
/// 采集网关日志表
/// </summary>
[SugarTable(null, "采集网关日志表")]
[SystemTable]
public class GatewayLogOp : EntityBase
{
    /// <summary>
    /// 记录器类别名称
    /// </summary>
    [SugarColumn(ColumnDescription = "记录器类别名称", Length = 256)]
    [MaxLength(256)]
    public string? LogName { get; set; }

    /// <summary>
    /// 日志级别
    /// </summary>
    [SugarColumn(ColumnDescription = "日志级别", Length = 16)]
    [MaxLength(16)]
    public string? LogLevel { get; set; }

    /// <summary>
    /// 事件Id
    /// </summary>
    [SugarColumn(ColumnDescription = "事件Id", ColumnDataType = "longtext,text,clob")]
    public string? EventId { get; set; }

    /// <summary>
    /// 日志消息
    /// </summary>
    [SugarColumn(ColumnDescription = "日志消息", ColumnDataType = "longtext,text,clob")]
    public string? Message { get; set; }

    /// <summary>
    /// 异常对象
    /// </summary>
    [SugarColumn(ColumnDescription = "异常对象", ColumnDataType = "longtext,text,clob")]
    public string? Exception { get; set; }

    /// <summary>
    /// 当前状态值
    /// </summary>
    [SugarColumn(ColumnDescription = "当前状态值", ColumnDataType = "longtext,text,clob")]
    public string? State { get; set; }

    /// <summary>
    /// 日志记录时间
    /// </summary>
    [SugarColumn(ColumnDescription = "日志记录时间")]
    public DateTime LogDateTime { get; set; }

    /// <summary>
    /// 线程Id
    /// </summary>
    [SugarColumn(ColumnDescription = "线程Id")]
    public int ThreadId { get; set; }
    /// <summary>
    /// 请求跟踪Id
    /// </summary>
    [SugarColumn(ColumnDescription = "请求跟踪Id", Length = 128)]
    [MaxLength(128)]
    public string? TraceId { get; set; }
}