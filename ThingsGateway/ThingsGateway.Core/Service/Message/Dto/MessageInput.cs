namespace ThingsGateway.Core;

public class MessageInput
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 用户ID列表
    /// </summary>
    public List<long> UserIds { get; set; }

    /// <summary>
    /// 消息标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public MessageTypeEnum MessageType { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Message { get; set; }
}