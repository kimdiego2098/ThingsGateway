namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统通知公告服务
/// </summary>
[ApiDescriptionSettings(Order = 100)]
public class SysNoticeService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly SqlSugarRepository<SysNotice> _sysNoticeRep;
    private readonly SqlSugarRepository<SysNoticeUser> _sysNoticeUserRep;
    private readonly SysOnlineUserService _sysOnlineUserService;

    public SysNoticeService(
        UserManager userManager,
        SqlSugarRepository<SysUser> sysUserRep,
        SqlSugarRepository<SysNotice> sysNoticeRep,
        SqlSugarRepository<SysNoticeUser> sysNoticeUserRep,
        SysOnlineUserService sysOnlineUserService)
    {
        _userManager = userManager;
        _sysUserRep = sysUserRep;
        _sysNoticeRep = sysNoticeRep;
        _sysNoticeUserRep = sysNoticeUserRep;
        _sysOnlineUserService = sysOnlineUserService;
    }

    /// <summary>
    /// 获取通知公告分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<SysNotice>> GetPageNotice([FromQuery] PageNoticeInput input)
    {
        return await _sysNoticeRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Title), u => u.Title.Contains(input.Title.Trim()))
            .WhereIF(input.Type > 0, u => u.Type == input.Type)
            .WhereIF(!_userManager.SuperAdmin, u => u.CreateUserId == _userManager.UserId)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加通知公告
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AddNotice(AddNoticeInput input)
    {
        var notice = input.Adapt<SysNotice>();
        InitNoticeInfo(notice);
        await _sysNoticeRep.InsertAsync(notice);
    }

    /// <summary>
    /// 更新通知公告
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [UnitOfWork]
    public async Task UpdateNotice(UpdateNoticeInput input)
    {
        var notice = input.Adapt<SysNotice>();
        InitNoticeInfo(notice);
        await _sysNoticeRep.UpdateAsync(notice);
    }

    /// <summary>
    /// 删除通知公告
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [UnitOfWork]
    public async Task DeleteNotice(DeleteNoticeInput input)
    {
        await _sysNoticeRep.DeleteAsync(u => u.Id == input.Id);

        await _sysNoticeUserRep.DeleteAsync(u => u.NoticeId == input.Id);
    }

    /// <summary>
    /// 发布通知公告
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task PublicNotice(NoticeInput input)
    {
        // 更新发布状态和时间
        await _sysNoticeRep.UpdateAsync(u => new SysNotice() { Status = NoticeStatusEnum.PUBLIC, PublicTime = DateTime.Now }, u => u.Id == input.Id);

        var notice = await _sysNoticeRep.GetFirstAsync(u => u.Id == input.Id);

        // 通知到的人(所有账号)
        var userIdList = await _sysUserRep.AsQueryable().Select(u => u.Id).ToListAsync();

        await _sysNoticeUserRep.DeleteAsync(u => u.NoticeId == notice.Id);
        var noticeUserList = userIdList.Select(u => new SysNoticeUser
        {
            NoticeId = notice.Id,
            UserId = u,
        }).ToList();
        await _sysNoticeUserRep.InsertRangeAsync(noticeUserList);

        // 广播所有在线账号
        await _sysOnlineUserService.PublicNotice(notice, userIdList);
    }

    /// <summary>
    /// 设置通知公告已读状态
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task SetNoticeRead(NoticeInput input)
    {
        await _sysNoticeUserRep.UpdateAsync(u => new SysNoticeUser
        {
            ReadStatus = NoticeUserStatusEnum.READ,
            ReadTime = DateTime.Now
        }, u => u.NoticeId == input.Id && u.UserId == _userManager.UserId);
    }

    /// <summary>
    /// 获取接收的通知公告（当前用户）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<SysNoticeUser>> GetPageReceivedNotice([FromQuery] PageNoticeInput input)
    {
        return await _sysNoticeRep.AsSugarClient().Queryable<SysNoticeUser>().Includes(u => u.SysNotice)
            .Where(u => u.UserId == _userManager.UserId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Title), u => u.SysNotice.Title.Contains(input.Title.Trim()))
            .WhereIF(input.Type > 0, u => u.SysNotice.Type == input.Type)
            .OrderBy(u => u.SysNotice.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取未读的通知公告（当前用户）
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<SysNotice>> GetUnReadNoticeList()
    {
        var noticeUserList = await _sysNoticeRep.AsSugarClient().Queryable<SysNoticeUser>().Includes(u => u.SysNotice)
            .Where(u => u.UserId == _userManager.UserId && u.ReadStatus == NoticeUserStatusEnum.UNREAD)
            .OrderBy(u => u.SysNotice.CreateTime, OrderByType.Desc).ToListAsync();
        return noticeUserList.Select(t => t.SysNotice).ToList();
    }

    /// <summary>
    /// 初始化通知公告信息
    /// </summary>
    /// <param name="notice"></param>
    [NonAction]
    private void InitNoticeInfo(SysNotice notice)
    {
        notice.PublicUserId = _userManager.UserId;
        notice.PublicUserName = _userManager.RealName;
        notice.PublicOrgId = _userManager.OrgId;
    }
}