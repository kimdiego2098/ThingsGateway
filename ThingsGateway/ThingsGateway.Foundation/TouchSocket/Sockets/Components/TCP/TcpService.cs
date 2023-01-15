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
using System.Linq;
using System.Net.Sockets;



namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// TCP泛型服务器，由客户自己指定<see cref="SocketClient"/>类型。
    /// </summary>
    public class TcpService<TClient> : TcpServiceBase, ITcpService<TClient> where TClient : SocketClient
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TcpService()
        {
            m_socketClients = new SocketClientCollection();
            m_getDefaultNewID = () =>
            {
                return Interlocked.Increment(ref m_nextId).ToString();
            };
        }

        #region 变量

        private readonly SocketClientCollection m_socketClients;
        private int m_backlog;
        private TouchSocketConfig m_config;
        private Func<string> m_getDefaultNewID;
        private int m_maxCount;
        private NetworkMonitor[] m_monitors;
        private long m_nextId;
        private ReceiveType m_receiveType;
        private ServerState m_serverState;
        private bool m_usePlugin;
        private bool m_useSsl;

        #endregion 变量

        #region 属性

        /// <summary>
        /// 获取服务器配置
        /// </summary>
        public override TouchSocketConfig Config => m_config;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IContainer Container => Config?.Container;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Func<string> GetDefaultNewID => m_getDefaultNewID;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override int MaxCount => m_maxCount;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override NetworkMonitor[] Monitors => m_monitors;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IPluginsManager PluginsManager => Config?.PluginsManager;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string ServerName => Config?.GetValue<string>(TouchSocketConfigExtension.ServerNameProperty);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override ServerState ServerState => m_serverState;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override SocketClientCollection SocketClients => m_socketClients;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool UsePlugin => m_usePlugin;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool UseSsl => m_useSsl;

        #endregion 属性

        #region 事件

        /// <summary>
        /// 用户连接完成
        /// </summary>
        public TouchSocketEventHandler<TClient> Connected { get; set; }

        /// <summary>
        /// 有用户连接的时候
        /// </summary>
        public OperationEventHandler<TClient> Connecting { get; set; }

        /// <summary>
        /// 有用户断开连接
        /// </summary>
        public DisconnectEventHandler<TClient> Disconnected { get; set; }

        /// <summary>
        /// 即将断开连接(仅主动断开时有效)。
        /// <para>
        /// 当主动调用Close断开时，可通过<see cref="TouchSocketEventArgs.IsPermitOperation"/>终止断开行为。
        /// </para>
        /// </summary>
        public DisconnectEventHandler<TClient> Disconnecting { get; set; }

        /// <summary>
        /// 当客户端ID被修改时触发。
        /// </summary>
        public IDChangedEventHandler<TClient> IDChanged { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="e"></param>
        protected override sealed void OnClientConnected(ISocketClient socketClient, TouchSocketEventArgs e)
        {
            OnConnected((TClient)socketClient, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="e"></param>
        protected override sealed void OnClientConnecting(ISocketClient socketClient, OperationEventArgs e)
        {
            OnConnecting((TClient)socketClient, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="e"></param>
        protected override sealed void OnClientDisconnected(ISocketClient socketClient, DisconnectEventArgs e)
        {
            OnDisconnected((TClient)socketClient, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="e"></param>
        protected override sealed void OnClientDisconnecting(ISocketClient socketClient, DisconnectEventArgs e)
        {
            OnDisconnecting((TClient)socketClient, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="byteBlock"></param>
        /// <param name="requestInfo"></param>
        protected override sealed void OnClientReceivedData(ISocketClient socketClient, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            OnReceived((TClient)socketClient, byteBlock, requestInfo);
        }

        /// <summary>
        /// 客户端连接完成，覆盖父类方法将不会触发事件。
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="e"></param>
        protected virtual void OnConnected(TClient socketClient, TouchSocketEventArgs e)
        {
            Connected?.Invoke(socketClient, e);
        }

        /// <summary>
        /// 客户端请求连接，覆盖父类方法将不会触发事件。
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="e"></param>
        protected virtual void OnConnecting(TClient socketClient, OperationEventArgs e)
        {
            Connecting?.Invoke(socketClient, e);
        }

        /// <summary>
        /// 客户端断开连接，覆盖父类方法将不会触发事件。
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="e"></param>
        protected virtual void OnDisconnected(TClient socketClient, DisconnectEventArgs e)
        {
            Disconnected?.Invoke(socketClient, e);
        }

        /// <summary>
        /// 即将断开连接(仅主动断开时有效)。
        /// <para>
        /// 当主动调用Close断开时，可通过<see cref="TouchSocketEventArgs.IsPermitOperation"/>终止断开行为。
        /// </para>
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="e"></param>
        protected virtual void OnDisconnecting(TClient socketClient, DisconnectEventArgs e)
        {
            Disconnecting?.Invoke(socketClient, e);
        }

        /// <summary>
        /// 当收到适配器数据。
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="byteBlock"></param>
        /// <param name="requestInfo"></param>
        protected virtual void OnReceived(TClient socketClient, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
        }

        #endregion 事件

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Clear()
        {
            string[] ids = GetIDs();
            foreach (var item in ids)
            {
                if (TryGetSocketClient(item, out TClient client))
                {
                    client.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取当前在线的所有客户端
        /// </summary>
        /// <returns></returns>
        public TClient[] GetClients()
        {
            var clients = m_socketClients.GetClients().ToArray();

            TClient[] clients1 = new TClient[clients.Length];
            for (int i = 0; i < clients.Length; i++)
            {
                clients1[i] = (TClient)clients[i];
            }
            return clients1;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="oldID"></param>
        /// <param name="newID"></param>
        /// <exception cref="ClientNotFindException"></exception>
        /// <exception cref="Exception"></exception>
        public override void ResetID(string oldID, string newID)
        {
            if (string.IsNullOrEmpty(oldID))
            {
                throw new ArgumentException($"“{nameof(oldID)}”不能为 null 或空。", nameof(oldID));
            }

            if (string.IsNullOrEmpty(newID))
            {
                throw new ArgumentException($"“{nameof(newID)}”不能为 null 或空。", nameof(newID));
            }

            if (oldID == newID)
            {
                return;
            }
            if (m_socketClients.TryGetSocketClient(oldID, out TClient socketClient))
            {
                socketClient.ResetID(newID);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(oldID));
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="config"></param>
        public override IService Setup(TouchSocketConfig config)
        {
            m_config = config;
            if (config.IsUsePlugin)
            {
                PluginsManager.Raise<IConfigPlugin>(nameof(IConfigPlugin.OnLoadingConfig), this, new ConfigEventArgs(config));
            }
            LoadConfig(m_config);
            if (UsePlugin)
            {
                PluginsManager.Raise<IConfigPlugin>(nameof(IConfigPlugin.OnLoadedConfig), this, new ConfigEventArgs(config));
            }
            Container.RegisterTransient<TClient>();
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="port"></param>
        public override IService Setup(int port)
        {
            TouchSocketConfig serviceConfig = new TouchSocketConfig();
            serviceConfig.SetListenIPHosts(new IPHost[] { new IPHost(port) });
            return Setup(serviceConfig);
        }

        /// <summary>
        ///<inheritdoc/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool SocketClientExist(string id)
        {
            return SocketClients.SocketClientExist(id);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public override IService Start()
        {
            IPHost[] iPHosts = Config.GetValue<IPHost[]>(TouchSocketConfigExtension.ListenIPHostsProperty);
            if (iPHosts == null || iPHosts.Length == 0)
            {
                throw new Exception("IPHosts为空，无法绑定");
            }
            switch (m_serverState)
            {
                case ServerState.None:
                    {
                        BeginListen(iPHosts);
                        break;
                    }
                case ServerState.Running:
                    {
                        return this;
                    }
                case ServerState.Stopped:
                    {
                        BeginListen(iPHosts);
                        break;
                    }
                case ServerState.Disposed:
                    {
                        throw new Exception("无法重新利用已释放对象");
                    }
            }
            m_serverState = ServerState.Running;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IService Stop()
        {
            if (m_monitors != null)
            {
                foreach (var item in m_monitors)
                {
                    if (item.Socket != null)
                    {
                        item.Socket.Dispose();
                    }
                }
            }
            m_monitors = null;

            Clear();

            m_serverState = ServerState.Stopped;

            return this;
        }

        /// <summary>
        /// 尝试获取TClient
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="socketClient">TClient</param>
        /// <returns></returns>
        public bool TryGetSocketClient(string id, out TClient socketClient)
        {
            return m_socketClients.TryGetSocketClient(id, out socketClient);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    if (m_monitors != null)
                    {
                        foreach (var item in m_monitors)
                        {
                            if (item.Socket != null)
                            {
                                item.Socket.Dispose();
                            }
                        }
                        m_monitors = null;
                    }
                    Clear();
                    m_serverState = ServerState.Disposed;
                    PluginsManager.Clear();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// 初始化客户端实例，默认实现为<see cref="Container.Resolve(Type, object[], string)"/>
        /// </summary>
        /// <returns></returns>
        protected virtual TClient GetClientInstence()
        {
            return Container.Resolve<TClient>();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="config"></param>
        protected virtual void LoadConfig(TouchSocketConfig config)
        {
            if (config == null)
            {
                throw new Exception("配置文件为空");
            }
            if (config.GetValue(TouchSocketConfigExtension.GetDefaultNewIDProperty) is Func<string> fun)
            {
                m_getDefaultNewID = fun;
            }
            m_usePlugin = config.IsUsePlugin;
            m_maxCount = config.GetValue(TouchSocketConfigExtension.MaxCountProperty);
            m_backlog = config.GetValue(TouchSocketConfigExtension.BacklogProperty);
            BufferLength = config.GetValue(TouchSocketConfigExtension.BufferLengthProperty);
            m_receiveType = config.GetValue(TouchSocketConfigExtension.ReceiveTypeProperty);

            Logger ??= Container.Resolve<ILog>();
            if (config.GetValue(TouchSocketConfigExtension.SslOptionProperty) != null)
            {
                m_useSsl = true;
            }
        }

        /// <summary>
        /// 在验证Ssl发送错误时。
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void OnAuthenticatingError(System.Exception ex)
        {
        }

        /// <summary>
        /// 在Socket初始化对象后，Bind之前调用。
        /// 可用于设置Socket参数。
        /// 父类方法可覆盖。
        /// </summary>
        /// <param name="socket"></param>
        protected virtual void PreviewBind(Socket socket)
        {
        }

        private void Args_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Accept)
            {
                OnAccepted(e);
            }
        }

        private void BeginListen(IPHost[] iPHosts)
        {
            int threadCount = Config.GetValue(TouchSocketConfigExtension.ThreadCountProperty);
            if (threadCount > 0)
            {
                while (!ThreadPool.SetMinThreads(threadCount, threadCount))
                {
                    if (--threadCount <= 10)
                    {
                        break;
                    }
                }
            }

            List<NetworkMonitor> networkMonitors = new List<NetworkMonitor>();
            foreach (var iPHost in iPHosts)
            {
                Socket socket = new Socket(iPHost.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveBufferSize = BufferLength,
                    SendBufferSize = BufferLength
                };
                if (m_config.GetValue(TouchSocketConfigExtension.ReuseAddressProperty))
                {
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                }
                PreviewBind(socket);
                socket.Bind(iPHost.EndPoint);
                socket.Listen(m_backlog);

                networkMonitors.Add(new NetworkMonitor(iPHost, socket));
            }

            m_monitors = networkMonitors.ToArray();
            foreach (var networkMonitor in m_monitors)
            {
                SocketAsyncEventArgs e = new SocketAsyncEventArgs
                {
                    UserToken = networkMonitor.Socket
                };
                e.Completed += Args_Completed;
                if (!networkMonitor.Socket.AcceptAsync(e))
                {
                    OnAccepted(e);
                }
            }
        }

        private void OnAccepted(SocketAsyncEventArgs e)
        {
            if (!DisposedValue)
            {
                if (e.SocketError == SocketError.Success && e.AcceptSocket != null)
                {
                    Socket socket = e.AcceptSocket;
                    socket.ReceiveBufferSize = BufferLength;
                    socket.SendBufferSize = BufferLength;
                    if (SocketClients.Count < m_maxCount)
                    {
                        SocketInit(socket);
                    }
                    else
                    {
                        socket.SafeDispose();
                        Logger.Warning(this, "连接客户端数量已达到设定最大值");
                    }
                }
                e.AcceptSocket = null;

                try
                {
                    if (!((Socket)e.UserToken).AcceptAsync(e))
                    {
                        OnAccepted(e);
                    }
                }
                catch
                {
                    e.Dispose();
                    return;
                }
            }
        }

        private void SetClientConfiguration(SocketClient client)
        {
            client.m_usePlugin = m_usePlugin;
            client.Config = m_config;
            client.m_service = this;
            client.Logger = Container.Resolve<ILog>();
            client.m_receiveType = m_receiveType;
            client.BufferLength = BufferLength;
            if (client.CanSetDataHandlingAdapter)
            {
                client.SetDataHandlingAdapter(Config.GetValue(TouchSocketConfigExtension.DataHandlingAdapterProperty).Invoke());
            }
        }

        private void SocketInit(Socket socket)
        {
            try
            {
                if (Config.GetValue(TouchSocketConfigExtension.NoDelayProperty))
                {
                    socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                }
                TClient client = GetClientInstence();
                SetClientConfiguration(client);
                client.SetSocket(socket);
                client.InternalInitialized();

                OperationEventArgs args = new OperationEventArgs
                {
                    ID = GetDefaultNewID()
                };
                client.InternalConnecting(args);//Connecting
                if (args.IsPermitOperation)
                {
                    client.m_id = args.ID;
                    if (SocketClients.TryAdd(client))
                    {
                        client.InternalConnected(new MsgEventArgs("客户端成功连接"));
                        if (!client.MainSocket.Connected)
                        {
                            return;
                        }
                        if (m_useSsl)
                        {
                            try
                            {
                                client.BeginReceiveSsl(m_receiveType, (ServiceSslOption)m_config.GetValue(TouchSocketConfigExtension.SslOptionProperty));
                            }
                            catch (System.Exception ex)
                            {
                                OnAuthenticatingError(ex);
                                return;
                            }
                        }
                        else
                        {
                            client.BeginReceive(m_receiveType);
                        }
                    }
                    else
                    {
                        throw new Exception($"ID={client.m_id}重复");
                    }
                }
                else
                {
                    socket.SafeDispose();
                }
            }
            catch (Exception ex)
            {
                socket.SafeDispose();
                Logger.Log(LogType.Error, this, "接收新连接错误", ex);
            }
        }
    }

    /// <summary>
    /// TCP服务器
    /// </summary>
    public class TcpService : TcpService<SocketClient>
    {
        /// <summary>
        /// 处理数据
        /// </summary>
        public ReceivedEventHandler<SocketClient> Received { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="byteBlock"></param>
        /// <param name="requestInfo"></param>
        protected override void OnReceived(SocketClient socketClient, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            Received?.Invoke(socketClient, byteBlock, requestInfo);
            base.OnReceived(socketClient, byteBlock, requestInfo);
        }
    }
}