namespace ThingsGateway.Core.Service;

public class PageNoticeInput : BasePageInput
{
    /// <summary>
    /// 标题
    /// </summary>
    public virtual string Title { get; set; }

    /// <summary>
    /// 类型（1通知 2公告）
    /// </summary>
    public virtual NoticeTypeEnum Type { get; set; }
}

public class AddNoticeInput : SysNotice
{
}

public class UpdateNoticeInput : AddNoticeInput
{
}

public class DeleteNoticeInput : BaseIdInput
{
}

public class NoticeInput : BaseIdInput
{
}