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
    /// TCP端口转发服务器
    /// </summary>
    public class NATService : TcpService<NATSocketClient>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        protected override NATSocketClient GetClientInstence()
        {
            var client = base.GetClientInstence();
            client.m_internalDis = OnTargetClientDisconnected;
            client.m_internalTargetClientRev = OnTargetClientReceived;
            return client;
        }

        /// <summary>
        /// 在NAT服务器收到数据时。
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="byteBlock"></param>
        /// <param name="requestInfo"></param>
        /// <returns>需要转发的数据。</returns>
        protected virtual byte[] OnNATReceived(NATSocketClient socketClient, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            return byteBlock?.ToArray();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="byteBlock"></param>
        /// <param name="requestInfo"></param>
        protected override sealed void OnReceived(NATSocketClient socketClient, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            var data = OnNATReceived(socketClient, byteBlock, requestInfo);
            if (data != null)
            {
                socketClient.SendToTargetClient(data, 0, data.Length);
            }
        }

        /// <summary>
        /// 当目标客户端断开。
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="tcpClient"></param>
        /// <param name="e"></param>
        protected virtual void OnTargetClientDisconnected(NATSocketClient socketClient, ITcpClient tcpClient, DisconnectEventArgs e)
        {
        }

        /// <summary>
        /// 在目标客户端收到数据时。
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="tcpClient"></param>
        /// <param name="byteBlock"></param>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        protected virtual byte[] OnTargetClientReceived(NATSocketClient socketClient, ITcpClient tcpClient, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            return byteBlock?.ToArray();
        }
    }
}