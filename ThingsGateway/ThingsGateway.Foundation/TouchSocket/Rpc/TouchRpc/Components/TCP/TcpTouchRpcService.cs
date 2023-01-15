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
using System.Threading
.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// TcpTouchRpcService
    /// </summary>
    public class TcpTouchRpcService : TcpTouchRpcService<TcpTouchRpcSocketClient>
    {
    }

    /// <summary>
    /// TcpTouchRpcService泛型类型
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    public partial class TcpTouchRpcService<TClient> : TcpService<TClient>, ITcpTouchRpcService where TClient : TcpTouchRpcSocketClient
    {
        /// <summary>
        /// 创建一个TcpTouchRpcService实例。
        /// </summary>
        public TcpTouchRpcService()
        {
            m_actionMap = new ActionMap();
            m_rpcActorGroup = new RpcActorGroup
            {
                OnClose = OnRpcServiceClose,
                OnRouting = OnRpcServiceRouting,
                GetInvokeMethod = GetInvokeMethod,
                OnFileTransfered = OnRpcServiceFileTransfered,
                OnFileTransfering = OnRpcServiceFileTransfering,
                OnFindRpcActor = OnRpcServiceFindRpcActor,
                OnHandshaked = OnRpcServiceHandshaked,
                OnHandshaking = OnRpcServiceHandshaking,
                OnReceived = OnRpcServiceReceived,
                OnStreamTransfered = OnRpcServiceStreamTransfered,
                OnStreamTransfering = OnRpcServiceStreamTransfering,
                OutputSend = RpcServiceOutputSend
            };
        }

        #region 字段

        private readonly ActionMap m_actionMap;
        private readonly RpcActorGroup m_rpcActorGroup;
        private RpcStore m_rpcStore;

        #endregion 字段

        /// <summary>
        /// 方法映射表
        /// </summary>
        public ActionMap ActionMap { get => m_actionMap; }

        /// <inheritdoc/>
        public RpcStore RpcStore => m_rpcStore;

        /// <summary>
        /// 连接令箭
        /// </summary>
        public string VerifyToken => Config.GetValue<string>(TouchRpcConfigExtensions.VerifyTokenProperty);

        /// <inheritdoc/>
        protected override void LoadConfig(TouchSocketConfig config)
        {
            base.LoadConfig(config);
            m_rpcActorGroup.Config = config;
            if (config.GetValue<RpcStore>(RpcConfigExtensions.RpcStoreProperty) is RpcStore rpcStore)
            {
                rpcStore.AddRpcParser(GetType().Name, this);
            }
            else
            {
                new RpcStore(config.Container).AddRpcParser(GetType().Name, this);
            }
        }

        /// <summary>
        /// 客户端请求连接
        /// </summary>
        /// <param name="socketClient"></param>
        /// <param name="e"></param>
        protected override void OnConnecting(TClient socketClient, OperationEventArgs e)
        {
            socketClient.SetRpcActor(m_rpcActorGroup.CreateRpcActor(socketClient));
            base.OnConnecting(socketClient, e);
        }

        #region 事件

        /// <summary>
        /// 当文件传输结束之后。并不意味着完成传输，请通过<see cref="FileTransferStatusEventArgs.Result"/>属性值进行判断。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnFileTransfered(TClient client, FileTransferStatusEventArgs e)
        {
        }

        /// <summary>
        /// 在需要转发路由包时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnRouting(TClient client, PackageRouterEventArgs e)
        {
        }

        /// <summary>
        /// 在文件传输即将进行时触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnFileTransfering(TClient client, FileOperationEventArgs e)
        {
        }

        /// <summary>
        /// 在完成握手连接时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnHandshaked(TClient client, VerifyOptionEventArgs e)
        {
        }

        /// <summary>
        /// 在验证Token时
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        protected virtual void OnHandshaking(TClient client, VerifyOptionEventArgs e)
        {
        }

        /// <summary>
        /// 收到协议数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="protocol"></param>
        /// <param name="byteBlock"></param>
        protected virtual void OnReceived(TClient client, short protocol, ByteBlock byteBlock)
        {
        }

        /// <summary>
        /// 流数据处理，用户需要在此事件中对e.Bucket手动释放。覆盖父类方法将不会触发事件和插件。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnStreamTransfered(TClient client, StreamStatusEventArgs e)
        {
        }

        /// <summary>
        /// 即将接收流数据，用户需要在此事件中对e.Bucket初始化。覆盖父类方法将不会触发事件和插件。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnStreamTransfering(TClient client, StreamOperationEventArgs e)
        {
        }

        #endregion 事件

        #region 小文件

        /// <inheritdoc/>
        public PullSmallFileResult PullSmallFile(string targetId, string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.PullSmallFile(path, metadata, timeout, token);
            }
            else
            {
                return new PullSmallFileResult(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public Task<PullSmallFileResult> PullSmallFileAsync(string targetId, string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.PullSmallFileAsync(path, metadata, timeout, token);
            }
            else
            {
                return Task.FromResult(new PullSmallFileResult(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription(targetId)));
            }
        }

        /// <inheritdoc/>
        public Result PushSmallFile(string targetId, string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.PushSmallFile(savePath, fileInfo, metadata, timeout, token);
            }
            else
            {
                return new Result(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public Task<Result> PushSmallFileAsync(string targetId, string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.PushSmallFileAsync(savePath, fileInfo, metadata, timeout, token);
            }
            else
            {
                return Task.FromResult(new Result(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription(targetId)));
            }
        }

        #endregion 小文件

        #region Rpc

        /// <inheritdoc/>
        public void Invoke(string targetId, string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                client.Invoke(targetId, method, invokeOption, ref parameters, types);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public T Invoke<T>(string targetId, string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.Invoke<T>(targetId, method, invokeOption, ref parameters, types);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public void Invoke(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                client.Invoke(method, invokeOption, parameters);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public T Invoke<T>(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.Invoke<T>(method, invokeOption, parameters);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public Task InvokeAsync(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.InvokeAsync(method, invokeOption, parameters);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public Task<T> InvokeAsync<T>(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.InvokeAsync<T>(method, invokeOption, parameters);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public bool Ping(string targetId, int timeout = 5000)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.Ping(timeout);
            }
            return false;
        }

        #endregion Rpc

        #region 通道

        /// <inheritdoc/>
        public Channel CreateChannel(string targetId)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.CreateChannel();
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public Channel CreateChannel(string targetId, int id)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.CreateChannel(id);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        #endregion 通道

        #region File

        /// <inheritdoc/>
        public Result PullFile(string targetId, FileOperator fileOperator)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.PullFile(fileOperator);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public Task<Result> PullFileAsync(string targetId, FileOperator fileOperator)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.PullFileAsync(fileOperator);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public Result PushFile(string targetId, FileOperator fileOperator)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.PushFile(fileOperator);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public Task<Result> PushFileAsync(string targetId, FileOperator fileOperator)
        {
            if (TryGetSocketClient(targetId, out TClient client))
            {
                return client.PushFileAsync(fileOperator);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        #endregion File

        #region 内部委托绑定

        private MethodInstance GetInvokeMethod(string arg)
        {
            return m_actionMap.GetMethodInstance(arg);
        }

        private void OnRpcServiceClose(RpcActor actor, string arg2)
        {
            TClient client = (TClient)actor.Caller;
            client.Close(arg2);
        }

        private void OnRpcServiceRouting(RpcActor actor, PackageRouterEventArgs e)
        {
            TClient client = (TClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnRouting), client, e))
            {
                return;
            }
            OnRouting(client, e);
        }

        private void OnRpcServiceFileTransfered(RpcActor actor, FileTransferStatusEventArgs e)
        {
            TClient client = (TClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnFileTransfered), client, e))
            {
                return;
            }
            OnFileTransfered(client, e);
        }

        private void OnRpcServiceFileTransfering(RpcActor actor, FileOperationEventArgs e)
        {
            TClient client = (TClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnFileTransfering), client, e))
            {
                return;
            }
            OnFileTransfering(client, e);
        }

        private RpcActor OnRpcServiceFindRpcActor(string arg)
        {
            if (TryGetSocketClient(arg, out TClient client))
            {
                return client.RpcActor;
            }
            return null;
        }

        private void OnRpcServiceHandshaked(RpcActor actor, VerifyOptionEventArgs e)
        {
            TClient client = (TClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnHandshaked), client, e))
            {
                return;
            }
            OnHandshaked(client, e);
        }

        private void OnRpcServiceHandshaking(RpcActor actor, VerifyOptionEventArgs e)
        {
            TClient client = (TClient)actor.Caller;
            if (e.Token == VerifyToken)
            {
                e.IsPermitOperation = true;
            }
            else
            {
                e.Message = "Token不受理";
            }
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnHandshaking), client, e))
            {
                return;
            }
            OnHandshaking(client, e);
        }

        private void OnRpcServiceReceived(RpcActor actor, short protocol, ByteBlock byteBlock)
        {
            TClient client = (TClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnReceivedProtocolData), client, new ProtocolDataEventArgs(protocol, byteBlock)))
            {
                return;
            }

            OnReceived(client, protocol, byteBlock);
        }

        private void OnRpcServiceStreamTransfered(RpcActor actor, StreamStatusEventArgs e)
        {
            TClient client = (TClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnStreamTransfered), client, e))
            {
                return;
            }
            OnStreamTransfered(client, e);
        }

        private void OnRpcServiceStreamTransfering(RpcActor actor, StreamOperationEventArgs e)
        {
            TClient client = (TClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnStreamTransfering), client, e))
            {
                return;
            }
            OnStreamTransfering(client, e);
        }

        private void RpcServiceOutputSend(RpcActor actor, ArraySegment<byte>[] arg3)
        {
            TClient client = (TClient)actor.Caller;
            client.RpcActorSend(arg3);
        }

        #endregion 内部委托绑定

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
            m_rpcActorGroup.RpcStore = rpcStore;
            m_rpcStore = rpcStore;
        }

        #endregion RPC解析器

        #region 发送

        /// <inheritdoc/>
        public void Send(string targetId, short protocol, byte[] buffer, int offset, int length)
        {
            if (SocketClients.TryGetSocketClient(targetId, out TClient client))
            {
                client.Send(protocol, buffer, offset, length);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <inheritdoc/>
        public Task SendAsync(string targetId, short protocol, byte[] buffer, int offset, int length)
        {
            if (SocketClients.TryGetSocketClient(targetId, out TClient client))
            {
                return client.SendAsync(protocol, buffer, offset, length);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        #endregion 发送
    }
}