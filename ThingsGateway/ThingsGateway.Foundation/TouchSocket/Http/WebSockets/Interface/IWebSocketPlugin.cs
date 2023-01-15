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

namespace ThingsGateway.Foundation.Http.WebSockets
{
    /// <summary>
    /// WebSocket插件
    /// </summary>
    public interface IWebSocketPlugin : IPlugin
    {
        /// <summary>
        /// 表示收到断开连接报文。如果对方直接断开连接，此方法则不会触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnClosing(ITcpClientBase client, MsgEventArgs e);

        /// <summary>
        /// 表示收到断开连接报文。如果对方直接断开连接，此方法则不会触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        Task OnClosingAsync(ITcpClientBase client, MsgEventArgs e);

        /// <summary>
        /// 当收到WS数据时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnHandleWSDataFrame(ITcpClientBase client, WSDataFrameEventArgs e);

        /// <summary>
        /// 当收到WS数据时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnHandleWSDataFrameAsync(ITcpClientBase client, WSDataFrameEventArgs e);

        /// <summary>
        /// 表示完成握手后。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnHandshaked(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 表示完成握手后。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnHandshakedAsync(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 表示在即将握手连接时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnHandshaking(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 表示在即将握手连接时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnHandshakingAsync(ITcpClientBase client, HttpContextEventArgs e);
    }
}