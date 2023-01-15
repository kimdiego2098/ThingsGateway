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
using System.Net.Sockets;


using System.Xml;

namespace ThingsGateway.Foundation.Rpc.XmlRpc
{
    /// <summary>
    /// XmlRpc解析器
    /// </summary>
    public class XmlRpcParserPlugin : HttpPluginBase, IRpcParser
    {
        private readonly ActionMap m_actionMap;
        private RpcStore m_rpcStore;
        private string m_xmlRpcUrl = "/xmlrpc";

        /// <summary>
        /// 构造函数
        /// </summary>
        public XmlRpcParserPlugin([DependencyParamterInject(true)] RpcStore rpcStore)
        {
            m_actionMap = new ActionMap();
            rpcStore?.AddRpcParser(GetType().Name, this);
        }

        /// <summary>
        /// XmlRpc调用
        /// </summary>
        public ActionMap ActionMap => m_actionMap;

        /// <summary>
        /// 所属服务器
        /// </summary>
        public RpcStore RpcStore => m_rpcStore;

        /// <summary>
        /// 当挂载在<see cref="HttpService"/>时，匹配Url然后响应。当设置为null或空时，会全部响应。
        /// </summary>
        public string XmlRpcUrl
        {
            get => m_xmlRpcUrl;
            set => m_xmlRpcUrl = string.IsNullOrEmpty(value) ? "/" : value;
        }

        #region RPC解析器

        void IRpcParser.OnRegisterServer(MethodInstance[] methodInstances)
        {
            foreach (var methodInstance in methodInstances)
            {
                if (methodInstance.GetAttribute<XmlRpcAttribute>() is XmlRpcAttribute attribute)
                {
                    m_actionMap.Add(attribute.GetInvokenKey(methodInstance), methodInstance);
                }
            }
        }

        void IRpcParser.OnUnregisterServer(MethodInstance[] methodInstances)
        {
            foreach (var methodInstance in methodInstances)
            {
                if (methodInstance.GetAttribute<XmlRpcAttribute>() is XmlRpcAttribute attribute)
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
        /// <param name="xmlRpcUrl"></param>
        /// <returns></returns>
        public XmlRpcParserPlugin SetXmlRpcUrl(string xmlRpcUrl)
        {
            XmlRpcUrl = xmlRpcUrl;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnPost(ITcpClientBase client, HttpContextEventArgs e)
        {
            if (m_xmlRpcUrl == "/" || e.Context.Request.UrlEquals(m_xmlRpcUrl))
            {
                e.Handled = true;

                XmlDocument xml = new XmlDocument();
                string xmlstring = e.Context.Request.GetBody();
                xml.LoadXml(xmlstring);
                XmlNode methodName = xml.SelectSingleNode("methodCall/methodName");
                string actionKey = methodName.InnerText;

                object[] ps = null;
                InvokeResult invokeResult = new InvokeResult();
                XmlRpcCallContext callContext = null;

                if (m_actionMap.TryGetMethodInstance(actionKey, out MethodInstance methodInstance))
                {
                    if (methodInstance.IsEnable)
                    {
                        try
                        {
                            callContext = new XmlRpcCallContext()
                            {
                                Caller = client,
                                HttpContext = e.Context,
                                MethodInstance = methodInstance,
                                XmlString = xmlstring
                            };
                            ps = new object[methodInstance.ParameterNames.Length];
                            XmlNode paramsNode = xml.SelectSingleNode("methodCall/params");
                            if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
                            {
                                ps[0] = callContext;
                                int index = 1;
                                foreach (XmlNode paramNode in paramsNode.ChildNodes)
                                {
                                    XmlNode valueNode = paramNode.FirstChild.FirstChild;
                                    ps[index] = XmlDataTool.GetValue(valueNode, methodInstance.ParameterTypes[index]);
                                    index++;
                                }
                            }
                            else
                            {
                                int index = 0;
                                foreach (XmlNode paramNode in paramsNode.ChildNodes)
                                {
                                    XmlNode valueNode = paramNode.FirstChild.FirstChild;
                                    ps[index] = XmlDataTool.GetValue(valueNode, methodInstance.ParameterTypes[index]);
                                    index++;
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
                        invokeResult.Message = "服务不可用";
                    }
                }
                else
                {
                    invokeResult.Status = InvokeStatus.UnFound;
                    invokeResult.Message = "没有找到这个服务。";
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

                HttpResponse httpResponse = e.Context.Response;

                ByteBlock byteBlock = new ByteBlock();

                if (invokeResult.Status == InvokeStatus.Success)
                {
                    XmlDataTool.CreatResponse(httpResponse, invokeResult.Result);
                }
                else
                {
                    httpResponse.StatusCode = "201";
                    httpResponse.StatusMessage = invokeResult.Message;
                }
                try
                {
                    httpResponse.Answer();
                }
                finally
                {
                    byteBlock.Dispose();
                }

                if (!e.Context.Request.KeepAlive)
                {
                    client.TryShutdown(SocketShutdown.Both);
                }
            }
            base.OnPost(client, e);
        }
    }
}