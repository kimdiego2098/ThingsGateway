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
using System.Net.Sockets;

using System.Threading.Tasks;

using ThingsGateway.Foundation.Http.WebSockets;
using ThingsGateway.Foundation.Rpc.TouchRpc;

namespace ThingsGateway.Foundation.Rpc.JsonRpc
{
    /// <summary>
    /// JsonRpc客户端
    /// </summary>
    public class JsonRpcClient : DisposableObject, ITcpClient, IJsonRpcClient
    {
        private readonly WaitHandlePool<IWaitResult> m_waitHandle;
        private JRPT m_jrpt;

        /// <summary>
        /// 构造函数
        /// </summary>
        public JsonRpcClient()
        {
            m_waitHandle = new WaitHandlePool<IWaitResult>();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int BufferLength => Client.BufferLength;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool CanSend => Client.CanSend;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool CanSetDataHandlingAdapter => Client.CanSetDataHandlingAdapter;

        /// <summary>
        /// 内部客户端
        /// </summary>
        public ITcpClient Client { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TouchSocketConfig Config => Client.Config;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public MessageEventHandler<ITcpClient> Connected { get => Client.Connected; set => Client.Connected = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ConnectingEventHandler<ITcpClient> Connecting { get => Client.Connecting; set => Client.Connecting = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IContainer Container => ((IClient)Client).Container;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DataHandlingAdapter DataHandlingAdapter => Client.DataHandlingAdapter;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DisconnectEventHandler<ITcpClientBase> Disconnected { get => Client.Disconnected; set => Client.Disconnected = value; }

        /// <inheritdoc/>
        public DisconnectEventHandler<ITcpClientBase> Disconnecting { get => Client.Disconnecting; set => Client.Disconnecting = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string IP => Client.IP;

        /// <inheritdoc/>
        public bool IsClient => Client.IsClient;

        /// <summary>
        /// 协议类型
        /// </summary>
        public JRPT JRPT => m_jrpt;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime LastReceivedTime => Client.LastReceivedTime;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime LastSendTime => Client.LastSendTime;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ILog Logger => Client.Logger;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Socket MainSocket => Client.MainSocket;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Func<ByteBlock, bool> OnHandleRawBuffer { get => Client.OnHandleRawBuffer; set => Client.OnHandleRawBuffer = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Func<ByteBlock, IRequestInfo, bool> OnHandleReceivedData { get => Client.OnHandleReceivedData; set => Client.OnHandleReceivedData = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool Online => Client.Online;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IPluginsManager PluginsManager => Client.PluginsManager;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Port => Client.Port;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Protocol Protocol { get => Client.Protocol; set => Client.Protocol = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReceiveType ReceiveType => Client.ReceiveType;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IPHost RemoteIPHost => Client.RemoteIPHost;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Func<IRpcClient, bool> TryCanInvoke { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool UsePlugin => Client.UsePlugin;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool UseSsl => Client.UseSsl;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Close()
        {
            Client.Close();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Close(string msg)
        {
            Client.Close(msg);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ITcpClient Connect(int timeout = 5000)
        {
            Client.OnHandleReceivedData = HandleReceivedData;
            Client.Connect(timeout);
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
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
        public void DefaultSend(byte[] buffer, int offset, int length)
        {
            Client.DefaultSend(buffer, offset, length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task DefaultSendAsync(byte[] buffer, int offset, int length)
        {
            return Client.DefaultSendAsync(buffer, offset, length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Stream GetStream()
        {
            return Client.GetStream();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TValue GetValue<TValue>(IDependencyProperty<TValue> dp)
        {
            return Client.GetValue(dp);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Send(byte[] buffer, int offset, int length)
        {
            Client.Send(buffer, offset, length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Send(IRequestInfo requestInfo)
        {
            Client.Send(requestInfo);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Send(IList<ArraySegment<byte>> transferBytes)
        {
            Client.Send(transferBytes);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task SendAsync(byte[] buffer, int offset, int length)
        {
            return Client.SendAsync(buffer, offset, length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task SendAsync(IRequestInfo requestInfo)
        {
            return Client.SendAsync(requestInfo);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task SendAsync(IList<ArraySegment<byte>> transferBytes)
        {
            return Client.SendAsync(transferBytes);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void SetDataHandlingAdapter(DataHandlingAdapter adapter)
        {
            Client.SetDataHandlingAdapter(adapter);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ITcpClient Setup(TouchSocketConfig config)
        {
            m_jrpt = config.GetValue<JRPT>(JsonRpcConfigExtensions.JRPTProperty);
            switch (m_jrpt)
            {
                case JRPT.Http:
                    Client ??= new HttpClient();
                    break;

                case JRPT.Websocket:
                    Client ??= new WebSocketClient();
                    break;

                case JRPT.Tcp:
                default:
                    Client ??= new Sockets.TcpClient();
                    break;
            }
            return Client.Setup(config);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ITcpClient Setup(string ipHost)
        {
            TouchSocketConfig config = new TouchSocketConfig();
            config.SetRemoteIPHost(new IPHost(ipHost));
            return Setup(config);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DependencyObject SetValue<TValue>(IDependencyProperty<TValue> dp, TValue value)
        {
            return Client.SetValue(dp, value);
        }

        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="byteBlock"></param>
        /// <param name="requestInfo"></param>
        private bool HandleReceivedData(ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            string jsonString = null;
            switch (m_jrpt)
            {
                case JRPT.Http:
                    {
                        HttpResponse httpResponse = (HttpResponse)requestInfo;
                        jsonString = httpResponse.GetBody();
                        break;
                    }
                case JRPT.Websocket:
                    {
                        if (requestInfo is WSDataFrame dataFrame && dataFrame.Opcode == WSDataType.Text)
                        {
                            jsonString = dataFrame.ToText();
                        }
                        break;
                    }
                case JRPT.Tcp:
                default:
                    {
                        if (byteBlock == null)
                        {
                            return true;
                        }
                        else
                        {
                            jsonString = byteBlock.ToString();
                        }
                        break;
                    }
            }

            if (string.IsNullOrEmpty(jsonString))
            {
                return true;
            }

            try
            {
                if (jsonString.Contains("error") || jsonString.Contains("result"))
                {
                    JsonResponseContext responseContext = jsonString.FromJson<JsonResponseContext>();
                    if (responseContext != null && !responseContext.id.IsNullOrEmpty())
                    {
                        JsonRpcWaitResult waitContext = new JsonRpcWaitResult
                        {
                            Status = 1,
                            Sign = long.Parse(responseContext.id),
                            error = responseContext.error,
                            Return = responseContext.result
                        };
                        m_waitHandle.SetRun(waitContext);
                    }
                }
            }
            catch
            {
            }
            return true;
        }

        #region RPC调用

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public bool HasValue<TValue>(IDependencyProperty<TValue> dp)
        {
            return Client.HasValue(dp);
        }

        /// <summary>
        /// Rpc调用
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="invokeOption">调用配置</param>
        /// <param name="parameters">参数</param>
        /// <param name="types"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="RpcException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public T Invoke<T>(string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            JsonRpcWaitResult context = new JsonRpcWaitResult();
            WaitData<IWaitResult> waitData = m_waitHandle.GetWaitData(context);

            using (ByteBlock byteBlock = BytePool.GetByteBlock(BufferLength))
            {
                if (invokeOption == default)
                {
                    invokeOption = InvokeOption.WaitInvoke;
                }

                parameters ??= new object[0];
                JsonRpcRequest jsonRpcRequest = new JsonRpcRequest()
                {
                    method = method,
                    @params = parameters
                };

                if (invokeOption.FeedbackType == FeedbackType.WaitInvoke)
                {
                    jsonRpcRequest.id = context.Sign.ToString();
                }
                else
                {
                    jsonRpcRequest.id = null;
                }
                switch (m_jrpt)
                {
                    case JRPT.Tcp:
                        {
                            byteBlock.Write(SerializeConvert.JsonSerializeToBytes(jsonRpcRequest));
                            break;
                        }
                    case JRPT.Http:
                        {
                            HttpRequest request = new HttpRequest();
                            request.Method = "POST";
                            request.SetUrl(RemoteIPHost.GetUrlPath());
                            request.FromJson(jsonRpcRequest.ToJson());
                            request.Build(byteBlock);
                            break;
                        }
                    case JRPT.Websocket:
                        {
                            ((WebSocketClient)Client).SendWithWS(jsonRpcRequest.ToJson());
                            break;
                        }
                }
                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            this.Send(byteBlock);
                            m_waitHandle.Destroy(waitData);
                            return default;
                        }
                    case FeedbackType.WaitSend:
                        {
                            this.Send(byteBlock);
                            m_waitHandle.Destroy(waitData);
                            return default;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            this.Send(byteBlock);
                            waitData.Wait(invokeOption.Timeout);
                            JsonRpcWaitResult resultContext = (JsonRpcWaitResult)waitData.WaitResult;
                            m_waitHandle.Destroy(waitData);

                            if (resultContext.Status == 0)
                            {
                                throw new TimeoutException("等待结果超时");
                            }
                            if (resultContext.error != null)
                            {
                                throw new RpcException(resultContext.error.message);
                            }

                            if (resultContext.Return == null)
                            {
                                return default;
                            }
                            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
                            {
                                return (T)resultContext.Return.ToString().ParseToType(typeof(T));
                            }

                            return resultContext.Return.ToJson().FromJson<T>();
                        }
                    default:
                        return default;
                }
            }
        }

        /// <summary>
        /// Rpc调用
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="invokeOption">调用配置</param>
        /// <param name="parameters">参数</param>
        /// <param name="types"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="RpcException"></exception>
        /// <exception cref="Exception"></exception>
        public void Invoke(string method, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            JsonRpcWaitResult context = new JsonRpcWaitResult();
            WaitData<IWaitResult> waitData = m_waitHandle.GetWaitData(context);

            using (ByteBlock byteBlock = BytePool.GetByteBlock(BufferLength))
            {
                if (invokeOption == default)
                {
                    invokeOption = InvokeOption.WaitInvoke;
                }
                parameters ??= new object[0];
                JsonRpcRequest jsonRpcRequest = new JsonRpcRequest()
                {
                    method = method,
                    @params = parameters
                };

                if (invokeOption.FeedbackType == FeedbackType.WaitInvoke)
                {
                    jsonRpcRequest.id = context.Sign.ToString();
                }
                else
                {
                    jsonRpcRequest.id = null;
                }
                switch (m_jrpt)
                {
                    case JRPT.Tcp:
                        {
                            byteBlock.Write(SerializeConvert.JsonSerializeToBytes(jsonRpcRequest));
                            break;
                        }
                    case JRPT.Websocket:
                        {
                            ((WebSocketClient)Client).SendWithWS(jsonRpcRequest.ToJson());
                            break;
                        }
                    case JRPT.Http:
                        {
                            HttpRequest request = new HttpRequest();
                            request.Method = "POST";
                            request.SetUrl(RemoteIPHost.GetUrlPath());
                            request.FromJson(jsonRpcRequest.ToJson());
                            request.Build(byteBlock);
                        }
                        break;
                }
                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            this.Send(byteBlock);
                            m_waitHandle.Destroy(waitData);
                            return;
                        }
                    case FeedbackType.WaitSend:
                        {
                            this.Send(byteBlock);
                            m_waitHandle.Destroy(waitData);
                            return;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            this.Send(byteBlock);
                            waitData.Wait(invokeOption.Timeout);
                            JsonRpcWaitResult resultContext = (JsonRpcWaitResult)waitData.WaitResult;
                            m_waitHandle.Destroy(waitData);

                            if (resultContext.Status == 0)
                            {
                                throw new TimeoutException("等待结果超时");
                            }
                            if (resultContext.error != null)
                            {
                                throw new RpcException(resultContext.error.message);
                            }
                            break;
                        }
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Rpc调用
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="invokeOption">调用配置</param>
        /// <param name="parameters">参数</param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="RpcException"></exception>
        /// <exception cref="Exception"></exception>
        public void Invoke(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            Invoke(method, invokeOption, ref parameters, null);
        }

        /// <summary>
        /// Rpc调用
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="invokeOption">调用配置</param>
        /// <param name="parameters">参数</param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="RpcException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public T Invoke<T>(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return Invoke<T>(method, invokeOption, ref parameters, null);
        }

        /// <summary>
        /// 函数式调用
        /// </summary>
        /// <param name="method">函数名</param>
        /// <param name="parameters">参数</param>
        /// <param name="invokeOption">Rpc调用设置</param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="RpcException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task InvokeAsync(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            JsonRpcWaitResult context = new JsonRpcWaitResult();
            WaitData<IWaitResult> waitData = m_waitHandle.GetWaitData(context);

            using (ByteBlock byteBlock = BytePool.GetByteBlock(BufferLength))
            {
                if (invokeOption == default)
                {
                    invokeOption = InvokeOption.WaitInvoke;
                }
                parameters ??= new object[0];
                JsonRpcRequest jsonRpcRequest = new JsonRpcRequest()
                {
                    method = method,
                    @params = parameters
                };

                if (invokeOption.FeedbackType == FeedbackType.WaitInvoke)
                {
                    jsonRpcRequest.id = context.Sign.ToString();
                }
                else
                {
                    jsonRpcRequest.id = null;
                }
                switch (m_jrpt)
                {
                    case JRPT.Tcp:
                        {
                            byteBlock.Write(SerializeConvert.JsonSerializeToBytes(jsonRpcRequest));
                            break;
                        }
                    case JRPT.Websocket:
                        {
                            ((WebSocketClient)Client).SendWithWS(jsonRpcRequest.ToJson());
                            break;
                        }
                    case JRPT.Http:
                        {
                            HttpRequest request = new HttpRequest();
                            request.Method = "POST";
                            request.SetUrl(RemoteIPHost.GetUrlPath());
                            request.FromJson(jsonRpcRequest.ToJson());
                            request.Build(byteBlock);
                        }
                        break;
                }
                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            this.Send(byteBlock);
                            m_waitHandle.Destroy(waitData);
                            return;
                        }
                    case FeedbackType.WaitSend:
                        {
                            this.Send(byteBlock);
                            m_waitHandle.Destroy(waitData);
                            return;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            this.Send(byteBlock);
                            await waitData.WaitAsync(invokeOption.Timeout);
                            JsonRpcWaitResult resultContext = (JsonRpcWaitResult)waitData.WaitResult;
                            m_waitHandle.Destroy(waitData);

                            if (resultContext.Status == 0)
                            {
                                throw new TimeoutException("等待结果超时");
                            }
                            if (resultContext.error != null)
                            {
                                throw new RpcException(resultContext.error.message);
                            }
                            break;
                        }
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// 函数式调用
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="parameters">参数</param>
        /// <param name="invokeOption">Rpc调用设置</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcException">Rpc异常</exception>
        /// <exception cref="Exception">其他异常</exception>
        /// <returns>服务器返回结果</returns>
        public async Task<T> InvokeAsync<T>(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            JsonRpcWaitResult context = new JsonRpcWaitResult();
            WaitData<IWaitResult> waitData = m_waitHandle.GetWaitData(context);

            using (ByteBlock byteBlock = BytePool.GetByteBlock(BufferLength))
            {
                if (invokeOption == default)
                {
                    invokeOption = InvokeOption.WaitInvoke;
                }
                parameters ??= new object[0];
                JsonRpcRequest jsonRpcRequest = new JsonRpcRequest()
                {
                    method = method,
                    @params = parameters
                };

                if (invokeOption.FeedbackType == FeedbackType.WaitInvoke)
                {
                    jsonRpcRequest.id = context.Sign.ToString();
                }
                else
                {
                    jsonRpcRequest.id = null;
                }
                switch (m_jrpt)
                {
                    case JRPT.Tcp:
                        {
                            byteBlock.Write(SerializeConvert.JsonSerializeToBytes(jsonRpcRequest));
                            break;
                        }
                    case JRPT.Http:
                        {
                            HttpRequest request = new HttpRequest();
                            request.Method = "POST";
                            request.SetUrl(RemoteIPHost.GetUrlPath());
                            request.FromJson(jsonRpcRequest.ToJson());
                            request.Build(byteBlock);
                            break;
                        }
                    case JRPT.Websocket:
                        {
                            ((WebSocketClient)Client).SendWithWS(jsonRpcRequest.ToJson());
                            break;
                        }
                }
                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            this.Send(byteBlock);
                            m_waitHandle.Destroy(waitData);
                            return default;
                        }
                    case FeedbackType.WaitSend:
                        {
                            this.Send(byteBlock);
                            m_waitHandle.Destroy(waitData);
                            return default;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            this.Send(byteBlock);
                            await waitData.WaitAsync(invokeOption.Timeout);
                            JsonRpcWaitResult resultContext = (JsonRpcWaitResult)waitData.WaitResult;
                            m_waitHandle.Destroy(waitData);

                            if (resultContext.Status == 0)
                            {
                                throw new TimeoutException("等待结果超时");
                            }
                            if (resultContext.error != null)
                            {
                                throw new RpcException(resultContext.error.message);
                            }

                            if (resultContext.Return == null)
                            {
                                return default;
                            }
                            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
                            {
                                return (T)resultContext.Return.ToString().ParseToType(typeof(T));
                            }

                            return resultContext.Return.ToJson().FromJson<T>();
                        }
                    default:
                        return default;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dp"></param>
        /// <returns></returns>
        public DependencyObject RemoveValue<TValue>(IDependencyProperty<TValue> dp)
        {
            return Client.RemoveValue(dp);
        }

        #endregion RPC调用
    }
}