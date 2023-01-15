//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  API首页：https://www.yuque.com/rrqm/touchsocket/index
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------



namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// TCP系列服务器接口
    /// </summary>
    public interface ITcpService<TClient> : ITcpService where TClient : ISocketClient
    {
        /// <summary>
        /// 用户连接完成
        /// </summary>
        TouchSocketEventHandler<TClient> Connected { get; set; }

        /// <summary>
        /// 有用户连接的时候
        /// </summary>
        OperationEventHandler<TClient> Connecting { get; set; }

        /// <summary>
        /// 有用户断开连接
        /// </summary>
        DisconnectEventHandler<TClient> Disconnected { get; set; }

        /// <summary>
        /// 尝试获取TClient
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="socketClient">TClient</param>
        /// <returns></returns>
        bool TryGetSocketClient(string id, out TClient socketClient);
    }

    /// <summary>
    /// TCP服务器接口
    /// </summary>
    public interface ITcpService : IService, IIDSender, IIDRequsetInfoSender, IPluginObject
    {
        /// <summary>
        /// 当前在线客户端数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取默认新ID。
        /// </summary>
        Func<string> GetDefaultNewID { get; }

        /// <summary>
        /// 获取最大可连接数
        /// </summary>
        int MaxCount { get; }

        /// <summary>
        /// 网络监听集合
        /// </summary>
        NetworkMonitor[] Monitors { get; }

        /// <summary>
        /// 获取当前连接的所有客户端
        /// </summary>
        SocketClientCollection SocketClients { get; }

        /// <summary>
        /// 使用Ssl加密
        /// </summary>
        bool UseSsl { get; }

        /// <summary>
        /// 清理当前已连接的所有客户端
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取当前在线的所有ID集合
        /// </summary>
        /// <returns></returns>
        string[] GetIDs();

        /// <summary>
        /// 重置ID
        /// </summary>
        /// <param name="oldID"></param>
        /// <param name="newID"></param>
        /// <exception cref="ClientNotFindException"></exception>
        /// <exception cref="Exception"></exception>
        void ResetID(string oldID, string newID);

        /// <summary>
        /// 根据ID判断SocketClient是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool SocketClientExist(string id);
    }
}