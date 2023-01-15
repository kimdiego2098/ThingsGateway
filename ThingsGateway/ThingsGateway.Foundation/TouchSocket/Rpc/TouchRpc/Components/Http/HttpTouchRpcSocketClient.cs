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
    /// Http服务器辅助类
    /// </summary>
    public partial class HttpTouchRpcSocketClient : HttpSocketClient, IHttpTouchRpcSocketClient
    {
        internal Action<HttpTouchRpcSocketClient> m_internalOnRpcActorInit;
        private RpcActor m_rpcActor;

        /// <inheritdoc/>
        public bool IsHandshaked => m_rpcActor == null ? false : m_rpcActor.IsHandshaked;

        /// <inheritdoc/>
        public string RootPath { get => m_rpcActor.RootPath; set => m_rpcActor.RootPath = value; }

        /// <inheritdoc/>
        public RpcActor RpcActor => m_rpcActor;

        /// <inheritdoc/>
        public SerializationSelector SerializationSelector => m_rpcActor.SerializationSelector;

        /// <inheritdoc/>
        public Func<IRpcClient, bool> TryCanInvoke { get; set; }

        /// <summary>
        /// 连接令箭
        /// </summary>
        public string VerifyToken => Config.GetValue<string>(TouchRpcConfigExtensions.VerifyTokenProperty);

        /// <inheritdoc/>
        public bool ChannelExisted(int id)
        {
            return m_rpcActor.ChannelExisted(id);
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
        public void Invoke(string id, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            m_rpcActor.Invoke(id, method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public T Invoke<T>(string id, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.Invoke<T>(id, method, invokeOption, parameters);
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
        public Task InvokeAsync(string id, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.InvokeAsync(id, method, invokeOption, parameters);
        }

        /// <inheritdoc/>
        public Task<T> InvokeAsync<T>(string id, string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.InvokeAsync<T>(id, method, invokeOption, parameters);
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
        public override void ResetID(string newID)
        {
            m_rpcActor.ResetID(newID);
            DirectResetID(newID);
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

        internal void RpcActorSend(ArraySegment<byte>[] transferBytes)
        {
            base.Send(transferBytes);
        }

        internal void SetRpcActor(RpcActor rpcActor)
        {
            m_rpcActor = rpcActor;
            m_rpcActor.OnResetID = ThisOnResetID;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            m_rpcActor.SafeDispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="byteBlock"></param>
        /// <param name="requestInfo"></param>
        protected override void HandleReceivedData(ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            if (Protocol == TouchRpcUtility.TouchRpcProtocol && byteBlock != null)
            {
                m_rpcActor.InputReceivedData(byteBlock);
            }
            else
            {
                base.HandleReceivedData(byteBlock, requestInfo);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDisconnected(DisconnectEventArgs e)
        {
            m_rpcActor?.Close(e.Message);
            base.OnDisconnected(e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="request"></param>
        protected override void OnReceivedHttpRequest(HttpRequest request)
        {
            if (request.Method == TouchRpcUtility.TouchRpc)
            {
                request.SafeDispose();
                InitRpcActor();
                this.DefaultSend(new HttpResponse().SetStatus().BuildAsBytes());
                return;
            }
            base.OnReceivedHttpRequest(request);
        }

        private void InitRpcActor()
        {
            this.SwitchProtocolToTouchRpc();
            m_internalOnRpcActorInit?.Invoke(this);
            m_rpcActor.ID = ID;
        }

        private void ThisOnResetID(bool first, RpcActor rpcActor, WaitSetID waitSetID)
        {
            DirectResetID(waitSetID.NewID);
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
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
    }
}