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
using System

.Net.Sockets;

namespace ThingsGateway.Foundation.Rpc.WebApi
{
    /// <summary>
    /// WebApi解析器
    /// </summary>
    public class WebApiParserPlugin : HttpPluginBase, IRpcParser
    {
        private readonly ActionMap m_actionMap;
        private readonly StringConverter m_converter;
        private RpcStore m_rpcStore;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebApiParserPlugin([DependencyParamterInject(true)] RpcStore rpcStore)
        {
            m_actionMap = new ActionMap();
            m_converter = new StringConverter();
            rpcStore?.AddRpcParser(GetType().Name, this);
        }

        /// <summary>
        /// 转化器
        /// </summary>
        public StringConverter Converter => m_converter;

        /// <summary>
        /// 获取路由映射图
        /// </summary>
        public ActionMap RouteMap => m_actionMap;

        /// <summary>
        /// 所属服务器
        /// </summary>
        public RpcStore RpcStore => m_rpcStore;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnGet(ITcpClientBase client, HttpContextEventArgs e)
        {
            if (m_actionMap.TryGetMethodInstance(e.Context.Request.RelativeURL.ToLower(), out MethodInstance methodInstance))
            {
                e.Handled = true;

                InvokeResult invokeResult = new InvokeResult();
                object[] ps = null;
                WebApiCallContext callContext = new WebApiCallContext()
                {
                    Caller = client,
                    HttpContext = e.Context,
                    MethodInstance = methodInstance
                };
                if (methodInstance.IsEnable)
                {
                    try
                    {
                        ps = new object[methodInstance.Parameters.Length];
                        int i = 0;
                        if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
                        {
                            ps[i] = callContext;
                            i++;
                        }
                        if (e.Context.Request.Query == null)
                        {
                            for (; i < methodInstance.Parameters.Length; i++)
                            {
                                ps[i] = methodInstance.ParameterTypes[i].GetDefault();
                            }
                        }
                        else
                        {
                            for (; i < methodInstance.Parameters.Length; i++)
                            {
                                string value = e.Context.Request.Query.Get(methodInstance.ParameterNames[i]);
                                if (!value.IsNullOrEmpty())
                                {
                                    ps[i] = m_converter.ConvertFrom(value, methodInstance.ParameterTypes[i]);
                                }
                                else
                                {
                                    ps[i] = methodInstance.ParameterTypes[i].GetDefault();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        invokeResult.Status = InvokeStatus.Exception;
                        invokeResult.Message = ex.Message;
                    }
                }
                else
                {
                    invokeResult.Status = InvokeStatus.UnEnable;
                }
                if (invokeResult.Status == InvokeStatus.Ready)
                {
                    IRpcServer rpcServer = methodInstance.ServerFactory.Create(callContext, ps);
                    if (rpcServer is ITransientRpcServer transientRpcServer)
                    {
                        transientRpcServer.CallContext = callContext;
                    }
                    invokeResult = m_rpcStore.Execute(rpcServer, ps, callContext);
                }

                if (e.Context.Response.Responsed)
                {
                    return;
                }
                HttpResponse httpResponse = e.Context.Response;
                switch (invokeResult.Status)
                {
                    case InvokeStatus.Success:
                        {
                            httpResponse.FromJson(m_converter.ConvertTo(invokeResult.Result)).SetStatus();
                            break;
                        }
                    case InvokeStatus.UnFound:
                        {
                            string jsonString = m_converter.ConvertTo(new ActionResult() { Status = invokeResult.Status, Message = invokeResult.Message });
                            httpResponse.FromJson(jsonString).SetStatus("404");
                            break;
                        }
                    case InvokeStatus.UnEnable:
                        {
                            string jsonString = m_converter.ConvertTo(new ActionResult() { Status = invokeResult.Status, Message = invokeResult.Message });
                            httpResponse.FromJson(jsonString).SetStatus("405");
                            break;
                        }
                    case InvokeStatus.InvocationException:
                    case InvokeStatus.Exception:
                        {
                            string jsonString = m_converter.ConvertTo(new ActionResult() { Status = invokeResult.Status, Message = invokeResult.Message });
                            httpResponse.FromJson(jsonString).SetStatus("422");
                            break;
                        }
                }

                using (ByteBlock byteBlock = new ByteBlock())
                {
                    httpResponse.Build(byteBlock);
                    client.DefaultSend(byteBlock);
                }

                if (!e.Context.Request.KeepAlive)
                {
                    client.TryShutdown(SocketShutdown.Both);
                }
            }

            base.OnGet(client, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnPost(ITcpClientBase client, HttpContextEventArgs e)
        {
            if (m_actionMap.TryGetMethodInstance(e.Context.Request.RelativeURL.ToLower(), out MethodInstance methodInstance))
            {
                e.Handled = true;

                InvokeResult invokeResult = new InvokeResult();
                object[] ps = null;
                WebApiCallContext callContext = new WebApiCallContext()
                {
                    Caller = client,
                    HttpContext = e.Context,
                    MethodInstance = methodInstance
                };
                if (methodInstance.IsEnable)
                {
                    try
                    {
                        int index;
                        ps = new object[methodInstance.Parameters.Length];
                        int i = 0;
                        if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
                        {
                            ps[i] = callContext;
                            i++;
                            index = methodInstance.Parameters.Length - 2;
                        }
                        else
                        {
                            index = methodInstance.Parameters.Length - 1;
                        }
                        if (e.Context.Request.Query == null)
                        {
                            for (; i < methodInstance.Parameters.Length - 1; i++)
                            {
                                ps[i] = methodInstance.ParameterTypes[i].GetDefault();
                            }
                        }
                        else
                        {
                            for (; i < methodInstance.Parameters.Length - 1; i++)
                            {
                                string value = e.Context.Request.Query.Get(methodInstance.ParameterNames[i]);
                                if (!value.IsNullOrEmpty())
                                {
                                    ps[i] = m_converter.ConvertFrom(value, methodInstance.ParameterTypes[i]);
                                }
                                else
                                {
                                    ps[i] = methodInstance.ParameterTypes[i].GetDefault();
                                }
                            }
                        }

                        if (index >= 0)
                        {
                            string str = e.Context.Request.GetBody();
                            ps[index] = m_converter.ConvertFrom(str, methodInstance.ParameterTypes[index]);
                        }
                    }
                    catch (Exception ex)
                    {
                        invokeResult.Status = InvokeStatus.Exception;
                        invokeResult.Message = ex.Message;
                    }
                }
                else
                {
                    invokeResult.Status = InvokeStatus.UnEnable;
                }
                if (invokeResult.Status == InvokeStatus.Ready)
                {
                    IRpcServer rpcServer = methodInstance.ServerFactory.Create(callContext, ps);
                    if (rpcServer is ITransientRpcServer transientRpcServer)
                    {
                        transientRpcServer.CallContext = callContext;
                    }
                    invokeResult = m_rpcStore.Execute(rpcServer, ps, callContext);
                }

                if (e.Context.Response.Responsed)
                {
                    return;
                }
                HttpResponse httpResponse = e.Context.Response;
                switch (invokeResult.Status)
                {
                    case InvokeStatus.Success:
                        {
                            httpResponse.FromJson(m_converter.ConvertTo(invokeResult.Result)).SetStatus();
                            break;
                        }
                    case InvokeStatus.UnFound:
                        {
                            string jsonString = m_converter.ConvertTo(new ActionResult() { Status = invokeResult.Status, Message = invokeResult.Message });
                            httpResponse.FromJson(jsonString).SetStatus("404", invokeResult.Status.ToString());
                            break;
                        }
                    case InvokeStatus.UnEnable:
                        {
                            string jsonString = m_converter.ConvertTo(new ActionResult() { Status = invokeResult.Status, Message = invokeResult.Message });
                            httpResponse.FromJson(jsonString).SetStatus("405", invokeResult.Status.ToString());
                            break;
                        }
                    case InvokeStatus.InvocationException:
                    case InvokeStatus.Exception:
                        {
                            string jsonString = m_converter.ConvertTo(new ActionResult() { Status = invokeResult.Status, Message = invokeResult.Message });
                            httpResponse.FromJson(jsonString).SetStatus("422", invokeResult.Status.ToString());
                            break;
                        }
                }

                using (ByteBlock byteBlock = new ByteBlock())
                {
                    httpResponse.Build(byteBlock);
                    client.DefaultSend(byteBlock);
                }

                if (!e.Context.Request.KeepAlive)
                {
                    client.TryShutdown(SocketShutdown.Both);
                }
            }

            base.OnPost(client, e);
        }

        #region RPC解析器

        void IRpcParser.OnRegisterServer(MethodInstance[] methodInstances)
        {
            foreach (var methodInstance in methodInstances)
            {
                if (methodInstance.GetAttribute<WebApiAttribute>() is WebApiAttribute attribute)
                {
                    string[] actionUrls = attribute.GetRouteUrls(methodInstance);
                    if (actionUrls != null)
                    {
                        foreach (var item in actionUrls)
                        {
                            m_actionMap.Add(item, methodInstance);
                        }
                    }
                }
            }
        }

        void IRpcParser.OnUnregisterServer(MethodInstance[] methodInstances)
        {
            foreach (var methodInstance in methodInstances)
            {
                if (methodInstance.GetAttribute<WebApiAttribute>() is WebApiAttribute attribute)
                {
                    string[] actionUrls = attribute.GetRouteUrls(methodInstance);
                    if (actionUrls != null)
                    {
                        foreach (var item in actionUrls)
                        {
                            m_actionMap.Remove(item);
                        }
                    }
                }
            }
        }

        void IRpcParser.SetRpcStore(RpcStore rpcService)
        {
            m_rpcStore = rpcService;
        }

        #endregion RPC解析器
    }
}