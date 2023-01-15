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
using System.Threading
.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// HttpRpcClient
    /// </summary>
    public partial class HttpTouchRpcClient : HttpClientBase, IHttpTouchRpcClient
    {
        private readonly ActionMap m_actionMap;
        private readonly RpcActor m_rpcActor;
        private RpcStore m_rpcStore;

        /// <summary>
        /// 创建一个HttpTouchRpcClient实例。
        /// </summary>
        public HttpTouchRpcClient()
        {
            m_actionMap = new ActionMap();
            m_rpcActor = new RpcActor(false)
            {
                OutputSend = RpcActorSend,
                OnHandshaked = OnRpcActorHandshaked,
                OnRouting = OnRpcActorRouting,
                OnReceived = OnRpcActorReceived,
                OnClose = OnRpcServiceClose,
                OnStreamTransfering = OnRpcActorStreamTransfering,
                OnStreamTransfered = OnRpcActorStreamTransfered,
                OnFileTransfering = OnRpcActorFileTransfering,
                OnFileTransfered = OnRpcActorFileTransfered,
                GetInvokeMethod = GetInvokeMethod,
                Caller = this
            };
        }

        /// <summary>
        /// 服务器映射
        /// </summary>
        public ActionMap ActionMap { get => m_actionMap; }

        /// <inheritdoc/>
        public string ID => m_rpcActor.ID;

        /// <inheritdoc/>
        public bool IsHandshaked => m_rpcActor != null && m_rpcActor.IsHandshaked;

        /// <inheritdoc/>
        public string RootPath { get => m_rpcActor.RootPath; set => m_rpcActor.RootPath = value; }

        /// <inheritdoc/>
        public RpcActor RpcActor => m_rpcActor;

        /// <inheritdoc/>
        public RpcStore RpcStore => m_rpcStore;

        /// <inheritdoc/>
        public SerializationSelector SerializationSelector => m_rpcActor.SerializationSelector;

        /// <inheritdoc/>
        public Func<IRpcClient, bool> TryCanInvoke { get; set; }

        /// <inheritdoc/>
        public bool ChannelExisted(int id)
        {
            return m_rpcActor.ChannelExisted(id);
        }

        /// <inheritdoc/>
        public override ITcpClient Connect(int timeout = 5000)
        {
            lock (SyncRoot)
            {
                if (IsHandshaked)
                {
                    return this;
                }
                if (!Online)
                {
                    base.Connect(timeout);
                }
                HttpRequest httpRequest = new HttpRequest();
                httpRequest.InitHeaders()
                    .AsMethod(TouchRpcUtility.TouchRpc);
                var response = RequestContent(httpRequest);
                if (response.StatusCode == "200")
                {
                    this.SwitchProtocolToTouchRpc();
                    m_rpcActor.Handshake(Config.GetValue<string>(TouchRpcConfigExtensions.VerifyTokenProperty),
                        Config.GetValue(TouchRpcConfigExtensions.DefaultIdProperty), default,
                        timeout, Config.GetValue<Metadata>(TouchRpcConfigExtensions.MetadataProperty));
                    return this;
                }
                else
                {
                    throw new Exception(response.StatusMessage);
                }
            }
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

        /// <inheritdoc/>
        public bool Ping(int timeout = 5000)
        {
            return m_rpcActor.Ping(timeout);
        }

        /// <inheritdoc/>
        public bool Ping(string targetId, int timeout = 5000)
        {
            return m_rpcActor.Ping(targetId, timeout);
        }

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

        /// <inheritdoc/>
        public void ResetID(string newId)
        {
            m_rpcActor.ResetID(newId);
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

        /// <inheritdoc/>
        public bool TrySubscribeChannel(int id, out Channel channel)
        {
            return m_rpcActor.TrySubscribeChannel(id, out channel);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            m_rpcActor.SafeDispose();
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        protected override void HandleReceivedData(ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            if (Protocol == TouchRpcUtility.TouchRpcProtocol && byteBlock != null)
            {
                m_rpcActor.InputReceivedData(byteBlock);
                return;
            }
            base.HandleReceivedData(byteBlock, requestInfo);
        }

        /// <inheritdoc/>
        protected override void LoadConfig(TouchSocketConfig config)
        {
            base.LoadConfig(config);
            m_rpcActor.Logger = Container.Resolve<ILog>();
            m_rpcActor.FileController = Container.GetFileResourceController();
            RootPath = Config.GetValue(TouchRpcConfigExtensions.RootPathProperty);
            m_rpcActor.SerializationSelector = Config.GetValue<SerializationSelector>(TouchRpcConfigExtensions.SerializationSelectorProperty);

            if (config.GetValue<RpcStore>(RpcConfigExtensions.RpcStoreProperty) is RpcStore rpcStore)
            {
                rpcStore.AddRpcParser(GetType().Name, this);
            }
            else
            {
                new RpcStore(config.Container).AddRpcParser(GetType().Name, this);
            }
        }

        #region 发送

        /// <inheritdoc/>
        public void Send(short protocol, byte[] buffer, int offset, int length)
        {
            m_rpcActor.Send(protocol, buffer, offset, length);
        }

        /// <summary>
        /// 不允许直接发送
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public override void Send(byte[] buffer, int offset, int length)
        {
            throw new Exception("不允许直接发送，请指定任意大于0的协议，然后发送。");
        }

        /// <summary>
        /// 不允许直接发送
        /// </summary>
        /// <param name="transferBytes"></param>
        public override void Send(IList<ArraySegment<byte>> transferBytes)
        {
            throw new Exception("不允许直接发送，请指定任意大于0的协议，然后发送。");
        }

        /// <inheritdoc/>
        public Task SendAsync(short protocol, byte[] buffer, int offset, int length)
        {
            return m_rpcActor.SendAsync(protocol, buffer, offset, length);
        }

        /// <summary>
        /// 不允许直接发送
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public override Task SendAsync(byte[] buffer, int offset, int length)
        {
            throw new Exception("不允许直接发送，请指定任意大于0的协议，然后发送。");
        }

        /// <summary>
        /// 不允许直接发送
        /// </summary>
        /// <param name="transferBytes"></param>
        public override Task SendAsync(IList<ArraySegment<byte>> transferBytes)
        {
            throw new Exception("不允许直接发送，请指定任意大于0的协议，然后发送。");
        }

        #endregion 发送

        /// <inheritdoc/>
        protected override void OnDisconnected(DisconnectEventArgs e)
        {
            m_rpcActor?.Close(e.Message);
            base.OnDisconnected(e);
        }

        #region 小文件

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

        #endregion 小文件

        #region 内部委托绑定

        private MethodInstance GetInvokeMethod(string arg)
        {
            return ActionMap.GetMethodInstance(arg);
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

        private void RpcActorSend(RpcActor actor, ArraySegment<byte>[] transferBytes)
        {
            base.Send(transferBytes);
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
        protected virtual void OnHandshaked(MsgEventArgs e)
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
                    ActionMap.Add(attribute.GetInvokenKey(methodInstance), methodInstance);
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