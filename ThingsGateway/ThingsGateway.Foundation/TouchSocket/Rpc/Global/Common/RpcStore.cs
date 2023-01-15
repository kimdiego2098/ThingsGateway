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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;


using System.Reflection;

namespace ThingsGateway.Foundation.Rpc
{
    /// <summary>
    /// Rpc仓库
    /// </summary>
    public class RpcStore : DisposableObject, IEnumerable<IRpcParser>
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        public const string Namespace = "namespace";

        /// <summary>
        /// 代理键
        /// </summary>
        public const string ProxyKey = "proxy";

        private static readonly ConcurrentDictionary<string, Type> m_proxyAttributeMap = new ConcurrentDictionary<string, Type>();
        private readonly ConcurrentDictionary<string, IRpcParser> m_parsers = new ConcurrentDictionary<string, IRpcParser>();
        private readonly ConcurrentDictionary<Type, List<MethodInstance>> m_serverTypes = new ConcurrentDictionary<Type, List<MethodInstance>>();
        private string m_proxyUrl = "/proxy";
        private HttpService m_service;

        static RpcStore()
        {
            SearchAttribute();
        }

        /// <summary>
        /// 实例化一个Rpc仓库。
        /// <para>需要指定<see cref="IContainer"/>容器。一般和对应的服务器、客户端共用一个容器比较好。</para>
        /// <para>如果，仅仅是只有一个解析器的话，可以考虑从配置<see cref="TouchSocketConfig"/>中，调用<see cref="RpcConfigExtensions.ConfigureRpcStore(TouchSocketConfig, Action{RpcStore}, RpcStore)"/></para>
        /// </summary>
        public RpcStore(IContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Container.RegisterSingleton<RpcStore>(this);

            if (!container.IsRegistered(typeof(IRpcServerFactory)))
            {
                Container.RegisterSingleton<IRpcServerFactory, RpcServerFactory>();
            }

            SearchAttribute();
        }

        /// <summary>
        /// 代理属性映射。
        /// </summary>
        public static ConcurrentDictionary<string, Type> ProxyAttributeMap => m_proxyAttributeMap;

        /// <summary>
        /// 内置IOC容器
        /// </summary>
        public IContainer Container { get; private set; }

        /// <summary>
        /// 解析器集合。
        /// <para>如果想快速获得对象，请使用<see cref="TryGetRpcParser(string, out IRpcParser)"/>，一般key为对象类型名称，或自定义的。</para>
        /// </summary>
        public IRpcParser[] RpcParsers => m_parsers.Values.ToArray();

        /// <summary>
        /// 请求代理。
        /// </summary>
        public Func<HttpRequest, bool> OnRequestProxy { get; set; }

        /// <summary>
        /// 代理路径。默认为“/proxy”。
        /// <para>必须以“/”开头</para>
        /// </summary>
        public string ProxyUrl
        {
            get => m_proxyUrl;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = "/";
                }
                m_proxyUrl = value;
            }
        }

        /// <summary>
        /// 服务类型
        /// </summary>
        public Type[] ServerTypes => m_serverTypes.Keys.ToArray();

        /// <summary>
        /// 获取IRpcParser
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IRpcParser this[string key] => m_parsers[key];

        /// <summary>
        /// 从远程获取代理
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetProxyInfo(string url)
        {
            string result = Get(url);
            if (string.IsNullOrEmpty(result))
            {
                throw new Exception("未知错误");
            }
            return result;
        }

        /// <summary>
        /// 添加Rpc解析器
        /// </summary>
        /// <param name="key">名称</param>
        /// <param name="parser">解析器实例</param>
        /// <param name="applyServer">是否应用已注册服务</param>
        public void AddRpcParser(string key, IRpcParser parser, bool applyServer = true)
        {
            if (DisposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (!m_parsers.TryAdd(key, parser))
            {
                throw new Exception("相同键值得解析器已经存在。");
            }
            parser.SetRpcStore(this);
            if (applyServer)
            {
                foreach (var item in m_serverTypes)
                {
                    parser.OnRegisterServer(item.Value.ToArray());
                }
            }
        }

        /// <summary>
        /// 获取服务类型对应的服务方法。
        /// </summary>
        /// <param name="serverType"></param>
        /// <returns></returns>
        public MethodInstance[] GetServerMethodInstances(Type serverType)
        {
            return m_serverTypes[serverType].ToArray();
        }

        /// <summary>
        /// 执行Rpc
        /// </summary>
        /// <param name="rpcServer"></param>
        /// <param name="ps"></param>
        /// <param name="callContext"></param>
        /// <returns></returns>
        public InvokeResult Execute(IRpcServer rpcServer, object[] ps, ICallContext callContext)
        {
            if (DisposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            InvokeResult invokeResult = new InvokeResult();
            try
            {
                if (callContext.MethodInstance.Filters != null)
                {
                    for (int i = 0; i < callContext.MethodInstance.Filters.Length; i++)
                    {
                        callContext.MethodInstance.Filters[i].Executing(callContext, ps, ref invokeResult);
                        callContext.MethodInstance.Filters[i].ExecutingAsync(callContext, ps, ref invokeResult);
                    }
                }

                if (invokeResult.Status != InvokeStatus.Ready)
                {
                    return invokeResult;
                }

                if (callContext.MethodInstance.HasReturn)
                {
                    invokeResult.Result = callContext.MethodInstance.Invoke(rpcServer, ps);
                }
                else
                {
                    callContext.MethodInstance.Invoke(rpcServer, ps);
                }
                invokeResult.Status = InvokeStatus.Success;
                if (callContext.MethodInstance.Filters != null)
                {
                    for (int i = 0; i < callContext.MethodInstance.Filters.Length; i++)
                    {
                        callContext.MethodInstance.Filters[i].Executed(callContext, ps, ref invokeResult);
                        callContext.MethodInstance.Filters[i].ExecutedAsync(callContext, ps, ref invokeResult);
                    }
                }
            }
            catch (TargetInvocationException ex)
            {
                invokeResult.Status = InvokeStatus.InvocationException;
                if (ex.InnerException != null)
                {
                    invokeResult.Message = "函数内部发生异常，信息：" + ex.InnerException.Message;
                }
                else
                {
                    invokeResult.Message = "函数内部发生异常，信息：未知";
                }
                if (callContext.MethodInstance.Filters != null)
                {
                    for (int i = 0; i < callContext.MethodInstance.Filters.Length; i++)
                    {
                        callContext.MethodInstance.Filters[i].ExecutException(callContext, ps, ref invokeResult, ex);
                        callContext.MethodInstance.Filters[i].ExecutExceptionAsync(callContext, ps, ref invokeResult, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                invokeResult.Status = InvokeStatus.Exception;
                invokeResult.Message = ex.Message;
                if (callContext.MethodInstance.Filters != null)
                {
                    for (int i = 0; i < callContext.MethodInstance.Filters.Length; i++)
                    {
                        callContext.MethodInstance.Filters[i].ExecutException(callContext, ps, ref invokeResult, ex);
                        callContext.MethodInstance.Filters[i].ExecutExceptionAsync(callContext, ps, ref invokeResult, ex);
                    }
                }
            }

            return invokeResult;
        }

        /// <summary>
        /// 获取所有已注册的函数。
        /// </summary>
        public MethodInstance[] GetAllMethods()
        {
            List<MethodInstance> methods = new List<MethodInstance>();
            foreach (var item in m_serverTypes.Values)
            {
                methods.AddRange(item);
            }

            return methods.ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_parsers.Values.GetEnumerator();
        }

        /// <summary>
        /// 返回枚举对象
        /// </summary>
        /// <returns></returns>
        IEnumerator<IRpcParser> IEnumerable<IRpcParser>.GetEnumerator()
        {
            return m_parsers.Values.GetEnumerator();
        }

        /// <summary>
        /// 本地获取代理
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="attrbuteTypes"></param>
        /// <returns></returns>
        public string GetProxyCodes(string @namespace, Type[] attrbuteTypes)
        {
            var cellCodes = GetProxyInfo(attrbuteTypes == null ? ProxyAttributeMap.Values.ToArray() : attrbuteTypes);
            return CodeGenerator.ConvertToCode(@namespace, cellCodes);
        }

        /// <summary>
        /// 本地获取代理
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public string GetProxyCodes(string @namespace)
        {
            return GetProxyCodes(@namespace, null);
        }

        /// <summary>
        /// 从本地获取代理
        /// </summary>
        /// <returns></returns>
        public ServerCellCode[] GetProxyInfo()
        {
            return GetProxyInfo(ProxyAttributeMap.Values.ToArray());
        }

        /// <summary>
        /// 从本地获取代理
        /// </summary>
        /// <param name="attrbuteType"></param>
        /// <returns></returns>
        public ServerCellCode[] GetProxyInfo(Type[] attrbuteType)
        {
            if (DisposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            List<ServerCellCode> codes = new List<ServerCellCode>();

            foreach (var attrbute in attrbuteType)
            {
                foreach (var item in m_serverTypes.Keys)
                {
                    ServerCellCode serverCellCode = CodeGenerator.Generator(item, attrbute);
                    codes.Add(serverCellCode);
                }
            }
            return codes.ToArray();
        }

        /// <summary>
        /// 移除Rpc解析器
        /// </summary>
        /// <param name="parserName"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public bool RemoveRpcParser(string parserName, out IRpcParser parser)
        {
            return m_parsers.TryRemove(parserName, out parser);
        }

        /// <summary>
        /// 移除Rpc解析器
        /// </summary>
        /// <param name="parserName"></param>
        /// <returns></returns>
        public bool RemoveRpcParser(string parserName)
        {
            return RemoveRpcParser(parserName, out _);
        }

        /// <summary>
        /// 分享代理。
        /// </summary>
        /// <param name="iPHost"></param>
        public void ShareProxy(IPHost iPHost)
        {
            if (DisposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (m_service != null)
            {
                return;
            }
            m_service = new HttpService();
            m_service.Setup(new TouchSocketConfig()
                .SetListenIPHosts(new IPHost[] { iPHost }))
                .Start();

            m_service.AddPlugin(new InternalPlugin(this));
        }

        /// <summary>
        /// 关闭分享中心
        /// </summary>
        public void StopShareProxy()
        {
            m_service.SafeDispose();
        }

        /// <summary>
        /// 获取IRpcParser
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public bool TryGetRpcParser(string key, out IRpcParser parser)
        {
            if (DisposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            return m_parsers.TryGetValue(key, out parser);
        }

        /// <summary>
        /// 移除注册服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public int UnregisterServer(IRpcServer provider)
        {
            return UnregisterServer(provider.GetType());
        }

        /// <summary>
        /// 移除注册服务
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public int UnregisterServer(Type providerType)
        {
            if (DisposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (!typeof(IRpcServer).IsAssignableFrom(providerType))
            {
                throw new RpcException("类型不相符");
            }

            if (RemoveServer(providerType, out MethodInstance[] instances))
            {
                foreach (var parser in this)
                {
                    parser.OnUnregisterServer(instances);
                }

                return instances.Length;
            }
            return 0;
        }

        /// <summary>
        /// 移除注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int UnregisterServer<T>() where T : RpcServer
        {
            return UnregisterServer(typeof(T));
        }

        internal bool TryRemove(string key, out IRpcParser parser)
        {
            return m_parsers.TryRemove(key, out parser);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                StopShareProxy();
                foreach (var item in this)
                {
                    item.SafeDispose();
                }
            }

            base.Dispose(disposing);
        }

        private static string Get(string url)
        {
            string result = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //添加参数
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            try
            {
                //获取内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                resp.SafeDispose();
                stream.Close();
            }
            return result;
        }

        private static void SearchAttribute()
        {
            List<Type> types = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    Type[] t1 = assembly.GetTypes().Where(p => typeof(RpcAttribute).IsAssignableFrom(p) && !p.IsAbstract).ToArray();
                    types.AddRange(t1);
                }
                catch
                {
                }
            }

            foreach (Type type in types)
            {
                ProxyAttributeMap.TryAdd(type.Name.Replace("Attribute", string.Empty).ToLower(), type);
            }
        }

        private bool RemoveServer(Type type, out MethodInstance[] methodInstances)
        {
            foreach (var newType in m_serverTypes.Keys)
            {
                if (newType.FullName == type.FullName)
                {
                    m_serverTypes.TryRemove(newType, out var list);
                    methodInstances = list.ToArray();
                    return true;
                }
            }
            methodInstances = null;
            return false;
        }

        #region 注册

        /// <summary>
        /// 注册为单例服务
        /// </summary>
        /// <param name="serverFromType"></param>
        /// <param name="rpcServer"></param>
        /// <returns></returns>
        public void RegisterServer(Type serverFromType, IRpcServer rpcServer)
        {
            if (!typeof(IRpcServer).IsAssignableFrom(serverFromType))
            {
                throw new RpcException($"注册类型必须与{nameof(IRpcServer)}有继承关系");
            }

            if (!serverFromType.IsAssignableFrom(rpcServer.GetType()))
            {
                throw new RpcException("实例类型必须与注册类型有继承关系。");
            }
            foreach (var item in m_serverTypes.Keys)
            {
                if (item.FullName == serverFromType.FullName)
                {
                    throw new RpcException($"名为{serverFromType.FullName}的类型已注册。");
                }
            }

            MethodInstance[] methodInstances = CodeGenerator.GetMethodInstances(serverFromType);
            foreach (var item in methodInstances)
            {
                item.IsSingleton = true;
                //item.ServerFactory = new RpcServerFactory(this.Container);
                item.ServerFactory = Container.Resolve<IRpcServerFactory>() ?? throw new ArgumentNullException($"{nameof(IRpcServerFactory)}");
            }
            m_serverTypes.TryAdd(serverFromType, new List<MethodInstance>(methodInstances));
            Container.RegisterSingleton(serverFromType, rpcServer);

            foreach (var parser in this)
            {
                parser.OnRegisterServer(methodInstances);
            }
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="serverFromType"></param>
        /// <param name="serverToType"></param>
        /// <returns></returns>
        public void RegisterServer(Type serverFromType, Type serverToType)
        {
            if (!typeof(IRpcServer).IsAssignableFrom(serverFromType))
            {
                throw new RpcException($"注册类型必须与{nameof(IRpcServer)}有继承关系");
            }

            if (!serverFromType.IsAssignableFrom(serverToType))
            {
                throw new RpcException("实例类型必须与注册类型有继承关系。");
            }

            foreach (var item in m_serverTypes.Keys)
            {
                if (item.FullName == serverFromType.FullName)
                {
                    throw new RpcException($"名为{serverFromType.FullName}的类型已注册。");
                }
            }

            bool singleton;
            if (typeof(ITransientRpcServer).IsAssignableFrom(serverFromType))
            {
                singleton = false;
                Container.RegisterTransient(serverFromType, serverToType);
            }
            else
            {
                singleton = true;
                Container.RegisterSingleton(serverFromType, serverToType);
            }
            MethodInstance[] methodInstances = CodeGenerator.GetMethodInstances(serverFromType);

            foreach (var item in methodInstances)
            {
                item.IsSingleton = singleton;
                //item.ServerFactory = new RpcServerFactory(this.Container);
                item.ServerFactory = Container.Resolve<IRpcServerFactory>() ?? throw new ArgumentNullException($"{nameof(IRpcServerFactory)}");
            }

            m_serverTypes.TryAdd(serverFromType, new List<MethodInstance>(methodInstances));

            foreach (var parser in this)
            {
                parser.OnRegisterServer(methodInstances);
            }
        }

        #endregion 注册
    }

    internal class InternalPlugin : HttpPluginBase
    {
        private readonly RpcStore m_rpcStore;

        public InternalPlugin(RpcStore rpcCerter)
        {
            m_rpcStore = rpcCerter;
        }

        protected override void OnGet(ITcpClientBase client, HttpContextEventArgs e)
        {
            if (e.Context.Request.UrlEquals(m_rpcStore.ProxyUrl))
            {
                bool? b = m_rpcStore.OnRequestProxy?.Invoke(e.Context.Request);
                if (b == false)
                {
                    using (ByteBlock byteBlock = new ByteBlock())
                    {
                        e.Context.Response
                            .FromText("拒绝响应内容")
                        .SetStatus("403", "Forbidden")
                        .Build(byteBlock);
                        client.DefaultSend(byteBlock);
                    }
                    return;
                }
                string value = e.Context.Request.Query[RpcStore.ProxyKey];
                List<Type> types = new List<Type>();

                if (value.Equals("all", StringComparison.CurrentCultureIgnoreCase))
                {
                    types = RpcStore.ProxyAttributeMap.Values.ToList();
                }
                else
                {
                    string[] vs = value.Split(',');
                    foreach (var item in vs)
                    {
                        if (RpcStore.ProxyAttributeMap.TryGetValue(item, out Type type))
                        {
                            types.Add(type);
                        }
                    }
                }

                string names = e.Context.Request.Query[RpcStore.Namespace];

                names = string.IsNullOrEmpty(names) ? "RRQMProxy" : names;

                string code = CodeGenerator.ConvertToCode(names, m_rpcStore.GetProxyInfo(types.ToArray()));

                using (ByteBlock byteBlock = new ByteBlock())
                {
                    e.Context.Response
                    .SetStatus()
                    .SetContent(code)
                    .SetContentTypeFromFileName($"{names}.cs")
                    .Build(byteBlock);
                    client.DefaultSend(byteBlock);
                }
            }
            base.OnGet(client, e);
        }
    }
}