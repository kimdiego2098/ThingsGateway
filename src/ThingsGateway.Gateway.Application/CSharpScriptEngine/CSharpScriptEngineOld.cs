﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://diego2098.gitee.io/thingsgateway-docs/
//  QQ群：605534569
//------------------------------------------------------------------------------

//using CSScriptLib;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

//using System.Dynamic;
//using System.Text;

//using ThingsGateway.Core.Extension.Json;

//namespace ThingsGateway.Gateway.Application;

///// <summary>
///// C#脚本扩展使用
/////代码修改自IoTSharp：https://gitee.com/IoTSharp/IoTSharp
///// </summary>
//public class CSharpScriptEngine
//{
//    /// <summary>
//    /// 执行脚本获取返回值，通常用于上传实体返回脚本，参数为input
//    /// </summary>
//    public string DoList(string _source, string input)
//    {
//        var cacheKey = $"{nameof(CSharpScriptEngine)}-{DoList}-{_source}";
//        var runscript = NewLife.Caching.Cache.Default.GetOrAdd(cacheKey, c =>
//        {
//            var src = _source.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.TrimEntries);
//            StringBuilder _using = new();
//            StringBuilder _body = new();
//            src.ToList().ForEach(l =>
//            {
//                if (l.StartsWith("using "))
//                {
//                    _using.AppendLine(l);
//                }
//                else
//                {
//                    _body.AppendLine(l);
//                }
//            });
//            var eva = CSScript.Evaluator
//                   .CreateDelegate(@$"
//                                    using System;
//                                    using System.Collections.Generic;
//                                    {_using}
//                                    dynamic  runscript(dynamic   input)
//                                    {{
//                                      {_body}
//                                    }}");
//            return eva;
//        }, 3600);
//        var expConverter = new ExpandoObjectConverter();
//        dynamic obj = JsonConvert.DeserializeObject<List<ExpandoObject>>(input, expConverter);
//        object result = runscript(obj);
//        var json = result.ToJsonString();
//        return json;
//    }

//    /// <summary>
//    /// 执行脚本获取返回值，通常用于上传实体返回脚本，参数为input
//    /// </summary>
//    public string Do(string _source, string input)
//    {
//        var cacheKey = $"{nameof(CSharpScriptEngine)}-{Do}-{_source}";
//        var runscript = NewLife.Caching.Cache.Default.GetOrAdd(cacheKey, c =>
//        {
//            var src = _source.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.TrimEntries);
//            StringBuilder _using = new();
//            StringBuilder _body = new();
//            src.ToList().ForEach(l =>
//            {
//                if (l.StartsWith("using "))
//                {
//                    _using.AppendLine(l);
//                }
//                else
//                {
//                    _body.AppendLine(l);
//                }
//            });
//            var eva = CSScript.Evaluator
//                   .CreateDelegate(@$"
//                                    using System;
//                                    using System.Collections.Generic;
//                                    {_using}
//                                    dynamic  runscript(dynamic   input)
//                                    {{
//                                      {_body}
//                                    }}");
//            return eva;
//        }, 3600);
//        var expConverter = new ExpandoObjectConverter();
//        dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(input, expConverter);
//        object result = runscript(obj);
//        var json = result.ToJsonString();
//        return json;
//    }
//}