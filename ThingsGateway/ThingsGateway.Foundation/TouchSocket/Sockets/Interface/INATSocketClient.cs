using System
.Threading.Tasks;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// INATSocketClient
    /// </summary>
    public interface INATSocketClient : ISocketClient
    {
        /// <summary>
        /// 添加转发客户端。
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="setupAction">当完成配置，但是还未连接时回调。</param>
        /// <returns></returns>
        ITcpClient AddTargetClient(TouchSocketConfig config, Action<ITcpClient> setupAction = null);

        /// <summary>
        /// 添加转发客户端。
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="setupAction">当完成配置，但是还未连接时回调。</param>
        /// <returns></returns>
        Task<ITcpClient> AddTargetClientAsync(TouchSocketConfig config, Action<ITcpClient> setupAction = null);

        /// <summary>
        /// 获取所有目标客户端
        /// </summary>
        /// <returns></returns>
        ITcpClient[] GetTargetClients();

        /// <summary>
        /// 发送数据到全部转发端。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        void SendToTargetClient(byte[] buffer, int offset, int length);
    }
}