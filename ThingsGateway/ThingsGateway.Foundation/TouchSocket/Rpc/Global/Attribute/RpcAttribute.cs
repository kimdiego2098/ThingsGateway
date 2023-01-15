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
using System.Reflection;
using System.Text;

namespace ThingsGateway.Foundation.Rpc
{
    /// <summary>
    /// Rpc方法属性基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class RpcAttribute : Attribute
    {
        private readonly Dictionary<Type, string> m_exceptions = new Dictionary<Type, string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public RpcAttribute()
        {
            MethodFlags = MethodFlags.None;
            m_exceptions.Add(typeof(TimeoutException), "调用超时");
            m_exceptions.Add(typeof(RpcInvokeException), "Rpc调用异常");
            m_exceptions.Add(typeof(Exception), "其他异常");
        }

        /// <summary>
        /// 调用键。
        /// </summary>
        public string InvokenKey { get; set; }

        /// <summary>
        /// 类生成器
        /// </summary>
        public ClassCodeGenerator ClassCodeGenerator { get; private set; }

        /// <summary>
        /// 异常提示
        /// </summary>
        public Dictionary<Type, string> Exceptions => m_exceptions;

        /// <summary>
        /// 生成代码
        /// </summary>
        public CodeGeneratorFlag GeneratorFlag { get; protected set; } =
            CodeGeneratorFlag.Sync | CodeGeneratorFlag.Async | CodeGeneratorFlag.ExtensionSync | CodeGeneratorFlag.ExtensionAsync
            | CodeGeneratorFlag.IncludeInterface | CodeGeneratorFlag.IncludeInstance | CodeGeneratorFlag.IncludeExtension;

        /// <summary>
        /// 函数标识
        /// </summary>
        public MethodFlags MethodFlags { get; set; }

        /// <summary>
        /// 重新指定生成的函数名称。可以使用类似“JsonRpc_{0}”的模板格式。
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 当使用TryCanInvoke不能调用时，执行的代码。
        /// </summary>
        /// <returns></returns>
        public virtual string GetCannotInvoke(MethodInstance methodInstance)
        {
            return "throw new RpcException(\"Rpc无法执行。\");";
        }

        /// <summary>
        /// 获取注释信息
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <returns></returns>
        public virtual string GetDescription(MethodInstance methodInstance)
        {
            return string.IsNullOrEmpty(methodInstance.Description) ? "无注释信息" : methodInstance.Description;
        }

        /// <summary>
        /// 获取扩展的代理代码
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <returns></returns>
        public virtual string GetExtensionsMethodProxyCode(MethodInstance methodInstance)
        {
            StringBuilder codeString = new StringBuilder();

            string description = GetDescription(methodInstance);
            bool isOut;
            bool isRef;

            ParameterInfo[] parameters;
            List<string> parametersStr = GetParameters(methodInstance, out isOut, out isRef, out parameters);
            var InterfaceTypes = GetGenericInterfaceTypes();
            if (GeneratorFlag.HasFlag(CodeGeneratorFlag.ExtensionSync))
            {
                codeString.AppendLine("///<summary>");
                codeString.AppendLine($"///{description}");
                codeString.AppendLine("///</summary>");
                foreach (var item in Exceptions)
                {
                    codeString.AppendLine($"/// <exception cref=\"{item.Key.FullName}\">{item.Value}</exception>");
                }

                codeString.Append("public static ");
                codeString.Append(GetReturn(methodInstance, false));
                codeString.Append(" ");
                codeString.Append(GetMethodName(methodInstance, false));
                codeString.Append("<TClient>(");//方法参数

                codeString.Append($"this TClient client");

                codeString.Append(",");
                for (int i = 0; i < parametersStr.Count; i++)
                {
                    if (i > 0)
                    {
                        codeString.Append(",");
                    }

                    codeString.Append(parametersStr[i]);
                }
                if (parametersStr.Count > 0)
                {
                    codeString.Append(",");
                }
                codeString.Append(GetInvokeOption());
                codeString.AppendLine(") where TClient:");

                for (int i = 0; i < InterfaceTypes.Length; i++)
                {
                    if (i > 0)
                    {
                        codeString.Append(",");
                    }

                    codeString.Append(InterfaceTypes[i].FullName);
                }

                codeString.AppendLine("{");//方法开始

                codeString.AppendLine("if (client.TryCanInvoke?.Invoke(client)==false)");
                codeString.AppendLine("{");
                codeString.AppendLine(GetCannotInvoke(methodInstance));
                codeString.AppendLine("}");

                if (parametersStr.Count > 0)
                {
                    codeString.Append($"object[] parameters = new object[]");
                    codeString.Append("{");

                    foreach (ParameterInfo parameter in parameters)
                    {
                        if (parameter.ParameterType.Name.Contains("&") && parameter.IsOut)
                        {
                            codeString.Append($"default({GetProxyTypeName(parameter.ParameterType)})");
                        }
                        else
                        {
                            codeString.Append(parameter.Name);
                        }
                        if (parameter != parameters[parameters.Length - 1])
                        {
                            codeString.Append(",");
                        }
                    }
                    codeString.AppendLine("};");

                    if (isOut || isRef)
                    {
                        codeString.Append($"Type[] types = new Type[]");
                        codeString.Append("{");
                        foreach (ParameterInfo parameter in parameters)
                        {
                            codeString.Append($"typeof({GetProxyTypeName(parameter.ParameterType)})");
                            if (parameter != parameters[parameters.Length - 1])
                            {
                                codeString.Append(",");
                            }
                        }
                        codeString.AppendLine("};");
                    }
                }

                if (methodInstance.HasReturn)
                {
                    if (parametersStr.Count == 0)
                    {
                        codeString.Append(string.Format("{0} returnData=client.Invoke<{0}>", GetProxyTypeName(methodInstance.ReturnType)));
                        codeString.Append("(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, null);");
                    }
                    else if (isOut || isRef)
                    {
                        codeString.Append(string.Format("{0} returnData=client.Invoke<{0}>", GetProxyTypeName(methodInstance.ReturnType)));
                        codeString.Append("(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption,ref parameters,types);");
                    }
                    else
                    {
                        codeString.Append(string.Format("{0} returnData=client.Invoke<{0}>", GetProxyTypeName(methodInstance.ReturnType)));
                        codeString.Append("(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, parameters);");
                    }
                }
                else
                {
                    if (parametersStr.Count == 0)
                    {
                        codeString.Append("client.Invoke(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, null);");
                    }
                    else if (isOut || isRef)
                    {
                        codeString.Append("client.Invoke(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption,ref parameters,types);");
                    }
                    else
                    {
                        codeString.Append("client.Invoke(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, parameters);");
                    }
                }
                if (isOut || isRef)
                {
                    codeString.AppendLine("if(parameters!=null)");
                    codeString.AppendLine("{");
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        codeString.AppendLine(string.Format("{0}=({1})parameters[{2}];", parameters[i].Name, GetProxyTypeName(parameters[i].ParameterType), i));
                    }
                    codeString.AppendLine("}");
                    if (isOut)
                    {
                        codeString.AppendLine("else");
                        codeString.AppendLine("{");
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].IsOut)
                            {
                                codeString.AppendLine(string.Format("{0}=default({1});", parameters[i].Name, GetProxyTypeName(parameters[i].ParameterType)));
                            }
                        }
                        codeString.AppendLine("}");
                    }
                }

                if (methodInstance.HasReturn)
                {
                    codeString.AppendLine("return returnData;");
                }

                codeString.AppendLine("}");
            }

            //以下生成异步
            if (GeneratorFlag.HasFlag(CodeGeneratorFlag.ExtensionAsync) && !isOut && !isRef)//没有out或者ref
            {
                codeString.AppendLine("///<summary>");
                codeString.AppendLine($"///{description}");
                codeString.AppendLine("///</summary>");
                codeString.Append("public static ");
                codeString.Append(GetReturn(methodInstance, true));
                codeString.Append(" ");
                codeString.Append(GetMethodName(methodInstance, true));
                codeString.Append("<TClient>(");//方法参数

                codeString.Append($"this TClient client");

                codeString.Append(",");
                for (int i = 0; i < parametersStr.Count; i++)
                {
                    if (i > 0)
                    {
                        codeString.Append(",");
                    }
                    codeString.Append(parametersStr[i]);
                }
                if (parametersStr.Count > 0)
                {
                    codeString.Append(",");
                }
                codeString.Append(GetInvokeOption());
                codeString.AppendLine(") where TClient:");

                for (int i = 0; i < InterfaceTypes.Length; i++)
                {
                    if (i > 0)
                    {
                        codeString.Append(",");
                    }

                    codeString.Append(InterfaceTypes[i].FullName);
                }

                codeString.AppendLine("{");//方法开始

                codeString.AppendLine("if (client.TryCanInvoke?.Invoke(client)==false)");
                codeString.AppendLine("{");
                codeString.AppendLine($"throw new RpcException(\"Rpc无法执行。\");");
                codeString.AppendLine("}");

                if (parametersStr.Count > 0)
                {
                    codeString.Append($"object[] parameters = new object[]");
                    codeString.Append("{");
                    foreach (ParameterInfo parameter in parameters)
                    {
                        codeString.Append(parameter.Name);
                        if (parameter != parameters[parameters.Length - 1])
                        {
                            codeString.Append(",");
                        }
                    }
                    codeString.AppendLine("};");
                }

                if (methodInstance.HasReturn)
                {
                    if (parametersStr.Count == 0)
                    {
                        codeString.Append(string.Format("return client.InvokeAsync<{0}>", GetProxyTypeName(methodInstance.ReturnType)));
                        codeString.Append("(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, null);");
                    }
                    else
                    {
                        codeString.Append(string.Format("return client.InvokeAsync<{0}>", GetProxyTypeName(methodInstance.ReturnType)));
                        codeString.Append("(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, parameters);");
                    }
                }
                else
                {
                    if (parametersStr.Count == 0)
                    {
                        codeString.Append("return client.InvokeAsync(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, null);");
                    }
                    else
                    {
                        codeString.Append("return client.InvokeAsync(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, parameters);");
                    }
                }
                codeString.AppendLine("}");
            }
            return codeString.ToString();
        }

        /// <summary>
        /// 获取生成的函数泛型限定名称。默认<see cref="IRpcClient"/>
        /// </summary>
        /// <returns></returns>
        public virtual Type[] GetGenericInterfaceTypes()
        {
            return new Type[] { typeof(IRpcClient) };
        }

        /// <summary>
        /// 获取生成实体类时的代码块
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <returns></returns>
        public virtual string GetInstanceProxyCode(MethodInstance methodInstance)
        {
            StringBuilder codeString = new StringBuilder();

            string description = GetDescription(methodInstance);
            ParameterInfo[] parameters;
            bool isOut;
            bool isRef;
            List<string> parametersStr = GetParameters(methodInstance, out isOut, out isRef, out parameters);
            if (GeneratorFlag.HasFlag(CodeGeneratorFlag.Sync))
            {
                codeString.AppendLine("///<summary>");
                codeString.AppendLine($"///{description}");
                codeString.AppendLine("///</summary>");
                foreach (var item in Exceptions)
                {
                    codeString.AppendLine($"/// <exception cref=\"{item.Key.FullName}\">{item.Value}</exception>");
                }

                codeString.Append("public ");
                codeString.Append(GetReturn(methodInstance, false));
                codeString.Append(" ");
                codeString.Append(GetMethodName(methodInstance, false));
                codeString.Append("(");//方法参数

                for (int i = 0; i < parametersStr.Count; i++)
                {
                    if (i > 0)
                    {
                        codeString.Append(",");
                    }
                    codeString.Append(parametersStr[i]);
                }
                if (parametersStr.Count > 0)
                {
                    codeString.Append(",");
                }
                codeString.Append(GetInvokeOption());
                codeString.AppendLine(")");

                codeString.AppendLine("{");//方法开始

                codeString.AppendLine("if(Client==null)");
                codeString.AppendLine("{");
                codeString.AppendLine("throw new RpcException(\"IRpcClient为空，请先初始化或者进行赋值\");");
                codeString.AppendLine("}");
                codeString.AppendLine("if (Client.TryCanInvoke?.Invoke(Client)==false)");
                codeString.AppendLine("{");
                codeString.AppendLine($"throw new RpcException(\"Rpc无法执行。\");");
                codeString.AppendLine("}");

                if (parametersStr.Count > 0)
                {
                    codeString.Append($"object[] parameters = new object[]");
                    codeString.Append("{");

                    foreach (ParameterInfo parameter in parameters)
                    {
                        if (parameter.ParameterType.Name.Contains("&") && parameter.IsOut)
                        {
                            codeString.Append($"default({GetProxyTypeName(parameter.ParameterType)})");
                        }
                        else
                        {
                            codeString.Append(parameter.Name);
                        }
                        if (parameter != parameters[parameters.Length - 1])
                        {
                            codeString.Append(",");
                        }
                    }
                    codeString.AppendLine("};");

                    if (isOut || isRef)
                    {
                        codeString.Append($"Type[] types = new Type[]");
                        codeString.Append("{");
                        foreach (ParameterInfo parameter in parameters)
                        {
                            codeString.Append($"typeof({GetProxyTypeName(parameter.ParameterType)})");
                            if (parameter != parameters[parameters.Length - 1])
                            {
                                codeString.Append(",");
                            }
                        }
                        codeString.AppendLine("};");
                    }
                }

                if (methodInstance.HasReturn)
                {
                    if (parametersStr.Count == 0)
                    {
                        codeString.Append(string.Format("{0} returnData=Client.Invoke<{0}>", GetProxyTypeName(methodInstance.ReturnType)));
                        codeString.Append("(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, null);");
                    }
                    else if (isOut || isRef)
                    {
                        codeString.Append(string.Format("{0} returnData=Client.Invoke<{0}>", GetProxyTypeName(methodInstance.ReturnType)));
                        codeString.Append("(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption,ref parameters,types);");
                    }
                    else
                    {
                        codeString.Append(string.Format("{0} returnData=Client.Invoke<{0}>", GetProxyTypeName(methodInstance.ReturnType)));
                        codeString.Append("(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, parameters);");
                    }
                }
                else
                {
                    if (parametersStr.Count == 0)
                    {
                        codeString.Append("Client.Invoke(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, null);");
                    }
                    else if (isOut || isRef)
                    {
                        codeString.Append("Client.Invoke(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption,ref parameters,types);");
                    }
                    else
                    {
                        codeString.Append("Client.Invoke(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, parameters);");
                    }
                }
                if (isOut || isRef)
                {
                    codeString.AppendLine("if(parameters!=null)");
                    codeString.AppendLine("{");
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        codeString.AppendLine(string.Format("{0}=({1})parameters[{2}];", parameters[i].Name, GetProxyTypeName(parameters[i].ParameterType), i));
                    }
                    codeString.AppendLine("}");
                    if (isOut)
                    {
                        codeString.AppendLine("else");
                        codeString.AppendLine("{");
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].IsOut)
                            {
                                codeString.AppendLine(string.Format("{0}=default({1});", parameters[i].Name, GetProxyTypeName(parameters[i].ParameterType)));
                            }
                        }
                        codeString.AppendLine("}");
                    }
                }

                if (methodInstance.HasReturn)
                {
                    codeString.AppendLine("return returnData;");
                }

                codeString.AppendLine("}");
            }

            //以下生成异步
            if (GeneratorFlag.HasFlag(CodeGeneratorFlag.Async) && !isOut && !isRef)//没有out或者ref
            {
                codeString.AppendLine("///<summary>");
                codeString.AppendLine($"///{description}");
                codeString.AppendLine("///</summary>");
                codeString.Append("public ");
                codeString.Append(GetReturn(methodInstance, true));
                codeString.Append(" ");
                codeString.Append(GetMethodName(methodInstance, true));
                codeString.Append("(");//方法参数

                for (int i = 0; i < parametersStr.Count; i++)
                {
                    if (i > 0)
                    {
                        codeString.Append(",");
                    }
                    codeString.Append(parametersStr[i]);
                }
                if (parametersStr.Count > 0)
                {
                    codeString.Append(",");
                }
                codeString.Append(GetInvokeOption());
                codeString.AppendLine(")");

                codeString.AppendLine("{");//方法开始

                codeString.AppendLine("if(Client==null)");
                codeString.AppendLine("{");
                codeString.AppendLine("throw new RpcException(\"IRpcClient为空，请先初始化或者进行赋值\");");
                codeString.AppendLine("}");

                codeString.AppendLine("if (Client.TryCanInvoke?.Invoke(Client)==false)");
                codeString.AppendLine("{");
                codeString.AppendLine($"throw new RpcException(\"Rpc无法执行。\");");
                codeString.AppendLine("}");

                if (parametersStr.Count > 0)
                {
                    codeString.Append($"object[] parameters = new object[]");
                    codeString.Append("{");
                    foreach (ParameterInfo parameter in parameters)
                    {
                        codeString.Append(parameter.Name);
                        if (parameter != parameters[parameters.Length - 1])
                        {
                            codeString.Append(",");
                        }
                    }
                    codeString.AppendLine("};");
                }

                if (methodInstance.HasReturn)
                {
                    if (parametersStr.Count == 0)
                    {
                        codeString.Append(string.Format("return Client.InvokeAsync<{0}>", GetProxyTypeName(methodInstance.ReturnType)));
                        codeString.Append("(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, null);");
                    }
                    else
                    {
                        codeString.Append(string.Format("return Client.InvokeAsync<{0}>", GetProxyTypeName(methodInstance.ReturnType)));
                        codeString.Append("(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, parameters);");
                    }
                }
                else
                {
                    if (parametersStr.Count == 0)
                    {
                        codeString.Append("return Client.InvokeAsync(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, null);");
                    }
                    else
                    {
                        codeString.Append("return Client.InvokeAsync(");
                        codeString.Append($"\"{GetInvokenKey(methodInstance)}\"");
                        codeString.AppendLine(",invokeOption, parameters);");
                    }
                }
                codeString.AppendLine("}");
            }
            return codeString.ToString();
        }

        /// <summary>
        /// 获取接口的代理代码
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <returns></returns>
        public virtual string GetInterfaceProxyCode(MethodInstance methodInstance)
        {
            StringBuilder codeString = new StringBuilder();
            bool isOut = false;
            bool isRef = false;
            string description = GetDescription(methodInstance);
            List<string> parameters = GetParameters(methodInstance, out isOut, out isRef, out _);
            if (GeneratorFlag.HasFlag(CodeGeneratorFlag.Sync))
            {
                codeString.AppendLine("///<summary>");
                codeString.AppendLine($"///{description}");
                codeString.AppendLine("///</summary>");
                foreach (var item in Exceptions)
                {
                    codeString.AppendLine($"/// <exception cref=\"{item.Key.FullName}\">{item.Value}</exception>");
                }

                codeString.Append(GetReturn(methodInstance, false));
                codeString.Append(" ");
                codeString.Append(GetMethodName(methodInstance, false));
                codeString.Append("(");//方法参数
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (i > 0)
                    {
                        codeString.Append(",");
                    }
                    codeString.Append(parameters[i]);
                }
                if (parameters.Count > 0)
                {
                    codeString.Append(",");
                }
                codeString.Append(GetInvokeOption());
                codeString.AppendLine(");");
            }

            if (GeneratorFlag.HasFlag(CodeGeneratorFlag.Async) && !isOut && !isRef)//没有out或者ref
            {
                codeString.AppendLine("///<summary>");
                codeString.AppendLine($"///{description}");
                codeString.AppendLine("///</summary>");
                foreach (var item in Exceptions)
                {
                    codeString.AppendLine($"/// <exception cref=\"{item.Key.FullName}\">{item.Value}</exception>");
                }

                codeString.Append(GetReturn(methodInstance, true));
                codeString.Append(" ");
                codeString.Append(GetMethodName(methodInstance, true));
                codeString.Append("(");//方法参数

                for (int i = 0; i < parameters.Count; i++)
                {
                    if (i > 0)
                    {
                        codeString.Append(",");
                    }
                    codeString.Append(parameters[i]);
                }
                if (parameters.Count > 0)
                {
                    codeString.Append(",");
                }
                codeString.Append(GetInvokeOption());
                codeString.AppendLine(");");
            }

            return codeString.ToString();
        }

        /// <summary>
        /// 获取调用键
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <returns></returns>
        public virtual string GetInvokenKey(MethodInstance methodInstance)
        {
            if (!InvokenKey.IsNullOrEmpty())
            {
                return InvokenKey;
            }
            return $"{methodInstance.ServerType.FullName}.{methodInstance.Name}".ToLower();
        }

        /// <summary>
        /// 获取调用配置
        /// </summary>
        /// <returns></returns>
        public virtual string GetInvokeOption()
        {
            return "IInvokeOption invokeOption = default";
        }

        /// <summary>
        /// 获取生成的函数名称
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        public virtual string GetMethodName(MethodInstance methodInstance, bool isAsync)
        {
            string name;
            if (string.IsNullOrEmpty(MethodName))
            {
                name = methodInstance.Name;
            }
            else
            {
                name = MethodName.Format(methodInstance.Name);
            }
            return isAsync ? name + "Async" : name;
        }

        /// <summary>
        /// 获取参数生成
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <param name="isOut"></param>
        /// <param name="isRef"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual List<string> GetParameters(MethodInstance methodInstance, out bool isOut, out bool isRef, out ParameterInfo[] parameters)
        {
            List<string> list = new List<string>();
            isOut = false;
            isRef = false;

            if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
            {
                List<ParameterInfo> infos = new List<ParameterInfo>(methodInstance.Parameters);
                infos.RemoveAt(0);
                parameters = infos.ToArray();
            }
            else
            {
                parameters = methodInstance.Parameters;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                StringBuilder codeString = new StringBuilder();
                if (parameters[i].ParameterType.Name.Contains("&"))
                {
                    if (parameters[i].IsOut)
                    {
                        isOut = true;
                        codeString.Append(string.Format("out {0} {1}", GetProxyTypeName(parameters[i].ParameterType), parameters[i].Name));
                    }
                    else
                    {
                        isRef = true;
                        codeString.Append(string.Format("ref {0} {1}", GetProxyTypeName(parameters[i].ParameterType), parameters[i].Name));
                    }
                }
                else
                {
                    codeString.Append(string.Format("{0} {1}", GetProxyTypeName(parameters[i].ParameterType), parameters[i].Name));
                }

                if (parameters[i].HasDefaultValue)
                {
                    object defaultValue = parameters[i].DefaultValue;
                    if (defaultValue == null)
                    {
                        codeString.Append(string.Format("=null"));
                    }
                    else if (defaultValue.ToString() == string.Empty)
                    {
                        codeString.Append(string.Format("=\"\""));
                    }
                    else if (defaultValue.GetType() == typeof(string))
                    {
                        codeString.Append(string.Format("=\"{0}\"", defaultValue));
                    }
                    else if (defaultValue.GetType() == typeof(bool))
                    {
                        codeString.Append(string.Format("={0}", defaultValue.ToString().ToLower()));
                    }
                    else if (typeof(ValueType).IsAssignableFrom(defaultValue.GetType()))
                    {
                        codeString.Append(string.Format("={0}", defaultValue));
                    }
                }

                list.Add(codeString.ToString());
            }

            return list;
        }

        /// <summary>
        /// 从类型获取代理名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string GetProxyTypeName(Type type)
        {
            return ClassCodeGenerator.GetTypeFullName(type);
        }

        /// <summary>
        /// 获取返回值
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        public virtual string GetReturn(MethodInstance methodInstance, bool isAsync)
        {
            if (isAsync)
            {
                if (methodInstance.ReturnType == null)
                {
                    return "Task";
                }
                else
                {
                    return $"Task<{GetProxyTypeName(methodInstance.ReturnType)}>";
                }
            }
            else
            {
                if (methodInstance.ReturnType == null)
                {
                    return "void";
                }
                else
                {
                    return GetProxyTypeName(methodInstance.ReturnType);
                }
            }
        }

        internal void SetClassCodeGenerator(ClassCodeGenerator classCodeGenerator)
        {
            ClassCodeGenerator = classCodeGenerator;
        }
    }
}