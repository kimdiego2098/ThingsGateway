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
using System.Net;
using System.Net.Sockets;
using System.Threading
.Tasks;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 简单UDP会话。
    /// </summary>
    public class UdpSession : UdpSessionBase
    {
        /// <summary>
        /// 当收到数据时
        /// </summary>
        public UdpReceivedEventHandler Received { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="byteBlock"></param>
        /// <param name="requestInfo"></param>
        protected override void HandleReceivedData(EndPoint remoteEndPoint, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            Received?.Invoke(remoteEndPoint, byteBlock, requestInfo);
        }
    }

    /// <summary>
    /// UDP基类服务器。
    /// </summary>
    public class UdpSessionBase : BaseSocket, IUdpSession, IPluginObject
    {
        private readonly ConcurrentList<SocketAsyncEventArgs> m_socketAsyncs;
        private TouchSocketConfig m_config;
        private UdpDataHandlingAdapter m_adapter;
        private NetworkMonitor m_monitor;
        private IPHost m_remoteIPHost;
        private ServerState m_serverState;
        private bool m_usePlugin;

        /// <summary>
        /// 构造函数
        /// </summary>
        public UdpSessionBase()
        {
            m_socketAsyncs = new ConcurrentList<SocketAsyncEventArgs>();
            Protocol = Protocol.UDP;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ReceiveBufferSize = BufferLength;
            socket.SendBufferSize = BufferLength;
            m_monitor = new NetworkMonitor(null, socket);
        }

        /// <summary>
        /// 处理未经过适配器的数据。返回值表示是否继续向下传递。
        /// </summary>
        public Func<ByteBlock, bool> OnHandleRawBuffer { get; set; }

        /// <summary>
        /// 处理经过适配器后的数据。返回值表示是否继续向下传递。
        /// </summary>
        public Func<ByteBlock, IRequestInfo, bool> OnHandleReceivedData { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool CanSend => m_serverState == ServerState.Running;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual bool CanSetDataHandlingAdapter => true;

        /// <summary>
        /// 获取配置
        /// </summary>
        public TouchSocketConfig Config => m_config;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IContainer Container => m_config?.Container;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime LastReceivedTime { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime LastSendTime { get; private set; }

        /// <summary>
        /// 数据处理适配器
        /// </summary>
        public UdpDataHandlingAdapter DataHandlingAdapter => m_adapter;

        /// <summary>
        /// 监听器
        /// </summary>
        public NetworkMonitor Monitor => m_monitor;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IPluginsManager PluginsManager => m_config?.PluginsManager;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual Protocol Protocol { get; set; }

        /// <summary>
        /// 默认远程节点
        /// </summary>
        public IPHost RemoteIPHost => m_remoteIPHost;

        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName => Config?.GetValue(TouchSocketConfigExtension.ServerNameProperty);

        /// <summary>
        /// 获取服务器状态
        /// </summary>
        public ServerState ServerState => m_serverState;

        /// <summary>
        /// 是否已启用插件
        /// </summary>
        public bool UsePlugin => m_usePlugin;

        /// <summary>
        /// 退出组播
        /// </summary>
        /// <param name="multicastAddr"></param>
        public void DropMulticastGroup(IPAddress multicastAddr)
        {
            if (DisposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (multicastAddr is null)
            {
                throw new ArgumentNullException(nameof(multicastAddr));
            }

            if (m_monitor.Socket.AddressFamily == AddressFamily.InterNetwork)
            {
                MulticastOption optionValue = new MulticastOption(multicastAddr);
                m_monitor.Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, optionValue);
            }
            else
            {
                IPv6MulticastOption optionValue2 = new IPv6MulticastOption(multicastAddr);
                m_monitor.Socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, optionValue2);
            }
        }

        /// <summary>
        /// 加入组播。
        /// <para>组播地址为 224.0.0.0 ~ 239.255.255.255，其中 224.0.0.0~224.255.255.255 不建议在用户程序中使用，因为它们一般都有特殊用途。</para>
        /// </summary>
        /// <param name="multicastAddr"></param>
        public void JoinMulticastGroup(IPAddress multicastAddr)
        {
            if (multicastAddr is null)
            {
                throw new ArgumentNullException(nameof(multicastAddr));
            }
            if (DisposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (m_monitor.Socket.AddressFamily == AddressFamily.InterNetwork)
            {
                MulticastOption optionValue = new MulticastOption(multicastAddr);
                m_monitor.Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, optionValue);
            }
            else
            {
                IPv6MulticastOption optionValue2 = new IPv6MulticastOption(multicastAddr);
                m_monitor.Socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, optionValue2);
            }
        }

        /// <summary>
        /// 设置数据处理适配器
        /// </summary>
        /// <param name="adapter"></param>
        public virtual void SetDataHandlingAdapter(UdpDataHandlingAdapter adapter)
        {
            if (!CanSetDataHandlingAdapter)
            {
                throw new Exception($"不允许自由调用{nameof(SetDataHandlingAdapter)}进行赋值。");
            }

            SetAdapter(adapter);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public IService Setup(TouchSocketConfig config)
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
            return this;
        }

        /// <summary>
        /// 通过端口配置
        /// </summary>
        /// <param name="port"></param>
        public IService Setup(int port)
        {
            TouchSocketConfig serverConfig = new TouchSocketConfig();
            serverConfig.SetBindIPHost(new IPHost(port));
            return Setup(serverConfig);
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public IService Start()
        {
            if (m_serverState == ServerState.Disposed)
            {
                throw new Exception("无法重新利用已释放对象");
            }

            switch (m_serverState)
            {
                case ServerState.None:
                    {
                        if (m_config.GetValue(TouchSocketConfigExtension.BindIPHostProperty) is IPHost iPHost)
                        {
                            BeginReceive(iPHost);
                        }

                        break;
                    }
                case ServerState.Running:
                    break;

                case ServerState.Stopped:
                    {
                        if (m_config.GetValue(TouchSocketConfigExtension.BindIPHostProperty) is IPHost iPHost)
                        {
                            BeginReceive(iPHost);
                        }
                        break;
                    }
                case ServerState.Disposed:
                    {
                        throw new Exception("无法再次利用已释放对象");
                    }
            }

            m_serverState = ServerState.Running;
            return this;
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public IService Stop()
        {
            if (m_monitor != null)
            {
                m_monitor.Socket.Dispose();
            }
            m_monitor = null;
            m_serverState = ServerState.Stopped;
            foreach (var item in m_socketAsyncs)
            {
                item.SafeDispose();
            }
            m_socketAsyncs.Clear();
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Stop();
            m_serverState = ServerState.Disposed;
            base.Dispose(disposing);
        }

        /// <summary>
        /// 处理已接收到的数据。
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="byteBlock">以二进制流形式传递</param>
        /// <param name="requestInfo">以解析的数据对象传递</param>
        protected virtual void HandleReceivedData(EndPoint remoteEndPoint, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
        }

        /// <summary>
        /// 当即将发送时，如果覆盖父类方法，则不会触发插件。
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="buffer">数据缓存区</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <returns>返回值表示是否允许发送</returns>
        protected virtual bool HandleSendingData(EndPoint endPoint, byte[] buffer, int offset, int length)
        {
            return true;
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
            Logger = Container.Resolve<ILog>();
            m_remoteIPHost = config.GetValue(TouchSocketConfigExtension.RemoteIPHostProperty);
            BufferLength = config.GetValue(TouchSocketConfigExtension.BufferLengthProperty);
            m_usePlugin = config.IsUsePlugin;

            if (CanSetDataHandlingAdapter)
            {
                SetDataHandlingAdapter(Config.GetValue(TouchSocketConfigExtension.UdpDataHandlingAdapterProperty).Invoke());
            }
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

        /// <summary>
        /// 设置适配器，该方法不会检验<see cref="CanSetDataHandlingAdapter"/>的值。
        /// </summary>
        /// <param name="adapter"></param>
        protected void SetAdapter(UdpDataHandlingAdapter adapter)
        {
            if (adapter is null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            if (adapter.m_owner != null)
            {
                throw new Exception("此适配器已被其他终端使用，请重新创建对象。");
            }

            if (Config != null)
            {
                if (Config.GetValue(TouchSocketConfigExtension.MaxPackageSizeProperty) is int v1)
                {
                    adapter.MaxPackageSize = v1;
                }
            }
            adapter.m_owner = this;
            adapter.ReceivedCallBack = PrivateHandleReceivedData;
            adapter.SendCallBack = DefaultSend;
            m_adapter = adapter;
        }

        private void BeginReceive(IPHost iPHost)
        {
            int threadCount = Config.GetValue(TouchSocketConfigExtension.ThreadCountProperty);
            threadCount = threadCount < 0 ? 1 : threadCount;
            Socket socket = new Socket(iPHost.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveBufferSize = BufferLength,
                SendBufferSize = BufferLength,
                EnableBroadcast = m_config.GetValue(TouchSocketConfigExtension.EnableBroadcastProperty)
            };
            if (m_config.GetValue(TouchSocketConfigExtension.ReuseAddressProperty))
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            }
            PreviewBind(socket);
            socket.Bind(iPHost.EndPoint);

            m_monitor = new NetworkMonitor(iPHost, socket);

            switch (m_config.GetValue(TouchSocketConfigExtension.ReceiveTypeProperty))
            {
                case ReceiveType.Auto:
                    {
#if NET45_OR_GREATER||NET5_0_OR_GREATER
                        for (int i = 0; i < threadCount; i++)
                        {
                            SocketAsyncEventArgs eventArg = new SocketAsyncEventArgs();
                            m_socketAsyncs.Add(eventArg);
                            eventArg.Completed += IO_Completed;
                            ByteBlock byteBlock = BytePool.GetByteBlock(BufferLength);
                            eventArg.UserToken = byteBlock;
                            eventArg.SetBuffer(byteBlock.Buffer, 0, byteBlock.Capacity);
                            eventArg.RemoteEndPoint = iPHost.EndPoint;
                            if (!socket.ReceiveFromAsync(eventArg))
                            {
                                ProcessReceive(socket, eventArg);
                            }
                        }
#else
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            for (int i = 0; i < threadCount; i++)
                            {
                                SocketAsyncEventArgs eventArg = new SocketAsyncEventArgs();
                                m_socketAsyncs.Add(eventArg);
                                eventArg.Completed += IO_Completed;
                                ByteBlock byteBlock = BytePool.GetByteBlock(BufferLength);
                                eventArg.UserToken = byteBlock;
                                eventArg.SetBuffer(byteBlock.Buffer, 0, byteBlock.Capacity);
                                eventArg.RemoteEndPoint = iPHost.EndPoint;
                                if (!socket.ReceiveFromAsync(eventArg))
                                {
                                    ProcessReceive(socket, eventArg);
                                }
                            }
                        }
                        else
                        {
                            Thread thread = new Thread(Received);
                            thread.IsBackground = true;
                            thread.Start();
                        }
#endif
                        break;
                    }
                default:
                    throw new Exception("UDP中只支持Auto模式");
            }
        }

        private void Received()
        {
            while (true)
            {
                ByteBlock byteBlock = new ByteBlock();
                try
                {
                    EndPoint endPoint = m_monitor.IPHost.EndPoint;
                    int r = m_monitor.Socket.ReceiveFrom(byteBlock.Buffer, ref endPoint);
                    byteBlock.SetLength(r);
                    HandleBuffer(endPoint, byteBlock);
                }
                catch (Exception ex)
                {
                    byteBlock.Dispose();
                    Logger.Log(LogType.Error, this, ex.Message, ex);
                    break;
                }
            }
        }

        private void HandleBuffer(EndPoint endPoint, ByteBlock byteBlock)
        {
            try
            {
                LastReceivedTime = DateTime.Now;
                if (OnHandleRawBuffer?.Invoke(byteBlock) == false)
                {
                    return;
                }
                if (DisposedValue)
                {
                    return;
                }
                if (m_adapter == null)
                {
                    Logger.Error(this, TouchSocketStatus.NullDataAdapter.GetDescription());
                    return;
                }
                m_adapter.ReceivedInput(endPoint, byteBlock);
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, this, "在处理数据时发生错误", ex);
            }
            finally
            {
                byteBlock.Dispose();
            }
        }

        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive((Socket)sender, e);
        }

        private void PrivateHandleReceivedData(EndPoint remoteEndPoint, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            if (OnHandleReceivedData?.Invoke(byteBlock, requestInfo) == false)
            {
                return;
            }

            if (m_usePlugin)
            {
                UdpReceivedDataEventArgs args = new UdpReceivedDataEventArgs(remoteEndPoint, byteBlock, requestInfo);
                PluginsManager.Raise<IUdpSessionPlugin>(nameof(IUdpSessionPlugin.OnReceivedData), this, args);
                if (args.Handled)
                {
                    return;
                }
            }
            HandleReceivedData(remoteEndPoint, byteBlock, requestInfo);
        }

        #region 向默认远程同步发送

        /// <summary>
        /// 向默认终结点发送
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public virtual void Send(byte[] buffer, int offset, int length)
        {
            if (m_remoteIPHost == null)
            {
                throw new Exception("默认终结点为空");
            }
            Send(m_remoteIPHost.EndPoint, buffer, offset, length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <exception cref="OverlengthException"></exception>
        /// <exception cref="Exception"></exception>
        public virtual void Send(IRequestInfo requestInfo)
        {
            if (DisposedValue)
            {
                return;
            }
            if (m_adapter == null)
            {
                throw new ArgumentNullException(nameof(DataHandlingAdapter), TouchSocketStatus.NullDataAdapter.GetDescription());
            }
            if (!m_adapter.CanSendRequestInfo)
            {
                throw new NotSupportedException($"当前适配器不支持对象发送。");
            }
            m_adapter.SendInput(requestInfo);
        }

        #endregion 向默认远程同步发送

        #region 向默认远程异步发送

        /// <summary>
        /// IOCP发送
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="NotConnectedException"></exception>
        /// <exception cref="OverlengthException"></exception>
        /// <exception cref="Exception"></exception>
        public virtual Task SendAsync(byte[] buffer, int offset, int length)
        {
            return EasyTask.Run(() =>
              {
                  Send(buffer, offset, length);
              });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <exception cref="OverlengthException"></exception>
        /// <exception cref="Exception"></exception>
        public virtual Task SendAsync(IRequestInfo requestInfo)
        {
            return EasyTask.Run(() =>
            {
                Send(requestInfo);
            });
        }

        #endregion 向默认远程异步发送

        #region 向设置的远程同步发送

        /// <summary>
        /// 向设置的远程同步发送
        /// </summary>
        /// <param name="remoteEP"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="NotConnectedException"></exception>
        /// <exception cref="OverlengthException"></exception>
        /// <exception cref="Exception"></exception>
        public virtual void Send(EndPoint remoteEP, byte[] buffer, int offset, int length)
        {
            m_adapter.SendInput(remoteEP, buffer, offset, length);
        }

        #endregion 向设置的远程同步发送

        #region 向设置的远程异步发送

        /// <summary>
        /// 向设置的远程异步发送
        /// </summary>
        /// <param name="remoteEP"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="NotConnectedException"></exception>
        /// <exception cref="OverlengthException"></exception>
        /// <exception cref="Exception"></exception>
        public virtual Task SendAsync(EndPoint remoteEP, byte[] buffer, int offset, int length)
        {
            return EasyTask.Run(() =>
             {
                 Send(remoteEP, buffer, offset, length);
             });
        }

        #endregion 向设置的远程异步发送

        private void ProcessReceive(Socket socket, SocketAsyncEventArgs e)
        {
            if (m_serverState == ServerState.Running && e.SocketError == SocketError.Success)
            {
                ByteBlock byteBlock = (ByteBlock)e.UserToken;
                byteBlock.SetLength(e.BytesTransferred);

                HandleBuffer(e.RemoteEndPoint, byteBlock);

                ByteBlock newByteBlock = BytePool.GetByteBlock(BufferLength);
                e.UserToken = newByteBlock;
                e.SetBuffer(newByteBlock.Buffer, 0, newByteBlock.Buffer.Length);

                try
                {
                    if (!socket.ReceiveFromAsync(e))
                    {
                        ProcessReceive(socket, e);
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Log(LogType.Error, this, ex.Message, ex);
                }
            }
        }

        #region DefaultSend

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void DefaultSend(byte[] buffer, int offset, int length)
        {
            DefaultSend(m_remoteIPHost.EndPoint, buffer, offset, length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void DefaultSend(EndPoint endPoint, byte[] buffer, int offset, int length)
        {
            if (HandleSendingData(endPoint, buffer, offset, length))
            {
                if (CanSend)
                {
                    m_monitor.Socket.SendTo(buffer, offset, length, SocketFlags.None, endPoint);
                }

                LastSendTime = DateTime.Now;
            }
        }

        #endregion DefaultSend

        #region DefaultSendAsync

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public Task DefaultSendAsync(byte[] buffer, int offset, int length)
        {
            return EasyTask.Run(() =>
            {
                DefaultSend(buffer, offset, length);
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public Task DefaultSendAsync(EndPoint endPoint, byte[] buffer, int offset, int length)
        {
            return EasyTask.Run(() =>
            {
                DefaultSend(buffer, offset, length);
            });
        }

        #endregion DefaultSendAsync

        #region 组合发送

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="transferBytes"></param>
        public void Send(IList<ArraySegment<byte>> transferBytes)
        {
            Send(m_remoteIPHost.EndPoint, transferBytes);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="transferBytes"></param>
        public void Send(EndPoint endPoint, IList<ArraySegment<byte>> transferBytes)
        {
            if (m_adapter == null)
            {
                throw new ArgumentNullException(nameof(DataHandlingAdapter), TouchSocketStatus.NullDataAdapter.GetDescription());
            }

            if (!m_adapter.CanSplicingSend)
            {
                throw new NotSupportedException("该适配器不支持拼接发送");
            }
            m_adapter.SendInput(endPoint, transferBytes);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="transferBytes"></param>
        public Task SendAsync(IList<ArraySegment<byte>> transferBytes)
        {
            return EasyTask.Run(() =>
            {
                Send(transferBytes);
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="transferBytes"></param>
        public Task SendAsync(EndPoint endPoint, IList<ArraySegment<byte>> transferBytes)
        {
            return EasyTask.Run(() =>
            {
                Send(endPoint, transferBytes);
            });
        }

        #endregion 组合发送
    }
}