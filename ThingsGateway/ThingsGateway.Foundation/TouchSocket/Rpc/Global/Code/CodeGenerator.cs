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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ThingsGateway.Foundation.Rpc
{
    /// <summary>
    /// 代码生成器
    /// </summary>
    public static class CodeGenerator
    {
        internal static readonly Dictionary<Type, string> m_proxyType = new Dictionary<Type, string>();
        internal static readonly List<Assembly> m_assemblies = new List<Assembly>();
        internal static readonly List<Assembly> m_ignoreAssemblies = new List<Assembly>();
        internal static readonly List<Type> m_ignoreTypes = new List<Type>();

        /// <summary>
        /// 添加不需要代理的程序集
        /// </summary>
        /// <param name="assembly"></param>
        public static void AddIgnoreProxyAssembly(Assembly assembly)
        {
            m_ignoreAssemblies.Add(assembly);
        }

        /// <summary>
        /// 添加不需要代理的类型
        /// </summary>
        /// <param name="type"></param>
        public static void AddIgnoreProxyType(Type type)
        {
            m_ignoreTypes.Add(type);
        }

        /// <summary>
        /// 添加需要代理的程序集
        /// </summary>
        /// <param name="assembly"></param>
        public static void AddProxyAssembly(Assembly assembly)
        {
            m_assemblies.Add(assembly);
        }

        /// <summary>
        /// 添加代理类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="deepSearch"></param>
        public static void AddProxyType(Type type, bool deepSearch = true)
        {
            if (type.IsPrimitive || type == typeof(string))
            {
                return;
            }
            if (!m_proxyType.ContainsKey(type))
            {
                RpcProxyAttribute attribute = type.GetCustomAttribute<RpcProxyAttribute>();
                m_proxyType.Add(type, attribute == null ? type.Name : attribute.ClassName);
                if (deepSearch)
                {
                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
                    foreach (var item in properties)
                    {
                        AddProxyType(item.PropertyType);
                    }
                }
            }
        }

        /// <summary>
        /// 添加代理类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deepSearch"></param>
        public static void AddProxyType<T>(bool deepSearch = true)
        {
            AddProxyType(typeof(T), deepSearch);
        }

        /// <summary>
        /// 是否包含类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ContainsType(Type type)
        {
            return m_proxyType.ContainsKey(type);
        }

        /// <summary>
        /// 转换为cs代码。
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="serverCodes"></param>
        /// <returns></returns>
        public static string ConvertToCode(string @namespace, params ServerCellCode[] serverCodes)
        {
            Dictionary<string, ServerCellCode> serverCellCodes = new Dictionary<string, ServerCellCode>();
            Dictionary<string, ClassCellCode> classCellCodes = new Dictionary<string, ClassCellCode>();
            StringBuilder codeString = new StringBuilder();
            foreach (var serverCellCode in serverCodes)
            {
                if (serverCellCodes.ContainsKey(serverCellCode.Name))
                {
                    if (serverCellCode.IncludeExtension)
                    {
                        serverCellCodes[serverCellCode.Name].IncludeExtension = true;
                    }

                    if (serverCellCode.IncludeInstance)
                    {
                        serverCellCodes[serverCellCode.Name].IncludeInstance = true;
                    }

                    if (serverCellCode.IncludeInterface)
                    {
                        serverCellCodes[serverCellCode.Name].IncludeInterface = true;
                    }

                    var ccm = serverCellCodes[serverCellCode.Name].Methods;
                    foreach (var item in serverCellCode.Methods.Keys)
                    {
                        if (!ccm.ContainsKey(item))
                        {
                            ccm.Add(item, serverCellCode.Methods[item]);
                        }
                    }
                }
                else
                {
                    serverCellCodes.Add(serverCellCode.Name, serverCellCode);
                }

                foreach (var item in serverCellCode.ClassCellCodes.Keys)
                {
                    if (!classCellCodes.ContainsKey(item))
                    {
                        classCellCodes.Add(item, serverCellCode.ClassCellCodes[item]);
                    }
                }
            }

            string namesp = string.IsNullOrEmpty(@namespace) ? "RRQMProxy" : @namespace;

            codeString.AppendLine("using System;");
            codeString.AppendLine("using ThingsGateway.Foundation;");
            codeString.AppendLine("using ThingsGateway.Foundation.Sockets;");
            codeString.AppendLine("using ThingsGateway.Foundation.Rpc;");
            codeString.AppendLine("using ThingsGateway.Foundation.Rpc.TouchRpc;");
            codeString.AppendLine("using System.Collections.Generic;");
            codeString.AppendLine("using System.Diagnostics;");
            codeString.AppendLine("using System.Text;");
            codeString.AppendLine("using System.Threading.Tasks;");
            codeString.AppendLine(string.Format("namespace {0}", namesp));
            codeString.AppendLine("{");

            foreach (var serverCellCode in serverCellCodes.Values)
            {
                if (serverCellCode.IncludeInterface)
                {
                    //接口
                    codeString.AppendLine($"public interface I{serverCellCode.Name}:IRemoteServer");//类开始
                    codeString.AppendLine("{");
                    foreach (var item in serverCellCode.Methods.Values)
                    {
                        codeString.AppendLine(item.InterfaceTemple);
                    }
                    codeString.AppendLine("}");
                    //接口
                }

                if (serverCellCode.IncludeInstance)
                {
                    //类
                    if (serverCellCode.IncludeInterface)
                    {
                        codeString.AppendLine($"public class {serverCellCode.Name} :I{serverCellCode.Name}");//类开始
                    }
                    else
                    {
                        codeString.AppendLine($"public class {serverCellCode.Name}");//类开始
                    }

                    codeString.AppendLine("{");
                    codeString.AppendLine($"public {serverCellCode.Name}(IRpcClient client)");
                    codeString.AppendLine("{");
                    codeString.AppendLine("this.Client=client;");
                    codeString.AppendLine("}");
                    codeString.AppendLine("public IRpcClient Client{get;private set; }");
                    foreach (var item in serverCellCode.Methods.Values)
                    {
                        codeString.AppendLine(item.CodeTemple);
                    }
                    codeString.AppendLine("}");
                    //类
                }

                if (serverCellCode.IncludeExtension)
                {
                    //扩展类
                    codeString.AppendLine($"public static class {serverCellCode.Name}Extensions");//类开始
                    codeString.AppendLine("{");
                    foreach (var item in serverCellCode.Methods.Values)
                    {
                        codeString.AppendLine(item.ExtensionsTemple);
                    }
                    codeString.AppendLine("}");
                    //扩展类
                }
            }

            foreach (var item in classCellCodes.Values)
            {
                codeString.AppendLine(item.Code);
            }

            codeString.AppendLine("}");

            return codeString.ToString();
        }

        /// <summary>
        /// 生成代码代理
        /// </summary>
        /// <typeparam name="TServer">服务类型</typeparam>
        /// <typeparam name="TAttribute">属性标签</typeparam>
        /// <returns></returns>
        public static ServerCellCode Generator<TServer, TAttribute>() where TServer : IRpcServer where TAttribute : RpcAttribute
        {
            return Generator(typeof(TServer), typeof(TAttribute));
        }

        /// <summary>
        /// 生成代码代理
        /// </summary>
        /// <param name="serverType">服务类型</param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static ServerCellCode Generator(Type serverType, Type attributeType)
        {
            ServerCellCode serverCellCode = new ServerCellCode();
            MethodInstance[] methodInstances = GetMethodInstances(serverType);

            List<Assembly> assemblies = new List<Assembly>(m_assemblies);
            assemblies.Add(serverType.Assembly);

            foreach (var item in m_ignoreAssemblies)
            {
                assemblies.Remove(item);
            }

            ClassCodeGenerator classCodeGenerator = new ClassCodeGenerator(assemblies.ToArray());

            serverCellCode.Name = serverType.IsInterface ?
                (serverType.Name.StartsWith("I") ? serverType.Name.Remove(0, 1) : serverType.Name) : serverType.Name;
            List<MethodInstance> instances = new List<MethodInstance>();

            foreach (var item in m_proxyType.Keys)
            {
                int deep = 0;
                classCodeGenerator.AddTypeString(item, ref deep);
            }

            foreach (MethodInstance methodInstance in methodInstances)
            {
                foreach (RpcAttribute att in methodInstance.RpcAttributes)
                {
                    if (attributeType == att.GetType())
                    {
                        if (methodInstance.ReturnType != null)
                        {
                            int deep = 0;
                            classCodeGenerator.AddTypeString(methodInstance.ReturnType, ref deep);
                        }

                        int i = 0;
                        if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
                        {
                            i = 1;
                        }
                        for (; i < methodInstance.ParameterTypes.Length; i++)
                        {
                            int deep = 0;
                            classCodeGenerator.AddTypeString(methodInstance.ParameterTypes[i], ref deep);
                        }

                        instances.Add(methodInstance);
                        break;
                    }
                }
            }

            classCodeGenerator.CheckDeep();

            foreach (var item in classCodeGenerator.GenericTypeDic.Keys.ToArray())
            {
                if (m_ignoreTypes.Contains(item))
                {
                    classCodeGenerator.GenericTypeDic.TryRemove(item, out _);
                }

                if (m_ignoreAssemblies.Contains(item.Assembly))
                {
                    classCodeGenerator.GenericTypeDic.TryRemove(item, out _);
                }
            }

            foreach (var item in classCodeGenerator.PropertyDic.Keys.ToArray())
            {
                if (m_ignoreTypes.Contains(item))
                {
                    classCodeGenerator.PropertyDic.TryRemove(item, out _);
                }

                if (m_ignoreAssemblies.Contains(item.Assembly))
                {
                    classCodeGenerator.PropertyDic.TryRemove(item, out _);
                }
            }

            foreach (var item in classCodeGenerator.GetClassCellCodes())
            {
                serverCellCode.ClassCellCodes.Add(item.Name, item);
            }

            //ServerCodeGenerator serverCodeGenerator = new ServerCodeGenerator(classCodeGenerator);

            bool first = true;
            foreach (var item in instances)
            {
                MethodCellCode methodCellCode = new MethodCellCode();
                RpcAttribute rpcAttribute = (RpcAttribute)item.GetAttribute(attributeType);
                if (rpcAttribute == null)
                {
                    continue;
                }
                rpcAttribute.SetClassCodeGenerator(classCodeGenerator);
                if (first)
                {
                    if (rpcAttribute.GeneratorFlag.HasFlag(CodeGeneratorFlag.IncludeInterface))
                    {
                        serverCellCode.IncludeInterface = true;
                    }
                    if (rpcAttribute.GeneratorFlag.HasFlag(CodeGeneratorFlag.IncludeInstance))
                    {
                        serverCellCode.IncludeInstance = true;
                    }
                    if (rpcAttribute.GeneratorFlag.HasFlag(CodeGeneratorFlag.IncludeExtension))
                    {
                        serverCellCode.IncludeExtension = true;
                    }
                    first = false;
                }

                methodCellCode.InterfaceTemple = rpcAttribute.GetInterfaceProxyCode(item);
                methodCellCode.CodeTemple = rpcAttribute.GetInstanceProxyCode(item);
                methodCellCode.ExtensionsTemple = rpcAttribute.GetExtensionsMethodProxyCode(item);
                methodCellCode.Name = ((RpcAttribute)item.GetAttribute(attributeType)).GetMethodName(item, false);
                serverCellCode.Methods.Add(methodCellCode.Name, methodCellCode);
            }

            return serverCellCode;
        }

        /// <summary>
        /// 从类型获取函数实例
        /// </summary>
        /// <typeparam name="TServer"></typeparam>
        /// <returns></returns>
        public static MethodInstance[] GetMethodInstances<TServer>() where TServer : IRpcServer
        {
            return GetMethodInstances(typeof(TServer));
        }

        /// <summary>
        /// 生成代理代码
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="serverTypes"></param>
        /// <param name="attributeTypes"></param>
        /// <returns></returns>
        public static string GetProxyCodes(string @namespace, Type[] serverTypes, Type[] attributeTypes)
        {
            List<ServerCellCode> serverCellCodeList = new List<ServerCellCode>();
            foreach (var item in serverTypes)
            {
                foreach (var item1 in attributeTypes)
                {
                    serverCellCodeList.Add(Generator(item, item1));
                }
            }
            return ConvertToCode(@namespace, serverCellCodeList.ToArray());
        }

        /// <summary>
        /// 从类型获取函数实例
        /// </summary>
        /// <param name="serverType"></param>
        /// <returns></returns>
        public static MethodInstance[] GetMethodInstances(Type serverType)
        {
            if (!typeof(IRpcServer).IsAssignableFrom(serverType))
            {
                throw new RpcException($"服务类型必须从{nameof(IRpcServer)}派生。");
            }
            List<MethodInstance> instances = new List<MethodInstance>();

            MethodInfo[] methodInfos = serverType.GetMethods();

            foreach (MethodInfo method in methodInfos)
            {
                if (method.IsGenericMethod)
                {
                    continue;
                }
                IEnumerable<RpcAttribute> attributes = method.GetCustomAttributes<RpcAttribute>(true);
                if (attributes.Count() > 0)
                {
                    MethodInstance methodInstance = new MethodInstance(method);
                    methodInstance.ServerType = serverType;
                    methodInstance.RpcAttributes = attributes.ToArray();
                    methodInstance.Description = method.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    methodInstance.IsEnable = true;
                    methodInstance.Parameters = method.GetParameters();

                    object[] filters = method.GetCustomAttributes(true);
                    List<IRpcActionFilter> actionFilters = new List<IRpcActionFilter>();
                    foreach (var item in filters)
                    {
                        if (item is IRpcActionFilter filter)
                        {
                            actionFilters.Add(filter);
                        }
                    }
                    if (actionFilters.Count > 0)
                    {
                        methodInstance.Filters = actionFilters.ToArray();
                    }
                    foreach (var item in attributes)
                    {
                        methodInstance.MethodFlags |= item.MethodFlags;
                    }
                    if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
                    {
                        if (methodInstance.Parameters.Length == 0 || !typeof(ICallContext).IsAssignableFrom(methodInstance.Parameters[0].ParameterType))
                        {
                            throw new RpcException($"函数：{method}，标识包含{MethodFlags.IncludeCallContext}时，必须包含{nameof(ICallContext)}或其派生类参数，且为第一参数。");
                        }
                    }
                    List<string> names = new List<string>();
                    foreach (var parameterInfo in methodInstance.Parameters)
                    {
                        names.Add(parameterInfo.Name);
                    }
                    methodInstance.ParameterNames = names.ToArray();
                    ParameterInfo[] parameters = method.GetParameters();
                    List<Type> types = new List<Type>();
                    foreach (var parameter in parameters)
                    {
                        types.Add(parameter.ParameterType.GetRefOutType());
                    }
                    methodInstance.ParameterTypes = types.ToArray();
                    instances.Add(methodInstance);
                }
            }

            return instances.ToArray();
        }

        /// <summary>
        /// 获取类型代理名称
        /// </summary>
        /// <param name="type"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static bool TryGetProxyTypeName(Type type, out string className)
        {
            return m_proxyType.TryGetValue(type, out className);
        }
    }
}