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
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 简单TCP客户端
    /// </summary>
    public class TcpClient : TcpClientBase
    {
        /// <summary>
        /// 接收到数据
        /// </summary>
        public ReceivedEventHandler<TcpClient> Received { get; set; }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="byteBlock"></param>
        /// <param name="requestInfo"></param>
        protected override void HandleReceivedData(ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            Received?.Invoke(this, byteBlock, requestInfo);
            base.HandleReceivedData(byteBlock, requestInfo);
        }
    }

    /// <summary>
    /// TCP客户端
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{IP}:{Port}")]
    public class TcpClientBase : BaseSocket, ITcpClient
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TcpClientBase()
        {
            Protocol = Protocol.TCP;
        }

        #region 变量

        private DelaySender m_delaySender;
        private TouchSocketConfig m_config;
        private DataHandlingAdapter m_adapter;
        private Socket m_mainSocket;
        private bool m_online;
        private ReceiveType m_receiveType;
        private bool m_useDelaySender;
        private bool m_usePlugin;
        private Stream m_workStream;
        private IPHost m_remoteIPHost;

        #endregion 变量

        #region 事件

        /// <summary>
        /// 成功连接到服务器
        /// </summary>
        public MessageEventHandler<ITcpClient> Connected { get; set; }

        /// <summary>
        /// 准备连接的时候，此时已初始化Socket，但是并未建立Tcp连接
        /// </summary>
        public ConnectingEventHandler<ITcpClient> Connecting { get; set; }

        /// <summary>
        /// 断开连接。在客户端未设置连接状态时，不会触发
        /// </summary>
        public DisconnectEventHandler<ITcpClientBase> Disconnected { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DisconnectEventHandler<ITcpClientBase> Disconnecting { get; set; }

        private void PrivateOnConnected(MsgEventArgs e)
        {
            if (m_usePlugin)
            {
                PluginsManager.Raise<ITcpPlugin>(nameof(ITcpPlugin.OnConnected), this, e);
                if (e.Handled)
                {
                    return;
                }
            }
            OnConnected(e);
        }

        /// <summary>
        /// 已经建立Tcp连接
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnConnected(MsgEventArgs e)
        {
            try
            {
                Connected?.Invoke(this, e);
            }
            catch (System.Exception ex)
            {
                Logger.Log(LogType.Error, this, $"在事件{nameof(Connected)}中发生错误。", ex);
            }
        }

        private void PrivateOnConnecting(ConnectingEventArgs e)
        {
            LastReceivedTime = DateTime.Now;
            LastSendTime = DateTime.Now;
            if (CanSetDataHandlingAdapter)
            {
                SetDataHandlingAdapter(Config.GetValue<Func<DataHandlingAdapter>>(TouchSocketConfigExtension.DataHandlingAdapterProperty).Invoke());
            }
            if (m_usePlugin)
            {
                PluginsManager.Raise<ITcpPlugin>(nameof(ITcpPlugin.OnConnecting), this, e);
                if (e.Handled)
                {
                    return;
                }
            }
            OnConnecting(e);
        }

        /// <summary>
        /// 准备连接的时候，此时已初始化Socket，但是并未建立Tcp连接
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnConnecting(ConnectingEventArgs e)
        {
            try
            {
                Connecting?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, this, $"在事件{nameof(this.OnConnecting)}中发生错误。", ex);
            }
        }

        private void PrivateOnDisconnected(DisconnectEventArgs e)
        {
            if (m_usePlugin)
            {
                PluginsManager.Raise<ITcpPlugin>(nameof(ITcpPlugin.OnDisconnected), this, e);
                if (e.Handled)
                {
                    return;
                }
            }
            OnDisconnected(e);
        }

        /// <summary>
        /// 断开连接。在客户端未设置连接状态时，不会触发
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDisconnected(DisconnectEventArgs e)
        {
            try
            {
                Disconnected?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, this, $"在事件{nameof(Disconnected)}中发生错误。", ex);
            }
        }

        private void PrivateOnDisconnecting(DisconnectEventArgs e)
        {
            if (m_usePlugin)
            {
                PluginsManager.Raise<ITcpPlugin>(nameof(ITcpPlugin.OnDisconnecting), this, e);
                if (e.Handled)
                {
                    return;
                }
            }
            OnDisconnecting(e);
        }

        /// <summary>
        /// 即将断开连接(仅主动断开时有效)。
        /// <para>
        /// 当主动调用Close断开时，可通过<see cref="TouchSocketEventArgs.IsPermitOperation"/>终止断开行为。
        /// </para>
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDisconnecting(DisconnectEventArgs e)
        {
            try
            {
                Disconnecting?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, this, $"在事件{nameof(Disconnecting)}中发生错误。", ex);
            }
        }

        #endregion 事件

        #region 属性

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime LastReceivedTime { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime LastSendTime { get; private set; }

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
        public IContainer Container => m_config?.Container;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual bool CanSetDataHandlingAdapter => true;

        /// <summary>
        /// 客户端配置
        /// </summary>
        public TouchSocketConfig Config => m_config;

        /// <summary>
        /// 数据处理适配器
        /// </summary>
        public DataHandlingAdapter DataHandlingAdapter => m_adapter;

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; private set; }

        /// <summary>
        /// 主通信器
        /// </summary>
        public Socket MainSocket => m_mainSocket;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool CanSend => m_online;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool Online => m_online;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IPluginsManager PluginsManager => m_config?.PluginsManager;

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReceiveType ReceiveType => m_receiveType;

        /// <summary>
        /// 是否已启用插件
        /// </summary>
        public bool UsePlugin => m_usePlugin;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool UseSsl { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Protocol Protocol { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IPHost RemoteIPHost => m_remoteIPHost;

        /// <inheritdoc/>
        public bool IsClient => true;

        #endregion 属性

        #region 断开操作

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Close()
        {
            Close($"{nameof(Close)}主动断开");
        }

        /// <summary>
        /// 中断终端，传递中断消息。
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Close(string msg)
        {
            if (this.m_online)
            {
                var args = new DisconnectEventArgs(true, msg)
                {
                    IsPermitOperation = true
                };
                PrivateOnDisconnecting(args);
                if (this.DisposedValue || args.IsPermitOperation)
                {
                    BreakOut(msg, true);
                }
            }
        }

        private void BreakOut(string msg, bool manual)
        {
            lock (this.SyncRoot)
            {
                if (m_online)
                {
                    m_online = false;
                    this.TryShutdown();
                    m_mainSocket.SafeDispose();
                    m_delaySender.SafeDispose();
                    m_workStream.SafeDispose();
                    m_adapter.SafeDispose();
                    PrivateOnDisconnected(new DisconnectEventArgs(manual, msg));
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (this.m_online)
            {
                var args = new DisconnectEventArgs(true, $"{nameof(Dispose)}主动断开");
                PrivateOnDisconnecting(args);
            }
            PluginsManager.Clear();
            m_config = default;
            m_adapter.SafeDispose();
            m_adapter = default;
            BreakOut($"{nameof(Dispose)}主动断开", true);
            base.Dispose(disposing);
        }

        #endregion 断开操作

        /// <summary>
        /// 建立Tcp的连接。
        /// </summary>
        /// <param name="timeout"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="TimeoutException"></exception>
        protected void TcpConnect(int timeout)
        {
            lock (SyncRoot)
            {
                if (m_online)
                {
                    return;
                }
                if (DisposedValue)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                if (m_config == null)
                {
                    throw new ArgumentNullException("配置文件不能为空。");
                }
                IPHost iPHost = m_config.GetValue(TouchSocketConfigExtension.RemoteIPHostProperty);
                if (iPHost == null)
                {
                    throw new ArgumentNullException("iPHost不能为空。");
                }
                if (m_mainSocket != null)
                {
                    m_mainSocket.Dispose();
                }
                m_mainSocket = CreateSocket(iPHost);

                ConnectingEventArgs args = new ConnectingEventArgs(m_mainSocket);
                PrivateOnConnecting(args);
                if (timeout == 5000)
                {
                    m_mainSocket.Connect(iPHost.EndPoint);
                    m_online = true;
                    LoadSocketAndReadIpPort();

                    if (m_config.GetValue(TouchSocketConfigExtension.DelaySenderProperty) is DelaySenderOption senderOption)
                    {
                        m_useDelaySender = true;
                        m_delaySender.SafeDispose();
                        m_delaySender = new DelaySender(m_mainSocket, senderOption.QueueLength, OnDelaySenderError)
                        {
                            DelayLength = senderOption.DelayLength
                        };
                    }

                    BeginReceive();
                    PrivateOnConnected(new MsgEventArgs("连接成功"));
                }
                else
                {
                    var result = m_mainSocket.BeginConnect(iPHost.EndPoint, null, null);
                    if (result.AsyncWaitHandle.WaitOne(timeout))
                    {
                        if (m_mainSocket.Connected)
                        {
                            m_mainSocket.EndConnect(result);
                            m_online = true;
                            LoadSocketAndReadIpPort();

                            if (m_config.GetValue(TouchSocketConfigExtension.DelaySenderProperty) is DelaySenderOption senderOption)
                            {
                                m_useDelaySender = true;
                                m_delaySender.SafeDispose();
                                m_delaySender = new DelaySender(m_mainSocket, senderOption.QueueLength, OnDelaySenderError)
                                {
                                    DelayLength = senderOption.DelayLength
                                };
                            }

                            BeginReceive();
                            PrivateOnConnected(new MsgEventArgs("连接成功"));
                            return;
                        }
                        else
                        {
                            m_mainSocket.SafeDispose();
                            throw new Exception("异步已完成，但是socket并未在连接状态，可能发生了绑定端口占用的错误。");
                        }
                    }
                    m_mainSocket.SafeDispose();
                    throw new TimeoutException();
                }
            }
        }

        /// <summary>
        /// 请求连接到服务器。
        /// </summary>
        public virtual ITcpClient Connect(int timeout = 5000)
        {
            TcpConnect(timeout);
            return this;
        }

        /// <summary>
        /// 异步连接服务器
        /// </summary>
        public Task<ITcpClient> ConnectAsync(int timeout = 5000)
        {
            return EasyTask.Run(() =>
            {
                return Connect(timeout);
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            if (m_workStream == null)
            {
                m_workStream = new NetworkStream(m_mainSocket, true);
            }
            return m_workStream;
        }

        /// <summary>
        /// 设置数据处理适配器
        /// </summary>
        /// <param name="adapter"></param>
        public virtual void SetDataHandlingAdapter(DataHandlingAdapter adapter)
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
        /// <param name="ipHost"></param>
        /// <returns></returns>
        public ITcpClient Setup(string ipHost)
        {
            TouchSocketConfig config = new TouchSocketConfig();
            config.SetRemoteIPHost(new IPHost(ipHost));
            return Setup(config);
        }

        /// <summary>
        /// 配置服务器
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="Exception"></exception>
        public ITcpClient Setup(TouchSocketConfig config)
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

        private void PrivateHandleReceivedData(ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            if (OnHandleReceivedData?.Invoke(byteBlock, requestInfo) == false)
            {
                return;
            }
            if (m_usePlugin)
            {
                ReceivedDataEventArgs args = new ReceivedDataEventArgs(byteBlock, requestInfo);
                PluginsManager.Raise<ITcpPlugin>(nameof(ITcpPlugin.OnReceivedData), this, args);
                if (args.Handled)
                {
                    return;
                }
            }

            HandleReceivedData(byteBlock, requestInfo);
        }

        /// <summary>
        /// 处理已接收到的数据。
        /// </summary>
        /// <param name="byteBlock">以二进制流形式传递</param>
        /// <param name="requestInfo">以解析的数据对象传递</param>
        protected virtual void HandleReceivedData(ByteBlock byteBlock, IRequestInfo requestInfo)
        {
        }

        /// <summary>
        /// 当即将发送时，如果覆盖父类方法，则不会触发插件。
        /// </summary>
        /// <param name="buffer">数据缓存区</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <returns>返回值表示是否允许发送</returns>
        protected virtual bool HandleSendingData(byte[] buffer, int offset, int length)
        {
            if (m_usePlugin)
            {
                SendingEventArgs args = new SendingEventArgs(buffer, offset, length);
                PluginsManager.Raise<ITcpPlugin>(nameof(ITcpPlugin.OnSendingData), this, args);
                if (args.IsPermitOperation)
                {
                    return true;
                }
                return false;
            }
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
            m_remoteIPHost = config.GetValue(TouchSocketConfigExtension.RemoteIPHostProperty);
            BufferLength = config.GetValue(TouchSocketConfigExtension.BufferLengthProperty);
            m_receiveType = config.GetValue(TouchSocketConfigExtension.ReceiveTypeProperty);
            m_usePlugin = config.IsUsePlugin;
            Logger = Container.Resolve<ILog>();
            if (config.GetValue(TouchSocketConfigExtension.SslOptionProperty) != null)
            {
                UseSsl = true;
            }
        }

        /// <summary>
        /// 在延迟发生错误
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void OnDelaySenderError(Exception ex)
        {
            Logger.Log(LogType.Error, this, "发送错误", ex);
        }

        /// <summary>
        /// 设置适配器，该方法不会检验<see cref="CanSetDataHandlingAdapter"/>的值。
        /// </summary>
        /// <param name="adapter"></param>
        protected void SetAdapter(DataHandlingAdapter adapter)
        {
            if (adapter is null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            if (Config != null)
            {
                if (Config.GetValue(TouchSocketConfigExtension.MaxPackageSizeProperty) is int v1)
                {
                    adapter.MaxPackageSize = v1;
                }
                if (Config.GetValue(TouchSocketConfigExtension.CacheTimeoutProperty) != TimeSpan.Zero)
                {
                    adapter.CacheTimeout = Config.GetValue(TouchSocketConfigExtension.CacheTimeoutProperty);
                }
                if (Config.GetValue(TouchSocketConfigExtension.CacheTimeoutEnableProperty) is bool v2)
                {
                    adapter.CacheTimeoutEnable = v2;
                }
                if (Config.GetValue(TouchSocketConfigExtension.UpdateCacheTimeWhenRevProperty) is bool v3)
                {
                    adapter.UpdateCacheTimeWhenRev = v3;
                }
            }

            adapter.OnLoaded(this);
            adapter.ReceivedCallBack = PrivateHandleReceivedData;
            adapter.SendCallBack = DefaultSend;
            m_adapter = adapter;
        }

        private void BeginReceive()
        {
            m_workStream.SafeDispose();
            if (UseSsl)
            {
                ClientSslOption sslOption = (ClientSslOption)m_config.GetValue(TouchSocketConfigExtension.SslOptionProperty);
                SslStream sslStream = (sslOption.CertificateValidationCallback != null) ? new SslStream(new NetworkStream(m_mainSocket, false), false, sslOption.CertificateValidationCallback) : new SslStream(new NetworkStream(m_mainSocket, false), false);
                if (sslOption.ClientCertificates == null)
                {
                    sslStream.AuthenticateAsClient(sslOption.TargetHost);
                }
                else
                {
                    sslStream.AuthenticateAsClient(sslOption.TargetHost, sslOption.ClientCertificates, sslOption.SslProtocols, sslOption.CheckCertificateRevocation);
                }
                m_workStream = sslStream;
                if (m_receiveType != ReceiveType.None)
                {
                    BeginSsl();
                }
            }
            else
            {
                if (m_receiveType == ReceiveType.Auto)
                {
                    SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
                    eventArgs.Completed += EventArgs_Completed;

                    ByteBlock byteBlock = BytePool.GetByteBlock(BufferLength);
                    eventArgs.UserToken = byteBlock;
                    eventArgs.SetBuffer(byteBlock.Buffer, 0, byteBlock.Buffer.Length);
                    if (!m_mainSocket.ReceiveAsync(eventArgs))
                    {
                        ProcessReceived(eventArgs);
                    }
                }
            }
        }

        private void BeginSsl()
        {
            ByteBlock byteBlock = new ByteBlock(BufferLength);
            try
            {
                m_workStream.BeginRead(byteBlock.Buffer, 0, byteBlock.Capacity, EndSsl, byteBlock);
            }
            catch (Exception ex)
            {
                byteBlock.Dispose();
                BreakOut(ex.Message, false);
            }
        }

        private void EndSsl(IAsyncResult result)
        {
            ByteBlock byteBlock = (ByteBlock)result.AsyncState;
            try
            {
                int r = m_workStream.EndRead(result);
                if (r == 0)
                {
                    BreakOut("远程终端主动关闭", false);
                }
                byteBlock.SetLength(r);
                HandleBuffer(byteBlock);
                BeginSsl();
            }
            catch (System.Exception ex)
            {
                byteBlock.Dispose();
                BreakOut(ex.Message, false);
            }
        }

        private Socket CreateSocket(IPHost iPHost)
        {
            Socket socket = new Socket(iPHost.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.ReceiveBufferSize = BufferLength;
            socket.SendBufferSize = BufferLength;
#if NET45_OR_GREATER
            KeepAliveValue keepAliveValue = m_config.GetValue<KeepAliveValue>(TouchSocketConfigExtension.KeepAliveValueProperty);
            if (keepAliveValue.Enable)
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                socket.IOControl(IOControlCode.KeepAliveValues, keepAliveValue.KeepAliveTime, null);
            }
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                KeepAliveValue keepAliveValue = m_config.GetValue<KeepAliveValue>(TouchSocketConfigExtension.KeepAliveValueProperty);
                if (keepAliveValue.Enable)
                {
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    socket.IOControl(IOControlCode.KeepAliveValues, keepAliveValue.KeepAliveTime, null);
                }
            }
#endif
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, m_config.GetValue<bool>(TouchSocketConfigExtension.NoDelayProperty));
            if (m_config.GetValue<IPHost>(TouchSocketConfigExtension.BindIPHostProperty) != null)
            {
                if (m_config.GetValue<bool>(TouchSocketConfigExtension.ReuseAddressProperty))
                {
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                }
                socket.Bind(m_config.GetValue<IPHost>(TouchSocketConfigExtension.BindIPHostProperty).EndPoint);
            }
            return socket;
        }

        private void EventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                ProcessReceived(e);
            }
            catch (System.Exception ex)
            {
                e.Dispose();
                BreakOut(ex.Message, false);
            }
        }

        /// <summary>
        /// 处理数据
        /// </summary>
        private void HandleBuffer(ByteBlock byteBlock)
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
                if (UsePlugin && PluginsManager.Raise<ITcpPlugin>(nameof(ITcpPlugin.OnReceivingData), this, new ByteBlockEventArgs(byteBlock)))
                {
                    return;
                }
                if (m_adapter == null)
                {
                    Logger.Error(this, TouchSocketStatus.NullDataAdapter.GetDescription());
                    return;
                }
                m_adapter.ReceivedInput(byteBlock);
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

        #region 发送

        #region 同步发送

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <exception cref="NotConnectedException"></exception>
        /// <exception cref="OverlengthException"></exception>
        /// <exception cref="Exception"></exception>
        public void Send(IRequestInfo requestInfo)
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"><inheritdoc/></param>
        /// <param name="offset"><inheritdoc/></param>
        /// <param name="length"><inheritdoc/></param>
        /// <exception cref="NotConnectedException"><inheritdoc/></exception>
        /// <exception cref="OverlengthException"><inheritdoc/></exception>
        /// <exception cref="Exception"><inheritdoc/></exception>
        public virtual void Send(byte[] buffer, int offset, int length)
        {
            if (m_adapter == null)
            {
                throw new ArgumentNullException(nameof(DataHandlingAdapter), TouchSocketStatus.NullDataAdapter.GetDescription());
            }
            m_adapter.SendInput(buffer, offset, length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="transferBytes"><inheritdoc/></param>
        /// <exception cref="NotConnectedException"><inheritdoc/></exception>
        /// <exception cref="OverlengthException"><inheritdoc/></exception>
        /// <exception cref="Exception"><inheritdoc/></exception>
        public virtual void Send(IList<ArraySegment<byte>> transferBytes)
        {
            if (m_adapter == null)
            {
                throw new ArgumentNullException(nameof(DataHandlingAdapter), TouchSocketStatus.NullDataAdapter.GetDescription());
            }

            if (m_adapter.CanSplicingSend)
            {
                m_adapter.SendInput(transferBytes);
            }
            else
            {
                ByteBlock byteBlock = BytePool.GetByteBlock(BufferLength);
                try
                {
                    foreach (var item in transferBytes)
                    {
                        byteBlock.Write(item.Array, item.Offset, item.Count);
                    }
                    m_adapter.SendInput(byteBlock.Buffer, 0, byteBlock.Len);
                }
                finally
                {
                    byteBlock.Dispose();
                }
            }
        }

        #endregion 同步发送

        #region 异步发送

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"><inheritdoc/></param>
        /// <param name="offset"><inheritdoc/></param>
        /// <param name="length"><inheritdoc/></param>
        /// <exception cref="NotConnectedException"><inheritdoc/></exception>
        /// <exception cref="OverlengthException"><inheritdoc/></exception>
        /// <exception cref="Exception"><inheritdoc/></exception>
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
        /// <exception cref="NotConnectedException"></exception>
        /// <exception cref="OverlengthException"></exception>
        /// <exception cref="Exception"></exception>
        public virtual Task SendAsync(IRequestInfo requestInfo)
        {
            return EasyTask.Run(() =>
             {
                 Send(requestInfo);
             });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="transferBytes"><inheritdoc/></param>
        /// <exception cref="NotConnectedException"><inheritdoc/></exception>
        /// <exception cref="OverlengthException"><inheritdoc/></exception>
        /// <exception cref="Exception"><inheritdoc/></exception>
        public virtual Task SendAsync(IList<ArraySegment<byte>> transferBytes)
        {
            return EasyTask.Run(() =>
              {
                  Send(transferBytes);
              });
        }

        #endregion 异步发送

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"><inheritdoc/></param>
        /// <param name="offset"><inheritdoc/></param>
        /// <param name="length"><inheritdoc/></param>
        /// <exception cref="NotConnectedException"><inheritdoc/></exception>
        /// <exception cref="OverlengthException"><inheritdoc/></exception>
        /// <exception cref="Exception"><inheritdoc/></exception>
        public void DefaultSend(byte[] buffer, int offset, int length)
        {
            if (!m_online)
            {
                throw new NotConnectedException(TouchSocketStatus.NotConnected.GetDescription());
            }
            if (HandleSendingData(buffer, offset, length))
            {
                if (UseSsl)
                {
                    m_workStream.Write(buffer, offset, length);
                }
                else
                {
                    if (m_useDelaySender && length < TouchSocketUtility.BigDataBoundary)
                    {
                        m_delaySender.Send(new QueueDataBytes(buffer, offset, length));
                    }
                    else
                    {
                        m_mainSocket.AbsoluteSend(buffer, offset, length);
                    }
                }

                LastSendTime = DateTime.Now;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"><inheritdoc/></param>
        /// <param name="offset"><inheritdoc/></param>
        /// <param name="length"><inheritdoc/></param>
        /// <exception cref="NotConnectedException"><inheritdoc/></exception>
        /// <exception cref="OverlengthException"><inheritdoc/></exception>
        /// <exception cref="Exception"><inheritdoc/></exception>
        public Task DefaultSendAsync(byte[] buffer, int offset, int length)
        {
            return EasyTask.Run(() =>
            {
                DefaultSend(buffer, offset, length);
            });
        }

        #endregion 发送

        private void LoadSocketAndReadIpPort()
        {
            if (m_mainSocket == null)
            {
                IP = null;
                Port = -1;
                return;
            }

            string ipport;
            if (m_mainSocket.Connected && m_mainSocket.RemoteEndPoint != null)
            {
                ipport = m_mainSocket.RemoteEndPoint.ToString();
            }
            else if (m_mainSocket.IsBound && m_mainSocket.LocalEndPoint != null)
            {
                ipport = m_mainSocket.LocalEndPoint.ToString();
            }
            else
            {
                return;
            }

            int r = ipport.LastIndexOf(":");
            IP = ipport.Substring(0, r);
            Port = Convert.ToInt32(ipport.Substring(r + 1, ipport.Length - (r + 1)));
        }

        private void ProcessReceived(SocketAsyncEventArgs e)
        {
            if (!m_online)
            {
                e.Dispose();
                return;
            }
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                ByteBlock byteBlock = (ByteBlock)e.UserToken;
                byteBlock.SetLength(e.BytesTransferred);
                HandleBuffer(byteBlock);
                try
                {
                    ByteBlock newByteBlock = BytePool.GetByteBlock(BufferLength);
                    e.UserToken = newByteBlock;
                    e.SetBuffer(newByteBlock.Buffer, 0, newByteBlock.Buffer.Length);

                    if (!MainSocket.ReceiveAsync(e))
                    {
                        ProcessReceived(e);
                    }
                }
                catch (System.Exception ex)
                {
                    e.Dispose();
                    BreakOut(ex.Message, false);
                }
            }
            else
            {
                e.Dispose();
                BreakOut("远程终端主动关闭", false);
            }
        }
    }
}