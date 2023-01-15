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

using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// Tcp系插件接口
    /// </summary>
    public interface ITcpPlugin : IPlugin
    {
        /// <summary>
        /// 客户端连接成功后触发
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        [AsyncRaiser]
        void OnConnected(ITcpClientBase client, TouchSocketEventArgs e);

        /// <summary>
        /// 客户端连接成功后触发
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnConnectedAsync(ITcpClientBase client, TouchSocketEventArgs e);

        /// <summary>
        ///在即将完成连接时触发。
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        [AsyncRaiser]
        void OnConnecting(ITcpClientBase client, OperationEventArgs e);

        /// <summary>
        /// 在即将完成连接时触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnConnectingAsync(ITcpClientBase client, OperationEventArgs e);

        /// <summary>
        /// 会话断开后触发
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        [AsyncRaiser]
        void OnDisconnected(ITcpClientBase client, DisconnectEventArgs e);

        /// <summary>
        /// 会话断开后触发
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnDisconnectedAsync(ITcpClientBase client, DisconnectEventArgs e);

        /// <summary>
        /// 即将断开连接(仅主动断开时有效)。
        /// <para>
        /// 当主动调用Close断开时，可通过<see cref="TouchSocketEventArgs.IsPermitOperation"/>终止断开行为。
        /// </para>
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        [AsyncRaiser]
        void OnDisconnecting(ITcpClientBase client, DisconnectEventArgs e);

        /// <summary>
        /// 即将断开连接(仅主动断开时有效)。
        /// <para>
        /// 当主动调用Close断开时，可通过<see cref="TouchSocketEventArgs.IsPermitOperation"/>终止断开行为。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnDisconnectingAsync(ITcpClientBase client, DisconnectEventArgs e);

        /// <summary>
        /// 当Client的ID被更改后触发
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnIDChanged(ITcpClientBase client, IDChangedEventArgs e);

        /// <summary>
        /// 当Client的ID被更改后触发
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnIDChangedAsync(ITcpClientBase client, IDChangedEventArgs e);

        /// <summary>
        /// 在收到数据时触发
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        [AsyncRaiser]
        void OnReceivedData(ITcpClientBase client, ReceivedDataEventArgs e);

        /// <summary>
        /// 在收到数据时触发
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnReceivedDataAsync(ITcpClientBase client, ReceivedDataEventArgs e);

        /// <summary>
        /// 在刚收到数据时触发，即在适配器之前。
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        [AsyncRaiser]
        void OnReceivingData(ITcpClientBase client, ByteBlockEventArgs e);

        /// <summary>
        /// 在刚收到数据时触发，即在适配器之前。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnReceivingDataAsync(ITcpClientBase client, ByteBlockEventArgs e);

        /// <summary>
        /// 当即将发送数据时，调用该方法在适配器之后，接下来即会发送数据。
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        [AsyncRaiser]
        void OnSendingData(ITcpClientBase client, SendingEventArgs e);

        /// <summary>
        /// 当即将发送数据时，调用该方法在适配器之后，接下来即会发送数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnSendingDataAsync(ITcpClientBase client, SendingEventArgs e);
    }
}