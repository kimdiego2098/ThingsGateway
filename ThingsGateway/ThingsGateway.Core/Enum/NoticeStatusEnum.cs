namespace ThingsGateway.Core;

/// <summary>
/// 通知公告状态枚举
/// </summary>
public enum NoticeStatusEnum
{
    /// <summary>
    /// 草稿
    /// </summary>
    [Description("草稿")]
    DRAFT = 0,

    /// <summary>
    /// 发布
    /// </summary>
    [Description("发布")]
    PUBLIC = 1,

    /// <summary>
    /// 撤回
    /// </summary>
    [Description("撤回")]
    CANCEL = 2,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    DELETED = 3
}