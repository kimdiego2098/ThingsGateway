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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ThingsGateway.Foundation.Resources;
using ThingsGateway.Foundation.Rpc;
using ThingsGateway.Foundation.Rpc.TouchRpc;
using ThingsGateway.Foundation.Sockets;

namespace ThingsGateway.Foundation.AspNetCore
{
    /// <summary>
    /// WSTouchRpcService
    /// </summary>
    public class WSTouchRpcService : DisposableObject, IWSTouchRpcService
    {
        #region SocketClient

        private readonly ConcurrentDictionary<string, WSTouchRpcSocketClient> m_tokenDic = new ConcurrentDictionary<string, WSTouchRpcSocketClient>();

        /// <summary>
        /// 数量
        /// </summary>
        public int Count => m_tokenDic.Count;

        /// <summary>
        /// 获取SocketClient
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WSTouchRpcSocketClient this[string id]
        {
            get
            {
                WSTouchRpcSocketClient t;
                TryGetSocketClient(id, out t);
                return t;
            }
        }

        /// <summary>
        /// 获取所有的客户端
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WSTouchRpcSocketClient> GetClients()
        {
            return m_tokenDic.Values;
        }

        /// <summary>
        /// 获取ID集合
        /// </summary>
        /// <returns></returns>
        public string[] GetIDs()
        {
            return m_tokenDic.Keys.ToArray();
        }

        /// <summary>
        /// 根据ID判断SocketClient是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SocketClientExist(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            if (m_tokenDic.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试获取实例
        /// </summary>
        /// <param name="id"></param>
        /// <param name="socketClient"></param>
        /// <returns></returns>
        public bool TryGetSocketClient(string id, out WSTouchRpcSocketClient socketClient)
        {
            if (string.IsNullOrEmpty(id))
            {
                socketClient = null;
                return false;
            }

            return m_tokenDic.TryGetValue(id, out socketClient);
        }

        internal bool TryAdd(string id, WSTouchRpcSocketClient socketClient)
        {
            return m_tokenDic.TryAdd(id, socketClient);
        }

        internal bool TryRemove(string id, out WSTouchRpcSocketClient socketClient)
        {
            if (string.IsNullOrEmpty(id))
            {
                socketClient = null;
                return false;
            }
            return m_tokenDic.TryRemove(id, out socketClient);
        }

        #endregion SocketClient

        private readonly ActionMap m_actionMap;
        private readonly RpcActorGroup m_rpcActorGroup;
        private long m_idCount;
        private RpcStore m_rpcStore;

        /// <summary>
        /// 创建一个基于WS的Touch服务器。
        /// </summary>
        /// <param name="config"></param>
        public WSTouchRpcService(TouchSocketConfig config)
        {
            m_actionMap = new ActionMap();
            Config = config;
            UsePlugin = config.IsUsePlugin;
            PluginsManager = config.PluginsManager;
            VerifyToken = config.GetValue<string>(TouchRpcConfigExtensions.VerifyTokenProperty);

            if (config.GetValue(TouchSocketConfigExtension.GetDefaultNewIDProperty) is Func<string> fun)
            {
                GetDefaultNewID = fun;
            }
            else
            {
                GetDefaultNewID = () => { return Interlocked.Increment(ref m_idCount).ToString(); };
            }

            m_rpcActorGroup = new RpcActorGroup
            {
                Config = Config,
                OnClose = OnRpcServiceClose,
                OnFileTransfered = OnRpcServiceFileTransfered,
                GetInvokeMethod = GetInvokeMethod,
                OnFileTransfering = OnRpcServiceFileTransfering,
                OnFindRpcActor = OnRpcServiceFindRpcActor,
                OnRouting = OnRpcServiceRouting,
                OnHandshaked = OnRpcServiceHandshaked,
                OnHandshaking = OnRpcServiceHandshaking,
                OnReceived = OnRpcServiceReceived,
                OnStreamTransfered = OnRpcServiceStreamTransfered,
                OnStreamTransfering = OnRpcServiceStreamTransfering,
                OutputSend = RpcServiceOutputSend
            };

            if (config.GetValue(RpcConfigExtensions.RpcStoreProperty) is RpcStore rpcStore)
            {
                rpcStore.AddRpcParser(GetType().Name, this);
            }
            else
            {
                new RpcStore(config.Container).AddRpcParser(GetType().Name, this);
            }
        }

        /// <summary>
        /// 方法映射表
        /// </summary>
        public ActionMap ActionMap { get => m_actionMap; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TouchSocketConfig Config { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IContainer Container { get; private set; }

        /// <summary>
        /// 获取默认新ID。
        /// </summary>
        public Func<string> GetDefaultNewID { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IPluginsManager PluginsManager { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public RpcStore RpcStore => m_rpcStore;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool UsePlugin { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string VerifyToken { get; private set; }

        #region 通道

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public Channel CreateChannel(string targetId)
        {
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
            {
                return client.CreateChannel();
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Channel CreateChannel(string targetId, int id)
        {
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
            {
                return client.CreateChannel(id);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        #endregion 通道

        #region Rpc

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="method"></param>
        /// <param name="invokeOption"></param>
        /// <param name="parameters"></param>
        /// <param name="types"></param>
        public void Invoke(string targetId, string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
            {
                client.Invoke(targetId, method, invokeOption, ref parameters, types);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetId"></param>
        /// <param name="method"></param>
        /// <param name="invokeOption"></param>
        /// <param name="parameters"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public T Invoke<T>(string targetId, string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
            {
                return client.Invoke<T>(targetId, method, invokeOption, ref parameters, types);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <summary>
        /// 反向调用客户端Rpc
        /// </summary>
        /// <param name="targetId">客户端ID</param>
        /// <param name="method">方法名</param>
        /// <param name="invokeOption">调用配置</param>
        /// <param name="parameters">参数</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcSerializationException">序列化异常</exception>
        /// <exception cref="RpcInvokeException">调用内部异常</exception>
        /// <exception cref="ClientNotFindException">没有找到ID对应的客户端</exception>
        /// <exception cref="Exception">其他异常</exception>
        public void Invoke(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
            {
                client.Invoke(method, invokeOption, parameters);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <summary>
        /// 反向调用客户端Rpc
        /// </summary>
        /// <param name="targetId">客户端ID</param>
        /// <param name="method">方法名</param>
        /// <param name="invokeOption">调用配置</param>
        /// <param name="parameters">参数</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcSerializationException">序列化异常</exception>
        /// <exception cref="RpcInvokeException">调用内部异常</exception>
        /// <exception cref="ClientNotFindException">没有找到ID对应的客户端</exception>
        /// <exception cref="Exception">其他异常</exception>
        /// <returns>返回值</returns>
        public T Invoke<T>(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
            {
                return client.Invoke<T>(method, invokeOption, parameters);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <summary>
        /// 反向调用客户端Rpc
        /// </summary>
        /// <param name="targetId">客户端ID</param>
        /// <param name="method">方法名</param>
        /// <param name="invokeOption">调用配置</param>
        /// <param name="parameters">参数</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcSerializationException">序列化异常</exception>
        /// <exception cref="RpcInvokeException">调用内部异常</exception>
        /// <exception cref="ClientNotFindException">没有找到ID对应的客户端</exception>
        /// <exception cref="Exception">其他异常</exception>
        public Task InvokeAsync(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
            {
                return client.InvokeAsync(method, invokeOption, parameters);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        /// <summary>
        /// 反向调用客户端Rpc
        /// </summary>
        /// <param name="targetId">客户端ID</param>
        /// <param name="method">方法名</param>
        /// <param name="invokeOption">调用配置</param>
        /// <param name="parameters">参数</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcSerializationException">序列化异常</exception>
        /// <exception cref="RpcInvokeException">调用内部异常</exception>
        /// <exception cref="ClientNotFindException">没有找到ID对应的客户端</exception>
        /// <exception cref="Exception">其他异常</exception>
        /// <returns>返回值</returns>
        public Task<T> InvokeAsync<T>(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
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
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
            {
                return client.Ping(timeout);
            }
            return false;
        }

        #endregion Rpc

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

        #region File

        /// <inheritdoc/>
        public Result PullFile(string targetId, FileOperator fileOperator)
        {
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
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
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
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
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
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
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
            {
                return client.PushFileAsync(fileOperator);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
            }
        }

        #endregion File

        /// <inheritdoc/>
        public void ResetID(string oldID, string newID)
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
            if (m_tokenDic.TryGetValue(oldID, out WSTouchRpcSocketClient socketClient))
            {
                socketClient.ResetID(newID);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(oldID));
            }
        }

        #region 内部委托绑定

        private MethodInstance GetInvokeMethod(string arg)
        {
            return m_actionMap.GetMethodInstance(arg);
        }

        private void OnRpcServiceClose(RpcActor actor, string arg2)
        {
            WSTouchRpcSocketClient client = (WSTouchRpcSocketClient)actor.Caller;
            client.Close(arg2);
        }

        private void OnRpcServiceFileTransfered(RpcActor actor, FileTransferStatusEventArgs e)
        {
            WSTouchRpcSocketClient client = (WSTouchRpcSocketClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnFileTransfered), client, e))
            {
                return;
            }
            OnFileTransfered(client, e);
        }

        private void OnRpcServiceFileTransfering(RpcActor actor, FileOperationEventArgs e)
        {
            WSTouchRpcSocketClient client = (WSTouchRpcSocketClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnFileTransfering), client, e))
            {
                return;
            }
            OnFileTransfering(client, e);
        }

        private RpcActor OnRpcServiceFindRpcActor(string id)
        {
            if (TryGetSocketClient(id, out WSTouchRpcSocketClient client))
            {
                return client.RpcActor;
            }
            return null;
        }

        private void OnRpcServiceRouting(RpcActor actor, PackageRouterEventArgs e)
        {
            WSTouchRpcSocketClient client = (WSTouchRpcSocketClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnRouting), client, e))
            {
                return;
            }
            OnRouting(client, e);
        }


        private void OnRpcServiceHandshaked(RpcActor actor, VerifyOptionEventArgs e)
        {
            WSTouchRpcSocketClient client = (WSTouchRpcSocketClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnHandshaked), client, e))
            {
                return;
            }
            OnHandshaked(client, e);
        }

        private void OnRpcServiceHandshaking(RpcActor actor, VerifyOptionEventArgs e)
        {
            WSTouchRpcSocketClient client = (WSTouchRpcSocketClient)actor.Caller;
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
            WSTouchRpcSocketClient client = (WSTouchRpcSocketClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnReceivedProtocolData), client, new ProtocolDataEventArgs(protocol, byteBlock)))
            {
                return;
            }

            OnReceived(client, protocol, byteBlock);
        }

        private void OnRpcServiceResetID(bool b, RpcActor actor, WaitSetID arg2)
        {
            ResetID(arg2.OldID, arg2.NewID);
        }

        private void OnRpcServiceStreamTransfered(RpcActor actor, StreamStatusEventArgs e)
        {
            WSTouchRpcSocketClient client = (WSTouchRpcSocketClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnStreamTransfered), client, e))
            {
                return;
            }
            OnStreamTransfered(client, e);
        }

        private void OnRpcServiceStreamTransfering(RpcActor actor, StreamOperationEventArgs e)
        {
            WSTouchRpcSocketClient client = (WSTouchRpcSocketClient)actor.Caller;
            if (UsePlugin && PluginsManager.Raise<ITouchRpcPlugin>(nameof(ITouchRpcPlugin.OnStreamTransfering), client, e))
            {
                return;
            }
            OnStreamTransfering(client, e);
        }

        private void PrivateDisconnected(WSTouchRpcSocketClient client, DisconnectEventArgs e)
        {
            if (TryRemove(client.ID, out _))
            {
                if (UsePlugin && PluginsManager.Raise<ITcpPlugin>(nameof(ITcpPlugin.OnDisconnected), client, e))
                {
                    return;
                }
                OnDisconnected(client, e);
            }
        }

        private void RpcServiceOutputSend(RpcActor actor, ArraySegment<byte>[] arg3)
        {
            WSTouchRpcSocketClient client = (WSTouchRpcSocketClient)actor.Caller;
            if (!client.CanSend)
            {
                return;
            }
            client.RpcActorSend(arg3);
        }

        #endregion 内部委托绑定

        /// <summary>
        /// 从WebSocket获取新客户端。
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public async Task SwitchClientAsync(System.Net.WebSockets.WebSocket webSocket)
        {
            string id = GetDefaultNewID();
            WSTouchRpcSocketClient client = new WSTouchRpcSocketClient();
            if (!TryAdd(id, client))
            {
                throw new Exception("ID重复");
            }
            client.m_service = this;
            client.m_usePlugin = UsePlugin;
            CheckService();

            client.SetRpcActor(m_rpcActorGroup.CreateRpcActor(client));
            client.RpcActor.ID = id;
            client.m_internalDisconnected = PrivateDisconnected;
            await client.Start(Config, webSocket);
        }

        private void CheckService()
        {
            if (m_rpcActorGroup == null)
            {
                throw new Exception($"{nameof(RpcActorGroup)}未在{nameof(RpcStore)}中注册为解析器。");
            }
        }

        #region 事件

        /// <summary>
        /// 客户端断开。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnDisconnected(WSTouchRpcSocketClient client, DisconnectEventArgs e)
        {
        }

        /// <summary>
        /// 当文件传输结束之后。并不意味着完成传输，请通过<see cref="FileTransferStatusEventArgs.Result"/>属性值进行判断。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnFileTransfered(WSTouchRpcSocketClient client, FileTransferStatusEventArgs e)
        {
        }

        /// <summary>
        /// 在文件传输即将进行时触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnFileTransfering(WSTouchRpcSocketClient client, FileOperationEventArgs e)
        {
        }

        /// <summary>
        /// 在完成握手连接时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnHandshaked(WSTouchRpcSocketClient client, VerifyOptionEventArgs e)
        {
        }

        /// <summary>
        /// 在需要转发路由包时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnRouting(WSTouchRpcSocketClient client, PackageRouterEventArgs e)
        {

        }

        /// <summary>
        /// 在验证Token时
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        protected virtual void OnHandshaking(WSTouchRpcSocketClient client, VerifyOptionEventArgs e)
        {
        }

        /// <summary>
        /// 接收到协议数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="protocol"></param>
        /// <param name="byteBlock"></param>
        protected virtual void OnReceived(WSTouchRpcSocketClient client, short protocol, ByteBlock byteBlock)
        {
        }

        /// <summary>
        /// 流数据处理，用户需要在此事件中对e.Bucket手动释放。覆盖父类方法将不会触发事件和插件。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnStreamTransfered(WSTouchRpcSocketClient client, StreamStatusEventArgs e)
        {
        }

        /// <summary>
        /// 即将接收流数据，用户需要在此事件中对e.Bucket初始化。覆盖父类方法将不会触发事件和插件。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnStreamTransfering(WSTouchRpcSocketClient client, StreamOperationEventArgs e)
        {
        }

        #endregion 事件

        #region 小文件
        /// <inheritdoc/>
        public PullSmallFileResult PullSmallFile(string targetId, string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
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
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
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
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
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
            if (TryGetSocketClient(targetId, out WSTouchRpcSocketClient client))
            {
                return client.PushSmallFileAsync(savePath, fileInfo, metadata, timeout, token);
            }
            else
            {
                return Task.FromResult(new Result(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription(targetId)));
            }
        }
        #endregion

        #region 发送

        /// <inheritdoc/>
        public void Send(string id, short protocol, byte[] buffer, int offset, int length)
        {
            if (TryGetSocketClient(id, out WSTouchRpcSocketClient client))
            {
                client.Send(protocol, buffer, offset, length);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(id));
            }
        }

        /// <inheritdoc/>
        public Task SendAsync(string id, short protocol, byte[] buffer, int offset, int length)
        {
            if (TryGetSocketClient(id, out WSTouchRpcSocketClient client))
            {
                return client.SendAsync(protocol, buffer, offset, length);
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(id));
            }
        }

        #endregion 发送
    }
}