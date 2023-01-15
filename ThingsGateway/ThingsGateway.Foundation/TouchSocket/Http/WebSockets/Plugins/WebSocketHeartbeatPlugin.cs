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



namespace ThingsGateway.Foundation.Http.WebSockets
{
    /// <summary>
    /// WebSocketHeartbeatPlugin
    /// </summary>
    [SingletonPlugin]
    public class WebSocketHeartbeatPlugin : WebSocketPluginBase
    {
        private readonly int m_timeTick;

        /// <summary>
        /// 初始化一个适用于WebSocket的心跳插件
        /// </summary>
        /// <param name="interval"></param>
        [DependencyInject(1000 * 5)]
        public WebSocketHeartbeatPlugin(int interval)
        {
            m_timeTick = interval;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnHandshaked(ITcpClientBase client, HttpContextEventArgs e)
        {
            if (client is HttpClientBase httpClientBase)
            {
                if (client.GetValue<Timer>(WebSocketExtensions.HeartbeatTimerProperty) is Timer timer)
                {
                    timer.Dispose();
                }
                client.SetValue(WebSocketExtensions.HeartbeatTimerProperty, new Timer((o) =>
                {
                    httpClientBase.Ping();
                }, null, 0, m_timeTick));
            }
            base.OnHandshaked(client, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnDisconnected(ITcpClientBase client, DisconnectEventArgs e)
        {
            base.OnDisconnected(client, e);
            if (client.GetValue<Timer>(WebSocketExtensions.HeartbeatTimerProperty) is Timer timer)
            {
                timer.Dispose();
                client.SetValue(WebSocketExtensions.HeartbeatTimerProperty, null);
            }
        }
    }
}