namespace ThingsGateway.Core;

public interface IOnlineUserHub
{
    /// <summary>
    /// 在线用户列表
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task OnlineUserList(OnlineUserList context);

    /// <summary>
    /// 强制下线
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task ForceOffline(object context);

    /// <summary>
    /// 发布站内消息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task PublicNotice(SysNotice context);

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task ReceiveMessage(object context);
}