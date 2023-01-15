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
using System
.Threading.Tasks;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 端口转发辅助
    /// </summary>
    public class NATSocketClient : SocketClient
    {
        internal Action<NATSocketClient, ITcpClient, DisconnectEventArgs> m_internalDis;
        internal Func<NATSocketClient, ITcpClient, ByteBlock, IRequestInfo, byte[]> m_internalTargetClientRev;
        private readonly ConcurrentList<ITcpClient> m_targetClients = new ConcurrentList<ITcpClient>();

        /// <summary>
        /// 添加转发客户端。
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="setupAction">当完成配置，但是还未连接时回调。</param>
        /// <returns></returns>
        public ITcpClient AddTargetClient(TouchSocketConfig config, Action<ITcpClient> setupAction = default)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Disconnected += TcpClient_Disconnected;
            tcpClient.Received += TcpClient_Received;
            tcpClient.Setup(config);
            setupAction?.Invoke(tcpClient);
            tcpClient.Connect();

            m_targetClients.Add(tcpClient);
            return tcpClient;
        }

        /// <summary>
        /// 添加转发客户端。
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="setupAction">当完成配置，但是还未连接时回调。</param>
        /// <returns></returns>
        public Task<ITcpClient> AddTargetClientAsync(TouchSocketConfig config, Action<ITcpClient> setupAction = default)
        {
            return EasyTask.Run(() =>
            {
                return AddTargetClient(config, setupAction);
            });
        }

        /// <summary>
        /// 获取所有目标客户端
        /// </summary>
        /// <returns></returns>
        public ITcpClient[] GetTargetClients()
        {
            return m_targetClients.ToArray();
        }

        /// <summary>
        /// 发送数据到全部转发端。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void SendToTargetClient(byte[] buffer, int offset, int length)
        {
            foreach (var socket in m_targetClients)
            {
                try
                {
                    socket.Send(buffer, offset, length);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDisconnected(DisconnectEventArgs e)
        {
            foreach (var client in m_targetClients)
            {
                client.TryShutdown();
                client.SafeDispose();
            }
            base.OnDisconnected(e);
        }

        private void TcpClient_Disconnected(ITcpClientBase client, DisconnectEventArgs e)
        {
            client.Dispose();
            m_targetClients.Remove((ITcpClient)client);
            m_internalDis?.Invoke(this, (ITcpClient)client, e);
        }

        private void TcpClient_Received(TcpClient client, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            if (DisposedValue)
            {
                return;
            }

            try
            {
                var data = m_internalTargetClientRev?.Invoke(this, client, byteBlock, requestInfo);
                if (data != null)
                {
                    if (Online)
                    {
                        this.Send(data);
                    }
                }
            }
            catch
            {
            }
        }
    }
}