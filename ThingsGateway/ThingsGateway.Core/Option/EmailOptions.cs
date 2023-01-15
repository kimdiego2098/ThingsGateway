using NETCore.MailKit.Infrastructure.Internal;

namespace ThingsGateway.Core;

/// <summary>
/// 邮件配置选项
/// </summary>
public sealed class EmailOptions : MailKitOptions, IConfigurableOptions
{
    /// <summary>
    /// 接收人邮箱
    /// </summary>
    public string ToEmail { get; set; }
}