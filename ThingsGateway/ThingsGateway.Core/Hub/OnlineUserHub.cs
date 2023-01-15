using Furion.InstantMessaging;

using Microsoft.AspNetCore.SignalR;

namespace ThingsGateway.Core;

/// <summary>
/// 在线用户集线器
/// </summary>
[MapHub("/hubs/onlineUser")]
public class OnlineUserHub : Hub<IOnlineUserHub>
{
    private const string GROUP_ONLINE = "GROUP_ONLINE_"; // 租户分组前缀

    private readonly SqlSugarRepository<SysOnlineUser> _sysOnlineUerRep;
    private readonly SysMessageService _sysMessageService;
    private readonly IHubContext<OnlineUserHub, IOnlineUserHub> _onlineUserHubContext;

    public OnlineUserHub(SqlSugarRepository<SysOnlineUser> sysOnlineUerRep,
        SysMessageService sysMessageService,
        IHubContext<OnlineUserHub, IOnlineUserHub> onlineUserHubContext)
    {
        _sysOnlineUerRep = sysOnlineUerRep;
        _sysMessageService = sysMessageService;
        _onlineUserHubContext = onlineUserHubContext;
    }

    /// <summary>
    /// 连接
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var token = Context.GetHttpContext().Request.Query["access_token"];
        var claims = JWTEncryption.ReadJwtToken(token)?.Claims;
        var client = Parser.GetDefault().Parse(Context.GetHttpContext().Request.Headers["User-Agent"]);
        var user = new SysOnlineUser
        {
            ConnectionId = Context.ConnectionId,
            UserId = long.Parse(claims.FirstOrDefault(u => u.Type == ClaimConst.UserId)?.Value),
            UserName = claims.FirstOrDefault(u => u.Type == ClaimConst.Account)?.Value,
            RealName = claims.FirstOrDefault(u => u.Type == ClaimConst.RealName)?.Value,
            Time = DateTime.Now,
            Ip = App.HttpContext.GetRemoteIpAddressToIPv4(),
            Browser = client.UA.Family + client.UA.Major,
            Os = client.OS.Family + client.OS.Major,
            TenantId = Convert.ToInt64(claims.FirstOrDefault(u => u.Type == ClaimConst.TenantId)?.Value),
        };
        await _sysOnlineUerRep.InsertAsync(user);

        // 以租户Id分组方便区分
        var groupName = $"{GROUP_ONLINE}{user.TenantId}";
        await _onlineUserHubContext.Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var userList = await _sysOnlineUerRep.AsQueryable().Filter("", true)
            .Where(u => u.TenantId == user.TenantId).Take(10).ToListAsync();
        await _onlineUserHubContext.Clients.Groups(groupName).OnlineUserList(new OnlineUserList
        {
            RealName = user.RealName,
            Online = true,
            UserList = userList
        });
    }

    /// <summary>
    /// 断开
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (string.IsNullOrEmpty(Context.ConnectionId)) return;

        var user = await _sysOnlineUerRep.AsQueryable().Filter("", true).FirstAsync(u => u.ConnectionId == Context.ConnectionId);
        if (user == null) return;

        await _sysOnlineUerRep.DeleteAsync(u => u.Id == user.Id);

        // 通知当前组用户变动
        var userList = await _sysOnlineUerRep.AsQueryable().Filter("", true)
            .Where(u => u.TenantId == user.TenantId).Take(10).ToListAsync();
        await _onlineUserHubContext.Clients.Groups($"{GROUP_ONLINE}{user.TenantId}").OnlineUserList(new OnlineUserList
        {
            RealName = user.RealName,
            Online = false,
            UserList = userList
        });
    }

    /// <summary>
    /// 强制下线
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task ForceOffline(OnlineUserHubInput input)
    {
        await _onlineUserHubContext.Clients.Client(input.ConnectionId).ForceOffline("强制下线");
    }

    /// <summary>
    /// 前端调用发送方法（发送信息给某个人）
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task ClientsSendMessage(MessageInput message)
    {
        await _sysMessageService.SendMessageToUser(message);
    }

    /// <summary>
    /// 前端调用发送方法（发送信息给所有人）
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task ClientsSendMessagetoAll(MessageInput message)
    {
        await _sysMessageService.SendMessageToAllUser(message);
    }

    /// <summary>
    /// 前端调用发送方法（发送消息给除了发送人的其他人）
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task ClientsSendMessagetoOther(MessageInput message)
    {
        await _sysMessageService.SendMessageToOtherUser(message);
    }

    /// <summary>
    /// 前端调用发送方法（发送消息给某些人）
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task ClientsSendMessagetoUsers(MessageInput message)
    {
        await _sysMessageService.SendMessageToUsers(message);
    }
}