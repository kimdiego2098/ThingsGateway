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
using System.IO;
using System.Net;
using System.Threading
.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// UDP Rpc解释器
    /// </summary>
    public partial class UdpTouchRpc : UdpSessionBase, IUdpTouchRpc
    {
        private readonly ActionMap m_actionMap;
        private readonly Timer m_timer;
        private readonly ConcurrentDictionary<EndPoint, UdpRpcActor> m_udpRpcActors;
        private RpcStore m_rpcStore;
        private SerializationSelector m_serializationSelector;

        /// <summary>
        /// 构造函数
        /// </summary>
        public UdpTouchRpc()
        {
            m_timer = new Timer((obj) =>
            {
                m_udpRpcActors.Remove((kv) =>
                {
                    if (--kv.Value.Tick < 0)
                    {
                        return true;
                    }
                    return false;
                });
            }, null, 1000 * 30, 1000 * 30);
            m_actionMap = new ActionMap();
            m_udpRpcActors = new ConcurrentDictionary<EndPoint, UdpRpcActor>();
        }

        /// <summary>
        /// 方法映射表
        /// </summary>
        public ActionMap ActionMap { get => m_actionMap; }

        /// <summary>
        /// 不需要握手，所以此值一直为True。
        /// </summary>
        public bool IsHandshaked => true;

        string IRpcActor.RootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public RpcActor RpcActor => GetUdpRpcActor();

        /// <inheritdoc/>
        public RpcStore RpcStore => m_rpcStore;

        /// <inheritdoc/>
        public SerializationSelector SerializationSelector => m_serializationSelector;

        /// <inheritdoc/>
        public Func<IRpcClient, bool> TryCanInvoke { get; set; }

        bool IRpcActor.ChannelExisted(int id)
        {
            throw new NotImplementedException();
        }

        Channel IRpcActor.CreateChannel()
        {
            throw new NotImplementedException();
        }

        Channel IRpcActor.CreateChannel(int id)
        {
            throw new NotImplementedException();
        }

        Channel ITargetRpcActor.CreateChannel(string targetId)
        {
            throw new NotImplementedException();
        }

        Channel ITargetRpcActor.CreateChannel(string targetId, int id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Invoke(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            GetUdpRpcActor().Invoke(method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public T Invoke<T>(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return GetUdpRpcActor().Invoke<T>(method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public T Invoke<T>(string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            return GetUdpRpcActor().Invoke<T>(method, invokeOption, ref parameters, types);
        }

        /// <inheritdoc/>
        public void Invoke(string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            GetUdpRpcActor().Invoke(method, invokeOption, ref parameters, types);
        }

        void ITargetRpcClient.Invoke(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        T ITargetRpcClient.Invoke<T>(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        void ITargetRpcClient.Invoke(string targetId, string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            throw new NotImplementedException();
        }

        T ITargetRpcClient.Invoke<T>(string targetId, string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task InvokeAsync(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return GetUdpRpcActor().InvokeAsync(method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public Task<T> InvokeAsync<T>(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return GetUdpRpcActor().InvokeAsync<T>(method, invokeOption, parameters);
        }

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

        void IRpcParser.SetRpcStore(RpcStore rpcService)
        {
            m_rpcStore = rpcService;
        }

        #endregion RPC解析器

        Task ITargetRpcClient.InvokeAsync(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        Task<T> ITargetRpcClient.InvokeAsync<T>(string targetId, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Ping(int timeout = 5000)
        {
            return GetUdpRpcActor().Ping(timeout);
        }

        bool ITargetRpcActor.Ping(string targetId, int timeout)
        {
            throw new NotImplementedException();
        }

        Result IRpcActor.PullFile(FileOperator fileOperator)
        {
            throw new NotImplementedException();
        }

        Result ITargetRpcActor.PullFile(string targetId, FileOperator fileOperator)
        {
            throw new NotImplementedException();
        }

        Task<Result> IRpcActor.PullFileAsync(FileOperator fileOperator)
        {
            throw new NotImplementedException();
        }

        Task<Result> ITargetRpcActor.PullFileAsync(string targetId, FileOperator fileOperator)
        {
            throw new NotImplementedException();
        }

        PullSmallFileResult IRpcActor.PullSmallFile(string path, Metadata metadata, int timeout, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        PullSmallFileResult ITargetRpcActor.PullSmallFile(string targetId, string path, Metadata metadata, int timeout, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        Task<PullSmallFileResult> IRpcActor.PullSmallFileAsync(string path, Metadata metadata, int timeout, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        Task<PullSmallFileResult> ITargetRpcActor.PullSmallFileAsync(string targetId, string path, Metadata metadata, int timeout, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        Result IRpcActor.PushFile(FileOperator fileOperator)
        {
            throw new NotImplementedException();
        }

        Result ITargetRpcActor.PushFile(string targetId, FileOperator fileOperator)
        {
            throw new NotImplementedException();
        }

        Task<Result> IRpcActor.PushFileAsync(FileOperator fileOperator)
        {
            throw new NotImplementedException();
        }

        Task<Result> ITargetRpcActor.PushFileAsync(string targetId, FileOperator fileOperator)
        {
            throw new NotImplementedException();
        }

        Result IRpcActor.PushSmallFile(string savePath, FileInfo fileInfo, Metadata metadata, int timeout, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        Result ITargetRpcActor.PushSmallFile(string targetId, string savePath, FileInfo fileInfo, Metadata metadata, int timeout, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        Task<Result> IRpcActor.PushSmallFileAsync(string savePath, FileInfo fileInfo, Metadata metadata, int timeout, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        Task<Result> ITargetRpcActor.PushSmallFileAsync(string targetId, string savePath, FileInfo fileInfo, Metadata metadata, int timeout, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        void IRpcActor.ResetID(string newID)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Send(short protocol, byte[] buffer, int offset, int length)
        {
            GetUdpRpcActor().Send(protocol, buffer, offset, length);
        }

        /// <inheritdoc/>
        public Task SendAsync(short protocol, byte[] buffer, int offset, int length)
        {
            return GetUdpRpcActor().SendAsync(protocol, buffer, offset, length);
        }

        Result IRpcActor.SendStream(Stream stream, StreamOperator streamOperator, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        Task<Result> IRpcActor.SendStreamAsync(Stream stream, StreamOperator streamOperator, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        bool IRpcActor.TrySubscribeChannel(int id, out Channel channel)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            m_timer.SafeDispose();
            m_udpRpcActors.Clear();
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        protected override void HandleReceivedData(EndPoint remoteEndPoint, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            GetUdpRpcActor(remoteEndPoint).InputReceivedData(byteBlock);
        }

        /// <inheritdoc/>
        protected override void LoadConfig(TouchSocketConfig config)
        {
            m_serializationSelector = config.GetValue(TouchRpcConfigExtensions.SerializationSelectorProperty);

            if (config.GetValue(RpcConfigExtensions.RpcStoreProperty) is RpcStore rpcStore)
            {
                rpcStore.AddRpcParser(GetType().Name, this);
            }
            else
            {
                new RpcStore(config.Container).AddRpcParser(GetType().Name, this);
            }
            base.LoadConfig(config);
        }

        private MethodInstance GetInvokeMethod(string arg)
        {
            return m_actionMap.GetMethodInstance(arg);
        }

        private UdpRpcActor GetUdpRpcActor(EndPoint endPoint)
        {
            if (!m_udpRpcActors.TryGetValue(endPoint, out UdpRpcActor udpRpcActor))
            {
                udpRpcActor = new UdpRpcActor(this, endPoint, Container.Resolve<ILog>())
                {
                    Caller = new UdpCaller(this, endPoint),
                    RpcStore = m_rpcStore,
                    GetInvokeMethod = GetInvokeMethod,
                    SerializationSelector = m_serializationSelector
                };
                m_udpRpcActors.TryAdd(endPoint, udpRpcActor);
            }
            udpRpcActor.Tick++;
            return udpRpcActor;
        }

        private UdpRpcActor GetUdpRpcActor()
        {
            if (RemoteIPHost == null)
            {
                throw new ArgumentNullException(nameof(RemoteIPHost));
            }
            return GetUdpRpcActor(RemoteIPHost.EndPoint);
        }
    }
}