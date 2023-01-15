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
using System.Threading;
using System.Threading.Tasks;

using ThingsGateway.Foundation.Extension;
using ThingsGateway.Foundation.Resources;
using ThingsGateway.Foundation.Rpc;
using ThingsGateway.Foundation.Rpc.TouchRpc;
using ThingsGateway.Foundation.Sockets;

namespace ThingsGateway.Foundation.AspNetCore
{
    /// <summary>
    /// WSTouchRpcSocketClient
    /// </summary>
    public class WSTouchRpcSocketClient : DisposableObject, IRpcActor, IPluginObject, ISenderBase
    {
        internal string m_id;
        internal DisconnectEventHandler<WSTouchRpcSocketClient> m_internalDisconnected;
        internal WSTouchRpcService m_service;
        internal bool m_usePlugin;
        private readonly byte[] m_buffer = new byte[1024 * 64];
        private WebSocket m_client;
        private RpcActor m_rpcActor;

        /// <inheritdoc/>
        public bool CanSend => m_client.State == WebSocketState.Open;

        /// <summary>
        /// 配置
        /// </summary>
        public TouchSocketConfig Config { get; private set; }

        /// <inheritdoc/>
        public IContainer Container => Config?.Container;

        /// <inheritdoc/>
        public string ID => m_rpcActor.ID;

        /// <inheritdoc/>
        public bool IsHandshaked => m_rpcActor.IsHandshaked;

        /// <inheritdoc/>
        public ILog Logger => m_rpcActor.Logger;

        /// <inheritdoc/>
        public IPluginsManager PluginsManager => Config?.PluginsManager;

        /// <inheritdoc/>
        public string RootPath { get => m_rpcActor.RootPath; set => m_rpcActor.RootPath = value; }

        /// <summary>
        /// RpcActor
        /// </summary>
        public RpcActor RpcActor { get => m_rpcActor; }

        /// <inheritdoc/>
        public SerializationSelector SerializationSelector => m_rpcActor.SerializationSelector;

        /// <inheritdoc/>
        public Func<IRpcClient, bool> TryCanInvoke { get => m_rpcActor.TryCanInvoke; set => m_rpcActor.TryCanInvoke = value; }

        /// <summary>
        /// 是否已启动插件
        /// </summary>
        public bool UsePlugin => m_usePlugin;

        /// <inheritdoc/>
        public bool ChannelExisted(int id)
        {
            return m_rpcActor.ChannelExisted(id);
        }

        /// <summary>
        /// 关闭通信
        /// </summary>
        /// <param name="msg"></param>
        public void Close(string msg)
        {
            BreakOut(msg, true);
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
            if (string.IsNullOrEmpty(newId))
            {
                throw new ArgumentException($"“{nameof(newId)}”不能为 null 或空。", nameof(newId));
            }

            if (m_id == newId)
            {
                return;
            }

            m_rpcActor.ResetID(newId);
            DirectResetID(newId);
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

        /// <inheritdoc/>
        public bool TrySubscribeChannel(int id, out Channel channel)
        {
            return m_rpcActor.TrySubscribeChannel(id, out channel);
        }

        internal async void RpcActorSend(ArraySegment<byte>[] transferBytes)
        {
            using ByteBlock byteBlock = new ByteBlock();
            foreach (var item in transferBytes)
            {
                byteBlock.Write(item.Array, item.Offset, item.Count);
            }
            await m_client.SendAsync(byteBlock.Buffer, System.Net.WebSockets.WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        internal void SetRpcActor(RpcActor rpcActor)
        {
            m_rpcActor = rpcActor;
            m_rpcActor.OnResetID = ThisOnResetID;
        }

        internal Task Start(TouchSocketConfig config, System.Net.WebSockets.WebSocket webSocket)
        {
            Config = config;
            m_client = webSocket;
            return BeginReceive();
        }

        /// <summary>
        /// 直接重置Id。
        /// </summary>
        /// <param name="newId"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ClientNotFindException"></exception>
        protected void DirectResetID(string newId)
        {
            string oldId = m_id;
            if (m_service.TryRemove(m_id, out var socketClient))
            {
                socketClient.m_id = newId;
                if (m_service.TryAdd(m_id, socketClient))
                {
                    if (m_usePlugin)
                    {
                        IDChangedEventArgs e = new IDChangedEventArgs(oldId, newId);
                        PluginsManager.Raise<ITcpPlugin>(nameof(ITcpPlugin.OnIDChanged), socketClient, e);
                    }
                    return;
                }
                else
                {
                    socketClient.m_id = oldId;
                    if (m_service.TryAdd(socketClient.m_id, socketClient))
                    {
                        throw new Exception("ID重复");
                    }
                    else
                    {
                        socketClient.Close("修改新ID时操作失败，且回退旧ID时也失败。");
                    }
                }
            }
            else
            {
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(oldId));
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (DisposedValue)
            {
                return;
            }
            base.Dispose(disposing);
            BreakOut($"{nameof(Dispose)}断开链接", true);
        }

        private async Task BeginReceive()
        {
            ByteBlock byteBlock = null;
            try
            {
                while (true)
                {
                    var result = await m_client.ReceiveAsync(m_buffer, default);
                    if (result.Count == 0)
                    {
                        byteBlock.SafeDispose();
                        break;
                    }
                    if (byteBlock == null)
                    {
                        byteBlock = new ByteBlock();
                    }
                    byteBlock.Write(m_buffer, 0, result.Count);
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
                            byteBlock = null;
                        }
                    }
                }

                BreakOut("远程终端主动关闭", false);
            }
            catch (Exception ex)
            {
                BreakOut(ex.Message, false);
            }
        }

        private void BreakOut(string msg, bool manual)
        {
            lock (this)
            {
                if (DisposedValue)
                {
                    return;
                }
                Dispose();
                m_client.SafeDispose();
                m_rpcActor.SafeDispose();
                m_internalDisconnected?.Invoke(this, new DisconnectEventArgs(manual, msg));
            }
        }

        private RpcActor FindRpcActor(string id)
        {
            if (m_service.TryGetSocketClient(id, out WSTouchRpcSocketClient socketClient))
            {
                return socketClient.m_rpcActor;
            }
            return null;
        }

        private void ThisOnResetID(bool first, RpcActor rpcActor, WaitSetID waitSetID)
        {
            DirectResetID(waitSetID.NewID);
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
    }
}