namespace ThingsGateway.Core;

/// <summary>
/// 系统通知公告用户表
/// </summary>
[SugarTable(null, "系统通知公告用户表")]
[SystemTable]
public class SysNoticeUser
{
    /// <summary>
    /// 通知公告Id
    /// </summary>
    [SugarColumn(ColumnDescription = "通知公告Id")]
    public long NoticeId { get; set; }

    /// <summary>
    /// 通知公告
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(NoticeId))]
    public SysNotice SysNotice { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    [SugarColumn(ColumnDescription = "阅读时间")]
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// 状态（0未读 1已读）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态（0未读 1已读）")]
    public NoticeUserStatusEnum ReadStatus { get; set; } = NoticeUserStatusEnum.UNREAD;
}