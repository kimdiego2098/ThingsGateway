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



namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// TouchRpcHeartbeatPlugin
    /// </summary>
    [SingletonPlugin]
    public class TouchRpcHeartbeatPlugin<TClient> : TouchRpcPluginBase<TClient> where TClient : IDependencyObject, ITcpClientBase, IDependencyTouchRpc
    {
        private TClient m_client;

        private Thread m_thread;

        /// <summary>
        /// 连接失败次数。当成功连接时，会重置为0。
        /// </summary>
        public int FailedCount { get; private set; }

        /// <summary>
        /// 心跳间隔。默认3秒。
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>
        /// 最大失败次数，默认3。
        /// </summary>
        public int MaxFailCount { get; set; } = 3;

        /// <summary>
        /// 心跳间隔。默认3秒。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TouchRpcHeartbeatPlugin<TClient> SetInterval(TimeSpan value)
        {
            if (value == TimeSpan.Zero || value == TimeSpan.MaxValue || value == TimeSpan.MinValue)
            {
                throw new InvalidTimeZoneException("设置的时间必须为有效值。");
            }
            Interval = value;
            return this;
        }

        /// <summary>
        /// 最大失败次数，默认3。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TouchRpcHeartbeatPlugin<TClient> SetMaxFailCount(int value)
        {
            MaxFailCount = value;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnHandshaked(TClient client, VerifyOptionEventArgs e)
        {
            m_client = client;
            if (m_thread == null)
            {
                m_thread = new Thread(ThreadHeartbeat)
                {
                    IsBackground = true,
                    Name = $"HeartbeatThread"
                };
                m_thread.Start();
            }
            base.OnHandshaked(client, e);
        }

        private void ThreadHeartbeat(object obj)
        {
            while (true)
            {
                if (DisposedValue)
                {
                    return;
                }
                if (m_client.IsHandshaked && (DateTime.Now.TimeOfDay - m_client.GetLastActiveTime().TimeOfDay > Interval))
                {
                    if (m_client.Ping())
                    {
                        FailedCount = 0;
                    }
                    else
                    {
                        FailedCount++;
                        if (FailedCount > MaxFailCount)
                        {
                            m_client.SafeClose("自动心跳失败次数达到最大，已断开连接。");
                        }
                    }
                }

                Thread.Sleep(Interval);
            }
        }
    }
}