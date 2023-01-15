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
    /// 检查清理连接插件。服务器适用。
    /// </summary>
    [SingletonPlugin]
    public class CheckClearPlugin : TcpPluginBase
    {
        private ITcpService m_service;
        private Thread m_thread;

        /// <summary>
        /// 清理统计类型。默认为：<see cref="CheckClearType.All"/>。当设置为<see cref="CheckClearType.OnlySend"/>时，
        /// 则只检验发送方向是否有数据流动。没有的话则会断开连接。
        /// </summary>
        public CheckClearType CheckClearType { get; set; } = CheckClearType.All;

        /// <summary>
        /// 获取或设置清理无数据交互的Client，默认60秒。
        /// </summary>
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// 清理统计类型。默认为：<see cref="CheckClearType.All"/>。当设置为<see cref="CheckClearType.OnlySend"/>时，
        /// 则只检验发送方向是否有数据流动。没有的话则会断开连接。
        /// </summary>
        /// <param name="clearType"></param>
        /// <returns></returns>
        public CheckClearPlugin SetCheckClearType(CheckClearType clearType)
        {
            CheckClearType = clearType;
            return this;
        }

        /// <summary>
        /// 设置清理无数据交互的Client，默认60秒。
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public CheckClearPlugin SetDuration(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero || timeSpan == TimeSpan.MaxValue || timeSpan == TimeSpan.MinValue)
            {
                throw new InvalidTimeZoneException("设置的时间必须为有效值。");
            }
            Duration = timeSpan;
            return this;
        }

        /// <inheritdoc/>
        protected override void OnLoadedConfig(object sender, ConfigEventArgs e)
        {
            if (sender is ITcpService service)
            {
                m_service = service;
                m_thread = new Thread(ClearThread);
                m_thread.IsBackground = true;
                m_thread.Start();
            }
            base.OnLoadedConfig(sender, e);
        }

        private void ClearThread(object obj)
        {
            while (true)
            {
                if (m_service.ServerState == ServerState.Disposed || DisposedValue)
                {
                    return;
                }
                foreach (var client in m_service.SocketClients.GetClients())
                {
                    if (CheckClearType == CheckClearType.OnlyReceive)
                    {
                        if (DateTime.Now - client.LastReceivedTime > Duration)
                        {
                            client.SafeClose("超时无数据Receive交互，主动断开连接");
                        }
                    }
                    else if (CheckClearType == CheckClearType.OnlySend)
                    {
                        if (DateTime.Now - client.LastSendTime > Duration)
                        {
                            client.SafeClose("超时无数据Send交互，主动断开连接");
                        }
                    }
                    else
                    {
                        if (DateTime.Now - client.GetLastActiveTime() > Duration)
                        {
                            client.SafeClose("超时无数据交互，主动断开连接");
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}