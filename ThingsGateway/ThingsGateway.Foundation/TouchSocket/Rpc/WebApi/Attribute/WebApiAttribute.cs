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
using System.Text;

namespace ThingsGateway.Foundation.Rpc.WebApi
{
    /// <summary>
    /// WebApiAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class WebApiAttribute : RpcAttribute
    {
        private readonly HttpMethodType m_method;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="method"></param>
        public WebApiAttribute(HttpMethodType method)
        {
            m_method = method;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override Type[] GetGenericInterfaceTypes()
        {
            return new Type[] { typeof(IWebApiClient) };
        }

        /// <summary>
        /// 函数类型。
        /// </summary>
        public HttpMethodType Method => m_method;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <returns></returns>
        public override string GetInvokenKey(MethodInstance methodInstance)
        {
            int i = 0;
            if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
            {
                i = 1;
            }
            if (m_method == HttpMethodType.GET)
            {
                string actionUrl = GetRouteUrls(methodInstance)[0];
                if (methodInstance.ParameterNames.Length > i)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(actionUrl);
                    stringBuilder.Append("?");
                    for (; i < methodInstance.ParameterNames.Length; i++)
                    {
                        stringBuilder.Append(methodInstance.ParameterNames[i] + "={&}".Replace("&", i.ToString()));
                        if (i != methodInstance.ParameterNames.Length - 1)
                        {
                            stringBuilder.Append("&");
                        }
                    }
                    actionUrl = stringBuilder.ToString();
                }
                return $"GET:{actionUrl}";
            }
            else if (m_method == HttpMethodType.POST)
            {
                string actionUrl = GetRouteUrls(methodInstance)[0];
                if (methodInstance.ParameterNames.Length > i + 1)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(actionUrl);
                    stringBuilder.Append("?");
                    for (; i < methodInstance.ParameterNames.Length - 1; i++)
                    {
                        stringBuilder.Append(methodInstance.ParameterNames[i] + "={&}".Replace("&", i.ToString()));
                        if (i != methodInstance.ParameterNames.Length - 2)
                        {
                            stringBuilder.Append("&");
                        }
                    }
                    actionUrl = stringBuilder.ToString();
                }
                return $"POST:{actionUrl}";
            }
            else
            {
                return base.GetInvokenKey(methodInstance);
            }
        }

        /// <summary>
        /// 获取路由路径。
        /// <para>路由路径的第一个值会被当做调用值。</para>
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <returns></returns>
        public virtual string[] GetRouteUrls(MethodInstance methodInstance)
        {
            if (methodInstance.GetAttribute<WebApiAttribute>() is WebApiAttribute webApiAttribute)
            {
                List<string> urls = new List<string>();

                var attrs = methodInstance.Info.GetCustomAttributes(typeof(RouterAttribute), true);
                if (attrs.Length > 0)
                {
                    foreach (RouterAttribute item in attrs)
                    {
                        string url = item.RouteTemple.ToLower()
                            .Replace("[api]", methodInstance.ServerType.Name)
                            .Replace("[action]", webApiAttribute.GetMethodName(methodInstance, false)).ToLower();

                        if (!urls.Contains(url))
                        {
                            urls.Add(url);
                        }
                    }
                }
                else
                {
                    attrs = methodInstance.ServerType.GetCustomAttributes(typeof(RouterAttribute), true);
                    if (attrs.Length > 0)
                    {
                        foreach (RouterAttribute item in attrs)
                        {
                            string url = item.RouteTemple.ToLower()
                                .Replace("[api]", methodInstance.ServerType.Name)
                                .Replace("[action]", webApiAttribute.GetMethodName(methodInstance, false)).ToLower();

                            if (!urls.Contains(url))
                            {
                                urls.Add(url);
                            }
                        }
                    }
                    else
                    {
                        urls.Add($"/{methodInstance.Info.DeclaringType.Name}/{methodInstance.Name}".ToLower());
                    }
                }

                return urls.ToArray();
            }
            return default;
        }
    }
}