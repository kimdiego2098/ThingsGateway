namespace ThingsGateway.Application.Core;

public class PageGatewayLogLogInput : BasePageInput
{
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
    /// <summary>
    /// 类别名称
    /// </summary>
    public string? LogName { get; set; }
    /// <summary>
    /// 消息
    /// </summary>
    public string? LogMessage { get; set; }
    /// <summary>
    /// 等级
    /// </summary>
    public string? LogLevel { get; set; }
}

public class GatewayLogLogInput
{
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
    /// <summary>
    /// 类别名称
    /// </summary>
    public string? LogName { get; set; }
    /// <summary>
    /// 消息
    /// </summary>
    public string? LogMessage { get; set; }
    /// <summary>
    /// 等级
    /// </summary>
    public string? LogLevel { get; set; }
}