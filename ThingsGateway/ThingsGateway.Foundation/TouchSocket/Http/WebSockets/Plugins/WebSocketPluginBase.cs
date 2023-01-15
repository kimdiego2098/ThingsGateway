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
    /// WS插件基类
    /// </summary>
    public class WebSocketPluginBase : WebSocketPluginBase<ITcpClientBase>
    {
    }

    /// <summary>
    /// WS插件基类
    /// </summary>
    public class WebSocketPluginBase<TClient> : HttpPluginBase<TClient>, IWebSocketPlugin
    {
        #region 虚函数

        /// <summary>
        /// 表示收到断开连接报文。如果对方直接断开连接，此方法则不会触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnClosing(ITcpClientBase client, MsgEventArgs e)
        {
        }

        /// <summary>
        /// 表示收到断开连接报文。如果对方直接断开连接，此方法则不会触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnClosingAsync(ITcpClientBase client, MsgEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// 处理WS数据帧。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnHandleWSDataFrame(TClient client, WSDataFrameEventArgs e)
        {
        }

        /// <summary>
        /// 处理WS数据帧。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnHandleWSDataFrameAsync(TClient client, WSDataFrameEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// 表示完成握手后。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnHandshaked(TClient client, HttpContextEventArgs e)
        {
        }

        /// <summary>
        /// 表示完成握手后。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnHandshakedAsync(TClient client, HttpContextEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// 表示在即将握手连接时。
        /// <para>在此处拒绝操作，则会返回403 Forbidden。</para>
        /// <para>也可以向<see cref="HttpContextEventArgs.Context"/>注入更多信息。</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnHandshaking(TClient client, HttpContextEventArgs e)
        {
        }

        /// <summary>
        /// 表示在即将握手连接时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnHandshakingAsync(TClient client, HttpContextEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        #endregion 虚函数

        void IWebSocketPlugin.OnClosing(ITcpClientBase client, MsgEventArgs e)
        {
            OnClosing(client, e);
        }

        Task IWebSocketPlugin.OnClosingAsync(ITcpClientBase client, MsgEventArgs e)
        {
            return OnClosingAsync(client, e);
        }

        void IWebSocketPlugin.OnHandleWSDataFrame(ITcpClientBase client, WSDataFrameEventArgs e)
        {
            OnHandleWSDataFrame((TClient)client, e);
        }

        Task IWebSocketPlugin.OnHandleWSDataFrameAsync(ITcpClientBase client, WSDataFrameEventArgs e)
        {
            return OnHandleWSDataFrameAsync((TClient)client, e);
        }

        void IWebSocketPlugin.OnHandshaked(ITcpClientBase client, HttpContextEventArgs e)
        {
            OnHandshaked((TClient)client, e);
        }

        Task IWebSocketPlugin.OnHandshakedAsync(ITcpClientBase client, HttpContextEventArgs e)
        {
            return OnHandshakedAsync((TClient)client, e);
        }

        void IWebSocketPlugin.OnHandshaking(ITcpClientBase client, HttpContextEventArgs e)
        {
            OnHandshaking((TClient)client, e);
        }

        Task IWebSocketPlugin.OnHandshakingAsync(ITcpClientBase client, HttpContextEventArgs e)
        {
            return OnHandshakingAsync((TClient)client, e);
        }
    }
}