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
using System.Collections.Generic;



namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 适用于Tcp客户端的连接工厂。
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    public class TcpClientFactory<TClient> : ClientFactory<TClient> where TClient : ITcpClient, new()
    {
        private readonly TClient m_mainClient = new TClient();

        private readonly SingleTimer m_singleTimer;

        private bool first = true;

        /// <summary>
        /// 适用于Tcp客户端的连接工厂。
        /// </summary>
        public TcpClientFactory()
        {
            m_singleTimer = new SingleTimer(1000, () =>
            {
                List<TClient> list = new List<TClient>();
                foreach (var item in this.CreatedClients)
                {
                    if (!this.IsAlive(item))
                    {
                        list.Add(item);
                    }
                }

                foreach (var item in list)
                {
                    this.DisposeClient(item);
                }

                if (IsAlive(this.MainClient))
                {
                    if (this.CreatedClients.Count < this.MinCount)
                    {
                        try
                        {
                            this.CreateTransferClient();
                        }
                        catch
                        {
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 连接超时设定
        /// </summary>
        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <inheritdoc/>
        public override TClient MainClient { get => m_mainClient; }

        /// <summary>
        /// 获取传输的客户端配置
        /// </summary>
        public Func<TouchSocketConfig> OnGetTransferConfig { get; set; }

        /// <inheritdoc/>
        public override Result CheckStatus(bool tryInit = true)
        {
            lock (this.m_singleTimer)
            {
                try
                {
                    if (!IsAlive(m_mainClient))
                    {
                        if (!tryInit)
                        {
                            return Result.UnknownFail;
                        }
                        if (first)
                        {
                            OnMainClientSetuping();
                            MainClient.Setup(this.MainConfig);
                            first = false;
                        }
                        MainClient.Close();
                        MainClient.Connect((int)this.ConnectTimeout.TotalMilliseconds);
                    }
                    return Result.Success;
                }
                catch (Exception ex)
                {
                    return new Result(ex);
                }
            }
        }

        /// <summary>
        /// 在主客户端加载配置之前
        /// </summary>
        protected virtual void OnMainClientSetuping()
        {

        }

        /// <inheritdoc/>
        public override void DisposeClient(TClient client)
        {
            client.TryShutdown();
            client.SafeDispose();
            this.CreatedClients.Remove(client);
        }

        /// <summary>
        /// 获取可以使用的客户端数量。
        /// <para>
        /// 注意：该值不一定是<see cref="ClientFactory{TClient}.FreeClients"/>的长度，当已创建数量小于设定的最大值时，也会累加未创建的值。
        /// </para>
        /// </summary>
        /// <returns></returns>
        public override int GetAvailableCount()
        {
            return Math.Max(0, this.MaxCount - this.CreatedClients.Count) + this.FreeClients.Count;
        }

        /// <summary>
        /// 获取一个空闲的连接对象，如果等待超出设定的时间，则会创建新的连接。
        /// </summary>
        /// <param name="waitTime">指定毫秒数</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        public TClient GetTransferClient(int waitTime)
        {
            return this.GetTransferClient(TimeSpan.FromMilliseconds(waitTime));
        }

        /// <summary>
        /// 获取一个空闲的连接对象，如果等待超出1秒的时间，则会创建新的连接。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        public TClient GetTransferClient()
        {
            return this.GetTransferClient(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// 获取一个空闲的连接对象，如果等待超出设定的时间，则会创建新的连接。
        /// </summary>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        public override TClient GetTransferClient(TimeSpan waitTime)
        {
            while (FreeClients.TryDequeue(out var client))
            {
                if (IsAlive(client))
                {
                    return client;
                }
                else
                {
                    DisposeClient(client);
                }
            }

            if (this.CreatedClients.Count > MaxCount)
            {
                if (SpinWait.SpinUntil(Wait, waitTime))
                {
                    return GetTransferClient(waitTime);
                }
            }

            var clientRes = CreateTransferClient();
            return clientRes;
        }

        /// <inheritdoc/>
        public override bool IsAlive(TClient client)
        {
            return client.Online;
        }

        /// <summary>
        /// 归还使用完的连接。
        /// <para>
        /// 首先内部会判定存活状态，如果不再活动状态，会直接调用<see cref="DisposeClient(TClient)"/>。
        /// 其次会计算是否可以进入缓存队列，如果队列数量超出<see cref="ClientFactory{TClient}.MaxCount"/>，也会直接调用<see cref="DisposeClient(TClient)"/>
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        public override void ReleaseTransferClient(TClient client)
        {
            if ((object)client == (object)MainClient)
            {
                return;
            }
            if (!IsAlive(client))
            {
                DisposeClient(client);
                return;
            }
            if (FreeClients.Count < MaxCount)
            {
                FreeClients.Enqueue(client);
            }
            else
            {
                DisposeClient(client);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.m_singleTimer.SafeDispose();
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        protected override TouchSocketConfig GetTransferConfig()
        {
            return OnGetTransferConfig?.Invoke();
        }

        private TClient CreateTransferClient()
        {
            TClient client = new TClient();
            client.Setup(this.GetTransferConfig());
            client.Connect((int)ConnectTimeout.TotalMilliseconds);
            this.CreatedClients.Add(client);
            return client;
        }

        private bool Wait()
        {
            if (FreeClients.Count > 0)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    ///  适用于基于<see cref="TcpClient"/>的连接工厂。
    /// </summary>
    public class TcpClientFactory : TcpClientFactory<TcpClient>
    {
    }
}