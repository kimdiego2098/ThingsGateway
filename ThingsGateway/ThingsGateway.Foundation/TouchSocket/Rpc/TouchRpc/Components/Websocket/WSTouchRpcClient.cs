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
using System.IO;
using System.Net.WebSockets;
using System.Threading
.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// WSTouchRpcClient
    /// </summary>
    public partial class WSTouchRpcClient : DisposableObject, IWSTouchRpcClient, IRpcActor, ISenderBase
    {
        private readonly ActionMap m_actionMap;
        private readonly ArraySegment<byte> m_buffer;
        private readonly RpcActor m_rpcActor;
        private ClientWebSocket m_client;
        private TouchSocketConfig m_config;
        private IPHost m_remoteIPHost;
        private RpcStore m_rpcStore;

        /// <summary>
        /// 创建一个WSTouchRpcClient实例。
        /// </summary>
        public WSTouchRpcClient()
        {
            m_buffer = new ArraySegment<byte>(new byte[1024 * 64]);
            m_actionMap = new ActionMap();
            m_rpcActor = new RpcActor(false)
            {
                OutputSend = RpcActorSend,
                OnRouting = OnRpcActorRouting,
                OnHandshaked = OnRpcActorHandshaked,
                OnReceived = OnRpcActorReceived,
                GetInvokeMethod = GetInvokeMethod,
                OnClose = OnRpcServiceClose,
                OnStreamTransfering = OnRpcActorStreamTransfering,
                OnStreamTransfered = OnRpcActorStreamTransfered,
                OnFileTransfering = OnRpcActorFileTransfering,
                OnFileTransfered = OnRpcActorFileTransfered,
                Caller = this
            };
        }

        /// <summary>
        /// 方法映射表
        /// </summary>
        public ActionMap ActionMap { get => m_actionMap; }

        /// <inheritdoc/>
        public bool CanSend => m_client.State == WebSocketState.Open;

        /// <summary>
        /// 客户端配置
        /// </summary>
        public TouchSocketConfig Config => m_config;

        /// <inheritdoc/>
        public IContainer Container { get; private set; }

        /// <summary>
        /// 断开连接
        /// </summary>
        public DisconnectEventHandler<WSTouchRpcClient> Disconnected { get; set; }

        /// <inheritdoc/>
        public string ID => m_rpcActor.ID;

        /// <inheritdoc/>
        public bool IsHandshaked => m_rpcActor.IsHandshaked;

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastActiveTime { get; private set; }

        /// <inheritdoc/>
        public ILog Logger => m_rpcActor.Logger;

        /// <inheritdoc/>
        public IPluginsManager PluginsManager { get; private set; }

        /// <inheritdoc/>
        public IPHost RemoteIPHost => m_remoteIPHost;

        /// <inheritdoc/>
        public string RootPath { get => m_rpcActor.RootPath; set => m_rpcActor.RootPath = value; }

        /// <inheritdoc/>
        public RpcActor RpcActor => m_rpcActor;

        /// <inheritdoc/>
        public RpcStore RpcStore { get => m_rpcStore; }

        /// <inheritdoc/>
        public SerializationSelector SerializationSelector => m_rpcActor.SerializationSelector;

        /// <inheritdoc/>
        public Func<IRpcClient, bool> TryCanInvoke { get => m_rpcActor.TryCanInvoke; set => m_rpcActor.TryCanInvoke = value; }

        /// <inheritdoc/>
        public bool UsePlugin { get; private set; }

        /// <inheritdoc/>
        public bool ChannelExisted(int id)
        {
            return m_rpcActor.ChannelExisted(id);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="msg"></param>
        public void Close(string msg)
        {
            if (m_client?.State != WebSocketState.Open)
            {
                return;
            }
            m_client.SafeDispose();
            m_rpcActor.SafeDispose();
            BreakOut($"调用{nameof(Close)}", true);
        }

        /// <inheritdoc/>
        public async Task ConnectAsync(int timeout = 5000)
        {
            if (!RemoteIPHost.IsUri)
            {
                throw new Exception("RemoteIPHost必须为Uri格式。");
            }
            if (m_client == null || m_client.State != WebSocketState.Open)
            {
                m_client.SafeDispose();
                m_client = new ClientWebSocket();
                await m_client.ConnectAsync(RemoteIPHost.Uri, default);

                _ = BeginReceive(null);
            }

            if (IsHandshaked)
            {
                return;
            }

            m_rpcActor.Handshake(Config.GetValue(TouchRpcConfigExtensions.VerifyTokenProperty),
                Config.GetValue(TouchRpcConfigExtensions.DefaultIdProperty), default,
                timeout, Config.GetValue(TouchRpcConfigExtensions.MetadataProperty));
        }

        /// <inheritdoc/>
        public Channel CreateChannel()
        {
            return m_rpcActor.CreateChannel();
        }

        /// <inheritdoc/>
        public Channel CreateChannel(int id)
        {
            return m_rpcActor.CreateChannel(id);
        }

        /// <inheritdoc/>
        public Channel CreateChannel(string targetId)
        {
            return m_rpcActor.CreateChannel(targetId);
        }

        /// <inheritdoc/>
        public Channel CreateChannel(string targetId, int id)
        {
            return m_rpcActor.CreateChannel(targetId, id);
        }

        /// <inheritdoc/>
        public bool Ping(string targetId, int timeout = 5000)
        {
            return m_rpcActor.Ping(targetId, timeout);
        }

        /// <inheritdoc/>
        public bool Ping(int timeout = 5000)
        {
            return m_rpcActor.Ping(timeout);
        }

        /// <inheritdoc/>
        public void ResetID(string newID)
        {
            m_rpcActor.ResetID(newID);
        }

        /// <inheritdoc/>
        public void Send(short protocol, byte[] buffer, int offset, int length)
        {
            m_rpcActor.Send(protocol, buffer, offset, length);
        }

        /// <inheritdoc/>
        public Task SendAsync(short protocol, byte[] buffer, int offset, int length)
        {
            return m_rpcActor.SendAsync(protocol, buffer, offset, length);
        }

        /// <inheritdoc/>
        public Result SendStream(Stream stream, StreamOperator streamOperator, Metadata metadata = null)
        {
            return m_rpcActor.SendStream(stream, streamOperator, metadata);
        }

        /// <inheritdoc/>
        public Task<Result> SendStreamAsync(Stream stream, StreamOperator streamOperator, Metadata metadata = null)
        {
            return m_rpcActor.SendStreamAsync(stream, streamOperator, metadata);
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="ipHost"></param>
        /// <returns></returns>
        public IWSTouchRpcClient Setup(string ipHost)
        {
            TouchSocketConfig config = new TouchSocketConfig();
            config.SetRemoteIPHost(new IPHost(ipHost));
            return Setup(config);
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <exception cref="Exception"></exception>
        public IWSTouchRpcClient Setup(TouchSocketConfig clientConfig)
        {
            m_config = clientConfig;
            LoadConfig(m_config);
            return this;
        }

        /// <inheritdoc/>
        public bool TrySubscribeChannel(int id, out Channel channel)
        {
            return m_rpcActor.TrySubscribeChannel(id, out channel);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            m_client.SafeDispose();
            m_rpcActor.SafeDispose();
            base.Dispose(disposing);
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
            m_remoteIPHost = config.GetValue<IPHost>(Sockets.TouchSocketConfigExtension.RemoteIPHostProperty);
            Container = config.Container;
            UsePlugin = config.IsUsePlugin;
            PluginsManager = config.PluginsManager;

            m_rpcActor.Logger = Container.Resolve<ILog>();
            m_rpcActor.FileController = Container.GetFileResourceController();
            RootPath = Config.GetValue<string>(TouchRpcConfigExtensions.RootPathProperty);
            m_rpcActor.SerializationSelector = Config.GetValue<SerializationSelector>(TouchRpcConfigExtensions.SerializationSelectorProperty);
        }

        /// <summary>
        /// 已断开连接。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDisconnected(DisconnectEventArgs e)
        {
            if (UsePlugin && PluginsManager.Raise<ITcpPlugin>(nameof(ITcpPlugin.OnDisconnected), this, e))
            {
                return;
            }
            Disconnected?.Invoke(this, e);
        }

        private async Task BeginReceive(ByteBlock byteBlock)
        {
            try
            {
                byteBlock ??= new ByteBlock();
                var result = await m_client.ReceiveAsync(m_buffer, default);
                if (result.Count == 0)
                {
                    BreakOut("远程终端主动关闭", false);
                }
                LastActiveTime = DateTime.Now;
                byteBlock.Write(m_buffer.Array, 0, result.Count);
                if (result.EndOfMessage)
                {
                    try
                    {
                        m_rpcActor.InputReceivedData(byteBlock);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        byteBlock.SafeDispose();
                    }
                    await BeginReceive(null);
                }
                else
                {
                    await BeginReceive(byteBlock);
                }
            }
            catch (Exception ex)
            {
                BreakOut(ex.Message, false);
            }
        }

        private void BreakOut(string msg, bool manual)
        {
            m_client.SafeDispose();
            m_rpcActor.SafeDispose();
            OnDisconnected(new DisconnectEventArgs(manual, msg));
        }

        #region 小文件

        /// <inheritdoc/>
        public PullSmallFileResult PullSmallFile(string targetId, string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return m_rpcActor.PullSmallFile(targetId, path, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public PullSmallFileResult PullSmallFile(string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return m_rpcActor.PullSmallFile(path, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public Task<PullSmallFileResult> PullSmallFileAsync(string targetId, string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return m_rpcActor.PullSmallFileAsync(targetId, path, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public Task<PullSmallFileResult> PullSmallFileAsync(string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return m_rpcActor.PullSmallFileAsync(path, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public Result PushSmallFile(string targetId, string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return m_rpcActor.PushSmallFile(targetId, savePath, fileInfo, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public Result PushSmallFile(string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return m_rpcActor.PushSmallFile(savePath, fileInfo, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public Task<Result> PushSmallFileAsync(string targetId, string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return m_rpcActor.PushSmallFileAsync(targetId, savePath, fileInfo, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public Task<Result> PushSmallFileAsync(string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return m_rpcActor.PushSmallFileAsync(savePath, fileInfo, metadata, timeout, token);
        }

        #endregion 小文件

        #region FileTransfer

        /// <inheritdoc/>
        public Result PullFile(FileOperator fileOperator)
        {
            return m_rpcActor.PullFile(fileOperator);
        }

        /// <inheritdoc/>
        public Result PullFile(string targetId, FileOperator fileOperator)
        {
            return m_rpcActor.PullFile(targetId, fileOperator);
        }

        /// <inheritdoc/>
        public Task<Result> PullFileAsync(FileOperator fileOperator)
        {
            return m_rpcActor.PullFileAsync(fileOperator);
        }

        /// <inheritdoc/>
        public Task<Result> PullFileAsync(string targetId, FileOperator fileOperator)
        {
            return m_rpcActor.PullFileAsync(targetId, fileOperator);
        }

        /// <inheritdoc/>
        public Result PushFile(FileOperator fileOperator)
        {
            return m_rpcActor.PushFile(fileOperator);
        }

        /// <inheritdoc/>
        public Result PushFile(string targetId, FileOperator fileOperator)
        {
            return m_rpcActor.PushFile(targetId, fileOperator);
        }

        /// <inheritdoc/>
        public Task<Result> PushFileAsync(FileOperator fileOperator)
        {
            return m_rpcActor.PushFileAsync(fileOperator);
        }

        /// <inheritdoc/>
        public Task<Result> PushFileAsync(string targetId, FileOperator fileOperator)
        {
            return m_rpcActor.PushFileAsync(targetId, fileOperator);
        }

        #endregion FileTransfer

        #region RPC

        /// <inheritdoc/>
        public void Invoke(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            m_rpcActor.Invoke(method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public T Invoke<T>(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.Invoke<T>(method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public T Invoke<T>(string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            return m_rpcActor.Invoke<T>(method, invokeOption, ref parameters, types);
        }

        /// <inheritdoc/>
        public void Invoke(string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            m_rpcActor.Invoke(method, invokeOption, ref parameters, types);
        }

        /// <inheritdoc/>
        public void Invoke(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            m_rpcActor.Invoke(targetId, method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public T Invoke<T>(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.Invoke<T>(targetId, method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public void Invoke(string targetId, string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            m_rpcActor.Invoke(targetId, method, invokeOption, ref parameters, types);
        }

        /// <inheritdoc/>
        public T Invoke<T>(string targetId, string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            return m_rpcActor.Invoke<T>(targetId, method, invokeOption, ref parameters, types);
        }

        /// <inheritdoc/>
        public Task InvokeAsync(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.InvokeAsync(method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public Task<T> InvokeAsync<T>(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.InvokeAsync<T>(method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public Task InvokeAsync(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.InvokeAsync(targetId, method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public Task<T> InvokeAsync<T>(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.InvokeAsync<T>(targetId, method, invokeOption, parameters);
        }

        #endregion RPC

        #region 内部委托绑定

        private MethodInstance GetInvokeMethod(string arg)
        {
            return m_actionMap.GetMethodInstance(arg);
        }

        private void OnRpcActorFileTransfered(RpcActor actor, FileTransferStatusEventArgs e)
        {
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnFileTransfered), this, e))
            {
                return;
            }
            OnFileTransfered(e);
        }

        private void OnRpcActorFileTransfering(RpcActor actor, FileOperationEventArgs e)
        {
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnFileTransfering), this, e))
            {
                return;
            }
            OnFileTransfering(e);
        }

        private void OnRpcActorHandshaked(RpcActor actor, VerifyOptionEventArgs e)
        {
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnHandshaked), this, e))
            {
                return;
            }
            OnHandshaked(e);
        }

        private void OnRpcActorReceived(RpcActor actor, short protocol, ByteBlock byteBlock)
        {
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnReceivedProtocolData), this, new ProtocolDataEventArgs(protocol, byteBlock)))
            {
                return;
            }

            OnReceived(protocol, byteBlock);
        }

        private void OnRpcActorRouting(RpcActor actor, PackageRouterEventArgs e)
        {
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnRouting), this, e))
            {
                return;
            }
            OnRouting(e);
        }

        private void OnRpcActorStreamTransfered(RpcActor actor, StreamStatusEventArgs e)
        {
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnStreamTransfered), this, e))
            {
                return;
            }
            OnStreamTransfered(e);
        }

        private void OnRpcActorStreamTransfering(RpcActor actor, StreamOperationEventArgs e)
        {
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnStreamTransfering), this, e))
            {
                return;
            }
            OnStreamTransfering(e);
        }

        private void OnRpcServiceClose(RpcActor actor, string arg2)
        {
            Close(arg2);
        }

        private async void RpcActorSend(RpcActor actor, ArraySegment<byte>[] transferBytes)
        {
            LastActiveTime = DateTime.Now;
            for (int i = 0; i < transferBytes.Length; i++)
            {
                if (i == transferBytes.Length - 1)
                {
                    await m_client.SendAsync(transferBytes[i], WebSocketMessageType.Binary, true, CancellationToken.None);
                }
                else
                {
                    await m_client.SendAsync(transferBytes[i], WebSocketMessageType.Binary, false, CancellationToken.None);
                }
            }
        }

        #endregion 内部委托绑定

        #region 事件触发

        /// <summary>
        /// 当文件传输结束之后。并不意味着完成传输，请通过<see cref="FileTransferStatusEventArgs.Result"/>属性值进行判断。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFileTransfered(FileTransferStatusEventArgs e)
        {
        }

        /// <summary>
        /// 在文件传输即将进行时触发。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFileTransfering(FileOperationEventArgs e)
        {
        }

        /// <summary>
        /// 在完成握手连接时
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnHandshaked(VerifyOptionEventArgs e)
        {
        }

        /// <summary>
        /// 收到数据。
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="byteBlock"></param>
        protected virtual void OnReceived(short protocol, ByteBlock byteBlock)
        {
        }

        /// <summary>
        /// 当需要转发路由包时
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRouting(PackageRouterEventArgs e)
        {
        }

        /// <summary>
        /// 流数据处理，用户需要在此事件中对e.Bucket手动释放。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStreamTransfered(StreamStatusEventArgs e)
        {
        }

        /// <summary>
        /// 即将接收流数据，用户需要在此事件中对e.Bucket初始化。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStreamTransfering(StreamOperationEventArgs e)
        {
        }

        #endregion 事件触发

        #region RPC解析器

        void IRpcParser.OnRegisterServer(MethodInstance[] methodInstances)
        {
            foreach (var methodInstance in methodInstances)
            {
                if (methodInstance.GetAttribute<TouchRpcAttribute>() is TouchRpcAttribute attribute)
                {
                    m_actionMap.Add(attribute.GetInvokenKey(methodInstance), methodInstance);
                }
            }
        }

        void IRpcParser.OnUnregisterServer(MethodInstance[] methodInstances)
        {
            foreach (var methodInstance in methodInstances)
            {
                if (methodInstance.GetAttribute<TouchRpcAttribute>() is TouchRpcAttribute attribute)
                {
                    m_actionMap.Remove(attribute.GetInvokenKey(methodInstance));
                }
            }
        }

        void IRpcParser.SetRpcStore(RpcStore rpcStore)
        {
            m_rpcActor.RpcStore = rpcStore;
            m_rpcStore = rpcStore;
        }

        #endregion RPC解析器
    }
}