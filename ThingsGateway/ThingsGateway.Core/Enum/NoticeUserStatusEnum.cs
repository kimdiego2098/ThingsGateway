namespace ThingsGateway.Core;

/// <summary>
/// 通知公告用户状态枚举
/// </summary>
public enum NoticeUserStatusEnum
{
    /// <summary>
    /// 未读
    /// </summary>
    [Description("未读")]
    UNREAD = 0,

    /// <summary>
    /// 已读
    /// </summary>
    [Description("已读")]
    READ = 1
}