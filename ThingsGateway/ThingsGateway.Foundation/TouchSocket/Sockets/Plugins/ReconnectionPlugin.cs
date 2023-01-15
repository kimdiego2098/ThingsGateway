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
    /// 重连插件
    /// </summary>
    [SingletonPlugin]
    public sealed class ReconnectionPlugin<TClient> : TcpPluginBase where TClient : class, ITcpClient
    {
        private readonly Func<TClient, bool> m_tryCon;

        /// <summary>
        /// 初始化一个重连插件
        /// </summary>
        /// <param name="tryCon">无论如何，只要返回True，则结束本轮尝试</param>
        public ReconnectionPlugin(Func<TClient, bool> tryCon)
        {
            Order = int.MinValue;
            m_tryCon = tryCon;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnDisconnected(ITcpClientBase client, DisconnectEventArgs e)
        {
            base.OnDisconnected(client, e);

            if (client is ITcpClient tcpClient)
            {
                if (e.Manual)
                {
                    return;
                }
                EasyTask.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            if (m_tryCon.Invoke((TClient)tcpClient))
                            {
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                });
            }
        }
    }
}