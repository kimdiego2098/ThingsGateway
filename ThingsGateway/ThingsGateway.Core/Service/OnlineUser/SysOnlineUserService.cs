using Microsoft.AspNetCore.SignalR;

namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统在线用户服务
/// </summary>
[ApiDescriptionSettings(Order = 100)]
public class SysOnlineUserService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysOnlineUser> _sysOnlineUerRep;
    private readonly SysConfigService _sysConfigService;
    private readonly IHubContext<OnlineUserHub, IOnlineUserHub> _onlineUserHubContext;

    public SysOnlineUserService(SqlSugarRepository<SysOnlineUser> sysOnlineUerRep,
        SysConfigService sysConfigService,
        IHubContext<OnlineUserHub, IOnlineUserHub> onlineUserHubContext)
    {
        _sysOnlineUerRep = sysOnlineUerRep;
        _sysConfigService = sysConfigService;
        _onlineUserHubContext = onlineUserHubContext;
    }

    /// <summary>
    /// 获取在线用户分页列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<SysOnlineUser>> GetOnlineUserPage([FromQuery] PageOnlineUserInput input)
    {
        return await _sysOnlineUerRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.UserName), u => u.UserName.Contains(input.UserName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.RealName), u => u.RealName.Contains(input.RealName))
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 强制下线
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPost]
    [NonValidation]
    public async Task ForceOffline(SysOnlineUser user)
    {
        await _onlineUserHubContext.Clients.Client(user.ConnectionId).ForceOffline("强制下线");
        await _sysOnlineUerRep.DeleteAsync(user);
    }

    /// <summary>
    /// 发布站内消息
    /// </summary>
    /// <param name="notice"></param>
    /// <param name="userIds"></param>
    /// <returns></returns>
    [NonAction]
    public async Task PublicNotice(SysNotice notice, List<long> userIds)
    {
        var userList = await _sysOnlineUerRep.GetListAsync(m => userIds.Contains(m.UserId));
        if (!userList.Any()) return;

        foreach (var item in userList)
        {
            await _onlineUserHubContext.Clients.Client(item.ConnectionId).PublicNotice(notice);
        }
    }

    /// <summary>
    /// 单用户登录
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task SignleLogin(long userId)
    {
        if (await _sysConfigService.GetConfigValue<bool>(CommonConst.SysSingleLogin))
        {
            var user = await _sysOnlineUerRep.GetFirstAsync(u => u.UserId == userId);
            if (user == null) return;

            await ForceOffline(user);
        }
    }
}