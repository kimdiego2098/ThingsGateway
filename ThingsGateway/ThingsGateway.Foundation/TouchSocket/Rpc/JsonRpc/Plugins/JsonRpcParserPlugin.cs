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

using System.Collections;
using System.Collections.Generic;



using ThingsGateway.Foundation.Http.WebSockets;

namespace ThingsGateway.Foundation.Rpc.JsonRpc
{
    /// <summary>
    /// JsonRpcParser解析器插件
    /// </summary>
    public class JsonRpcParserPlugin : WebSocketPluginBase, IRpcParser
    {
        private readonly ActionMap m_actionMap;
        private string m_jsonRpcUrl = "/jsonrpc";
        private RpcStore m_rpcStore;

        /// <summary>
        /// 构造函数
        /// </summary>
        public JsonRpcParserPlugin([DependencyParamterInject(true)] RpcStore rpcStore)
        {
            m_actionMap = new ActionMap();
            rpcStore?.AddRpcParser(GetType().Name, this);
        }

        /// <summary>
        /// JsonRpc的调用键。
        /// </summary>
        public ActionMap ActionMap => m_actionMap;

        /// <summary>
        /// 自动转换协议
        /// </summary>
        public bool AutoSwitch { get; set; } = true;

        /// <summary>
        /// 当挂载在<see cref="HttpService"/>时，匹配Url然后响应。当设置为null或空时，会全部响应。
        /// </summary>
        public string JsonRpcUrl
        {
            get => m_jsonRpcUrl;
            set => m_jsonRpcUrl = string.IsNullOrEmpty(value) ? "/" : value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public RpcStore RpcStore => m_rpcStore;

        /// <summary>
        /// 不需要自动转化协议。
        /// <para>仅当服务器是TCP时生效。此时如果携带协议为TcpJsonRpc时才会解释为jsonRpc。</para>
        /// </summary>
        /// <returns></returns>
        public JsonRpcParserPlugin NoSwitchProtocol()
        {
            AutoSwitch = false;
            return this;
        }

        #region RPC解析器

        void IRpcParser.OnRegisterServer(MethodInstance[] methodInstances)
        {
            foreach (var methodInstance in methodInstances)
            {
                if (methodInstance.GetAttribute<JsonRpcAttribute>() is JsonRpcAttribute attribute)
                {
                    m_actionMap.Add(attribute.GetInvokenKey(methodInstance), methodInstance);
                }
            }
        }

        void IRpcParser.OnUnregisterServer(MethodInstance[] methodInstances)
        {
            foreach (var methodInstance in methodInstances)
            {
                if (methodInstance.GetAttribute<JsonRpcAttribute>() is JsonRpcAttribute attribute)
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

        /// <summary>
        /// 当挂载在<see cref="HttpService"/>时，匹配Url然后响应。当设置为null或空时，会全部响应。
        /// </summary>
        /// <param name="jsonRpcUrl"></param>
        /// <returns></returns>
        public JsonRpcParserPlugin SetJsonRpcUrl(string jsonRpcUrl)
        {
            JsonRpcUrl = jsonRpcUrl;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnConnecting(ITcpClientBase client, OperationEventArgs e)
        {
            if (AutoSwitch && client.Protocol == Protocol.TCP)
            {
                client.SwitchProtocolToTcpJsonRpc();
            }
            base.OnConnecting(client, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnHandleWSDataFrame(ITcpClientBase client, WSDataFrameEventArgs e)
        {
            if (e.DataFrame.Opcode == WSDataType.Text)
            {
                string jsonRpcStr = e.DataFrame.ToText();
                if (jsonRpcStr.Contains("jsonrpc"))
                {
                    e.Handled = true;
                    ThreadPool.QueueUserWorkItem(InvokeWaitCallback, new JsonRpcCallContext()
                    {
                        Caller = client,
                        JRPT = JRPT.Websocket,
                        JsonString = jsonRpcStr
                    });
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnPost(ITcpClientBase client, HttpContextEventArgs e)
        {
            if (m_jsonRpcUrl == "/" || e.Context.Request.UrlEquals(m_jsonRpcUrl))
            {
                e.Handled = true;
                ThreadPool.QueueUserWorkItem(InvokeWaitCallback, new JsonRpcCallContext()
                {
                    Caller = client,
                    JRPT = JRPT.Http,
                    JsonString = e.Context.Request.GetBody(),
                    HttpContext = e.Context
                });
            }
            base.OnPost(client, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnReceivedData(ITcpClientBase client, ReceivedDataEventArgs e)
        {
            if (client.Protocol == JsonRpcConfigExtensions.TcpJsonRpc)
            {
                string jsonRpcStr = e.ByteBlock.ToString();
                if (jsonRpcStr.Contains("jsonrpc"))
                {
                    e.Handled = true;
                    ThreadPool.QueueUserWorkItem(InvokeWaitCallback, new JsonRpcCallContext()
                    {
                        Caller = client,
                        JRPT = JRPT.Tcp,
                        JsonString = e.ByteBlock.ToString()
                    });
                }
            }
            base.OnReceivedData(client, e);
        }

        private static void Response(JsonRpcCallContext callContext, object result, error error)
        {
            try
            {
                using (ByteBlock responseByteBlock = new ByteBlock())
                {
                    object jobject = null;
                    if (error == null)
                    {
                        jobject = new JsonRpcSuccessResponse
                        {
                            result = result,
                            id = callContext.JsonRpcContext.id
                        };
                    }
                    else
                    {
                        jobject = new JsonRpcErrorResponse
                        {
                            error = error,
                            id = callContext.JsonRpcContext.id
                        };
                    }
                    ITcpClientBase client = (ITcpClientBase)callContext.Caller;
                    if (callContext.JRPT == JRPT.Http)
                    {
                        HttpResponse httpResponse = callContext.HttpContext.Response;
                        httpResponse.FromJson(jobject.ToJson());
                        httpResponse.Answer();
                    }
                    else if (callContext.JRPT == JRPT.Websocket)
                    {
                        ((HttpSocketClient)client).SendWithWS(jobject.ToJson());
                    }
                    else
                    {
                        responseByteBlock.Write(jobject.ToJson().ToUTF8Bytes());
                        client.Send(responseByteBlock);
                    }
                }
            }
            catch
            {
            }
        }

        private void BuildRequestContext(ref JsonRpcCallContext callContext)
        {
            JsonRpcContext jsonRpcContext = SerializeConvert.JsonDeserializeFromString<JsonRpcContext>(callContext.JsonString);
            callContext.JsonRpcContext = jsonRpcContext;
            if (jsonRpcContext.id != null)
            {
                jsonRpcContext.needResponse = true;
            }

            if (m_actionMap.TryGetMethodInstance(jsonRpcContext.method, out MethodInstance methodInstance))
            {
                callContext.MethodInstance = methodInstance;
                if (jsonRpcContext.@params == null)
                {
                    if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
                    {
                        if (methodInstance.ParameterNames.Length > 1)
                        {
                            throw new RpcException("调用参数计数不匹配");
                        }
                        else
                        {
                            jsonRpcContext.parameters = new object[] { callContext };
                        }
                    }
                    else
                    {
                        if (methodInstance.ParameterNames.Length != 0)
                        {
                            throw new RpcException("调用参数计数不匹配");
                        }
                    }
                }
                if (jsonRpcContext.@params is Dictionary<string, object> obj)
                {
                    jsonRpcContext.parameters = new object[methodInstance.ParameterNames.Length];
                    //内联
                    int i = 0;
                    if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
                    {
                        jsonRpcContext.parameters[0] = callContext;
                        i = 1;
                    }
                    for (; i < methodInstance.ParameterNames.Length; i++)
                    {
                        if (obj.TryGetValue(methodInstance.ParameterNames[i], out object jToken))
                        {
                            Type type = methodInstance.ParameterTypes[i];
                            jsonRpcContext.parameters[i] = jToken.ToJson().FromJson(type);
                        }
                        else if (methodInstance.Parameters[i].HasDefaultValue)
                        {
                            jsonRpcContext.parameters[i] = methodInstance.Parameters[i].DefaultValue;
                        }
                        else
                        {
                            throw new RpcException("调用参数计数不匹配");
                        }
                    }
                }
                else
                {
                    IList array = (IList)jsonRpcContext.@params;
                    if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
                    {
                        if (array.Count != methodInstance.ParameterNames.Length - 1)
                        {
                            throw new RpcException("调用参数计数不匹配");
                        }
                        jsonRpcContext.parameters = new object[methodInstance.ParameterNames.Length];

                        jsonRpcContext.parameters[0] = callContext;
                        for (int i = 0; i < array.Count; i++)
                        {
                            jsonRpcContext.parameters[i + 1] = array[i].ToJson().FromJson(methodInstance.ParameterTypes[i + 1]);
                        }
                    }
                    else
                    {
                        if (array.Count != methodInstance.ParameterNames.Length)
                        {
                            throw new RpcException("调用参数计数不匹配");
                        }
                        jsonRpcContext.parameters = new object[methodInstance.ParameterNames.Length];

                        for (int i = 0; i < array.Count; i++)
                        {
                            jsonRpcContext.parameters[i] = array[i].ToJson().FromJson(methodInstance.ParameterTypes[i]);
                        }
                    }
                }
            }
        }

        private void InvokeWaitCallback(object context)
        {
            if (context is null)
            {
                return;
            }
            JsonRpcCallContext callContext = (JsonRpcCallContext)context;
            InvokeResult invokeResult = new InvokeResult();

            try
            {
                BuildRequestContext(ref callContext);
            }
            catch (Exception ex)
            {
                invokeResult.Status = InvokeStatus.Exception;
                invokeResult.Message = ex.Message;
            }

            if (callContext.MethodInstance != null)
            {
                if (!callContext.MethodInstance.IsEnable)
                {
                    invokeResult.Status = InvokeStatus.UnEnable;
                }
            }
            else
            {
                invokeResult.Status = InvokeStatus.UnFound;
            }

            if (invokeResult.Status == InvokeStatus.Ready)
            {
                IRpcServer rpcServer = callContext.MethodInstance.ServerFactory.Create(callContext, callContext.JsonRpcContext.parameters);
                if (rpcServer is ITransientRpcServer transientRpcServer)
                {
                    transientRpcServer.CallContext = callContext;
                }

                invokeResult = m_rpcStore.Execute(rpcServer, callContext.JsonRpcContext.parameters, callContext);
            }

            if (!callContext.JsonRpcContext.needResponse)
            {
                return;
            }
            error error = null;
            switch (invokeResult.Status)
            {
                case InvokeStatus.Success:
                    {
                        break;
                    }
                case InvokeStatus.UnFound:
                    {
                        error = new error();
                        error.code = -32601;
                        error.message = "函数未找到";
                        break;
                    }
                case InvokeStatus.UnEnable:
                    {
                        error = new error();
                        error.code = -32601;
                        error.message = "函数已被禁用";
                        break;
                    }
                case InvokeStatus.InvocationException:
                    {
                        error = new error();
                        error.code = -32603;
                        error.message = "函数内部异常";
                        break;
                    }
                case InvokeStatus.Exception:
                    {
                        error = new error();
                        error.code = -32602;
                        error.message = invokeResult.Message;
                        break;
                    }
                default:
                    return;
            }

            Response(callContext, invokeResult.Result, error);
        }
    }
}