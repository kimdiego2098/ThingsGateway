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
//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;

//namespace ThingsGateway.Foundation.Rpc
//{
//    /// <summary>
//    /// 服务代码生成
//    /// </summary>
//    public class ServerCodeGenerator
//    {
//        private readonly ClassCodeGenerator m_classCodeGenerator;

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="classCodeGenerator"></param>
//        public ServerCodeGenerator(ClassCodeGenerator classCodeGenerator)
//        {
//            this.m_classCodeGenerator = classCodeGenerator;
//        }

//        /// <summary>
//        /// 类代码生成器
//        /// </summary>
//        public ClassCodeGenerator ClassCodeGenerator => this.m_classCodeGenerator;

//        /// <summary>
//        /// 获取接口代码
//        /// </summary>
//        /// <param name="methodInstance"></param>
//        /// <param name="attribute"></param>
//        /// <returns></returns>
//        public string GetInterfaceProxy(MethodInstance methodInstance, RpcAttribute attribute)
//        {
//        }

//        /// <summary>
//        /// 获取函数代码
//        /// </summary>
//        /// <param name="methodInstance"></param>
//        /// <param name="attribute"></param>
//        /// <returns></returns>
//        public string GetMethodProxy(MethodInstance methodInstance, RpcAttribute attribute)
//        {
//           return attribute.GetInstanceProxyCode(methodInstance,this.m_classCodeGenerator);
//        }

//        ///// <summary>
//        ///// 获取函数代码
//        ///// </summary>
//        ///// <param name="methodInstance"></param>
//        ///// <param name="attribute"></param>
//        ///// <returns></returns>
//        //public string GetMethodProxy(MethodInstance methodInstance, RpcAttribute attribute)
//        //{
//        //    StringBuilder codeString = new StringBuilder();

//        //    string description = attribute.GetDescription(methodInstance);
//        //    ParameterInfo[] parameters;
//        //    bool isOut;
//        //    bool isRef;
//        //    List<string> parametersStr = attribute.GetParameters(methodInstance, this.m_classCodeGenerator, out isOut, out isRef, out parameters);
//        //    if (attribute.GeneratorFlag.HasFlag(CodeGeneratorFlag.Sync))
//        //    {
//        //        codeString.AppendLine("///<summary>");
//        //        codeString.AppendLine($"///{description}");
//        //        codeString.AppendLine("///</summary>");
//        //        foreach (var item in attribute.Exceptions)
//        //        {
//        //            codeString.AppendLine($"/// <exception cref=\"{item.Key.FullName}\">{item.Value}</exception>");
//        //        }

//        //        codeString.Append("public ");
//        //        codeString.Append(attribute.GetReturn(methodInstance, this.m_classCodeGenerator, false));
//        //        codeString.Append(" ");
//        //        codeString.Append(attribute.GetMethodName(methodInstance, false));
//        //        codeString.Append("(");//方法参数

//        //        for (int i = 0; i < parametersStr.Count; i++)
//        //        {
//        //            if (i > 0)
//        //            {
//        //                codeString.Append(",");
//        //            }
//        //            codeString.Append(parametersStr[i]);
//        //        }
//        //        if (parametersStr.Count > 0)
//        //        {
//        //            codeString.Append(",");
//        //        }
//        //        codeString.Append(attribute.GetInvokeOption());
//        //        codeString.AppendLine(")");

//        //        codeString.AppendLine("{");//方法开始

//        //        codeString.AppendLine("if(Client==null)");
//        //        codeString.AppendLine("{");
//        //        codeString.AppendLine("throw new RpcException(\"IRpcClient为空，请先初始化或者进行赋值\");");
//        //        codeString.AppendLine("}");
//        //        codeString.AppendLine("if (Client.TryCanInvoke?.Invoke(Client)==false)");
//        //        codeString.AppendLine("{");
//        //        codeString.AppendLine($"throw new RpcException(\"Rpc无法执行。\");");
//        //        codeString.AppendLine("}");

//        //        if (parametersStr.Count > 0)
//        //        {
//        //            codeString.Append($"object[] parameters = new object[]");
//        //            codeString.Append("{");

//        //            foreach (ParameterInfo parameter in parameters)
//        //            {
//        //                if (parameter.ParameterType.Name.Contains("&") && parameter.IsOut)
//        //                {
//        //                    codeString.Append($"default({this.GetName(parameter.ParameterType)})");
//        //                }
//        //                else
//        //                {
//        //                    codeString.Append(parameter.Name);
//        //                }
//        //                if (parameter != parameters[parameters.Length - 1])
//        //                {
//        //                    codeString.Append(",");
//        //                }
//        //            }
//        //            codeString.AppendLine("};");

//        //            if (isOut || isRef)
//        //            {
//        //                codeString.Append($"Type[] types = new Type[]");
//        //                codeString.Append("{");
//        //                foreach (ParameterInfo parameter in parameters)
//        //                {
//        //                    codeString.Append($"typeof({this.GetName(parameter.ParameterType)})");
//        //                    if (parameter != parameters[parameters.Length - 1])
//        //                    {
//        //                        codeString.Append(",");
//        //                    }
//        //                }
//        //                codeString.AppendLine("};");
//        //            }
//        //        }

//        //        if (methodInstance.HasReturn)
//        //        {
//        //            if (parametersStr.Count == 0)
//        //            {
//        //                codeString.Append(string.Format("{0} returnData=Client.Invoke<{0}>", this.GetName(methodInstance.ReturnType)));
//        //                codeString.Append("(");
//        //                codeString.Append($"\"{attribute.GetInvokenKey(methodInstance)}\"");
//        //                codeString.AppendLine(",invokeOption, null);");
//        //            }
//        //            else if (isOut || isRef)
//        //            {
//        //                codeString.Append(string.Format("{0} returnData=Client.Invoke<{0}>", this.GetName(methodInstance.ReturnType)));
//        //                codeString.Append("(");
//        //                codeString.Append($"\"{attribute.GetInvokenKey(methodInstance)}\"");
//        //                codeString.AppendLine(",invokeOption,ref parameters,types);");
//        //            }
//        //            else
//        //            {
//        //                codeString.Append(string.Format("{0} returnData=Client.Invoke<{0}>", this.GetName(methodInstance.ReturnType)));
//        //                codeString.Append("(");
//        //                codeString.Append($"\"{attribute.GetInvokenKey(methodInstance)}\"");
//        //                codeString.AppendLine(",invokeOption, parameters);");
//        //            }
//        //        }
//        //        else
//        //        {
//        //            if (parametersStr.Count == 0)
//        //            {
//        //                codeString.Append("Client.Invoke(");
//        //                codeString.Append($"\"{attribute.GetInvokenKey(methodInstance)}\"");
//        //                codeString.AppendLine(",invokeOption, null);");
//        //            }
//        //            else if (isOut || isRef)
//        //            {
//        //                codeString.Append("Client.Invoke(");
//        //                codeString.Append($"\"{attribute.GetInvokenKey(methodInstance)}\"");
//        //                codeString.AppendLine(",invokeOption,ref parameters,types);");
//        //            }
//        //            else
//        //            {
//        //                codeString.Append("Client.Invoke(");
//        //                codeString.Append($"\"{attribute.GetInvokenKey(methodInstance)}\"");
//        //                codeString.AppendLine(",invokeOption, parameters);");
//        //            }
//        //        }
//        //        if (isOut || isRef)
//        //        {
//        //            codeString.AppendLine("if(parameters!=null)");
//        //            codeString.AppendLine("{");
//        //            for (int i = 0; i < parameters.Length; i++)
//        //            {
//        //                codeString.AppendLine(string.Format("{0}=({1})parameters[{2}];", parameters[i].Name, this.GetName(parameters[i].ParameterType), i));
//        //            }
//        //            codeString.AppendLine("}");
//        //            if (isOut)
//        //            {
//        //                codeString.AppendLine("else");
//        //                codeString.AppendLine("{");
//        //                for (int i = 0; i < parameters.Length; i++)
//        //                {
//        //                    if (parameters[i].IsOut)
//        //                    {
//        //                        codeString.AppendLine(string.Format("{0}=default({1});", parameters[i].Name, this.GetName(parameters[i].ParameterType)));
//        //                    }
//        //                }
//        //                codeString.AppendLine("}");
//        //            }
//        //        }

//        //        if (methodInstance.HasReturn)
//        //        {
//        //            codeString.AppendLine("return returnData;");
//        //        }

//        //        codeString.AppendLine("}");
//        //    }

//        //    //以下生成异步
//        //    if (attribute.GeneratorFlag.HasFlag(CodeGeneratorFlag.Async) && !isOut && !isRef)//没有out或者ref
//        //    {
//        //        codeString.AppendLine("///<summary>");
//        //        codeString.AppendLine($"///{description}");
//        //        codeString.AppendLine("///</summary>");
//        //        codeString.Append("public ");
//        //        codeString.Append(attribute.GetReturn(methodInstance, this.m_classCodeGenerator, true));
//        //        codeString.Append(" ");
//        //        codeString.Append(attribute.GetMethodName(methodInstance, true));
//        //        codeString.Append("(");//方法参数

//        //        for (int i = 0; i < parametersStr.Count; i++)
//        //        {
//        //            if (i > 0)
//        //            {
//        //                codeString.Append(",");
//        //            }
//        //            codeString.Append(parametersStr[i]);
//        //        }
//        //        if (parametersStr.Count > 0)
//        //        {
//        //            codeString.Append(",");
//        //        }
//        //        codeString.Append(attribute.GetInvokeOption());
//        //        codeString.AppendLine(")");

//        //        codeString.AppendLine("{");//方法开始

//        //        codeString.AppendLine("if(Client==null)");
//        //        codeString.AppendLine("{");
//        //        codeString.AppendLine("throw new RpcException(\"IRpcClient为空，请先初始化或者进行赋值\");");
//        //        codeString.AppendLine("}");

//        //        codeString.AppendLine("if (Client.TryCanInvoke?.Invoke(Client)==false)");
//        //        codeString.AppendLine("{");
//        //        codeString.AppendLine($"throw new RpcException(\"Rpc无法执行。\");");
//        //        codeString.AppendLine("}");

//        //        if (parametersStr.Count > 0)
//        //        {
//        //            codeString.Append($"object[] parameters = new object[]");
//        //            codeString.Append("{");
//        //            foreach (ParameterInfo parameter in parameters)
//        //            {
//        //                codeString.Append(parameter.Name);
//        //                if (parameter != parameters[parameters.Length - 1])
//        //                {
//        //                    codeString.Append(",");
//        //                }
//        //            }
//        //            codeString.AppendLine("};");
//        //        }

//        //        if (methodInstance.HasReturn)
//        //        {
//        //            if (parametersStr.Count == 0)
//        //            {
//        //                codeString.Append(string.Format("return Client.InvokeAsync<{0}>", this.GetName(methodInstance.ReturnType)));
//        //                codeString.Append("(");
//        //                codeString.Append($"\"{attribute.GetInvokenKey(methodInstance)}\"");
//        //                codeString.AppendLine(",invokeOption, null);");
//        //            }
//        //            else
//        //            {
//        //                codeString.Append(string.Format("return Client.InvokeAsync<{0}>", this.GetName(methodInstance.ReturnType)));
//        //                codeString.Append("(");
//        //                codeString.Append($"\"{attribute.GetInvokenKey(methodInstance)}\"");
//        //                codeString.AppendLine(",invokeOption, parameters);");
//        //            }
//        //        }
//        //        else
//        //        {
//        //            if (parametersStr.Count == 0)
//        //            {
//        //                codeString.Append("return Client.InvokeAsync(");
//        //                codeString.Append($"\"{attribute.GetInvokenKey(methodInstance)}\"");
//        //                codeString.AppendLine(",invokeOption, null);");
//        //            }
//        //            else
//        //            {
//        //                codeString.Append("return Client.InvokeAsync(");
//        //                codeString.Append($"\"{attribute.GetInvokenKey(methodInstance)}\"");
//        //                codeString.AppendLine(",invokeOption, parameters);");
//        //            }
//        //        }
//        //        codeString.AppendLine("}");
//        //    }
//        //    return codeString.ToString();
//        //}

//        /// <summary>
//        /// 获取扩展函数代码
//        /// </summary>
//        /// <param name="methodInstance"></param>
//        /// <param name="attribute"></param>
//        /// <returns></returns>
//        public string GetExtensionsMethodProxy(MethodInstance methodInstance, RpcAttribute attribute)
//        {
//        }
//    }
//}