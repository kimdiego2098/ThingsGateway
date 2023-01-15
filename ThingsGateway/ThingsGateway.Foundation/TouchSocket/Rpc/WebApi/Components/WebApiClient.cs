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
.Threading.Tasks;

using ThingsGateway.Foundation.Rpc.TouchRpc;

namespace ThingsGateway.Foundation.Rpc.WebApi
{
    /// <summary>
    /// WebApi客户端
    /// </summary>
    public class WebApiClient : HttpClientBase, IWebApiClient
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WebApiClient()
        {
            m_stringConverter = new StringConverter();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Func<IRpcClient, bool> TryCanInvoke { get; set; }

        private readonly StringConverter m_stringConverter;

        /// <summary>
        /// 字符串转化器
        /// </summary>
        public StringConverter StringConverter => m_stringConverter;

        #region RPC调用

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
            string[] strs = method.Split(':');
            if (strs.Length != 2)
            {
                throw new RpcException("不是有效的url请求。");
            }
            if (invokeOption == default)
            {
                invokeOption = InvokeOption.WaitInvoke;
            }

            HttpRequest request = new HttpRequest();

            switch (strs[0])
            {
                case TouchSocketHttpUtility.Get:
                    {
                        request.InitHeaders()
                            .SetHost(RemoteIPHost.Host)
                            .SetUrl(strs[1].Format(parameters))
                            .AsGet();
                        break;
                    }
                case TouchSocketHttpUtility.Post:
                    {
                        request.InitHeaders()
                            .SetHost(RemoteIPHost.Host)
                            .SetUrl(strs[1].Format(parameters))
                            .AsPost();
                        if (parameters.Length > 0)
                        {
                            request.FromJson(parameters[parameters.Length - 1].ToJson());
                        }
                        break;
                    }
                default:
                    break;
            }

            HttpResponse response = RequestContent(request, false, invokeOption.Timeout, invokeOption.Token);

            if (invokeOption.FeedbackType != FeedbackType.WaitInvoke)
            {
                return default;
            }

            if (response.StatusCode == "200")
            {
                return (T)m_stringConverter.ConvertFrom(response.GetBody(), typeof(T));
            }
            else if (response.StatusCode == "422")
            {
                throw new RpcException(response.GetBody().FromJson<ActionResult>().Message);
            }
            else
            {
                throw new RpcException(response.StatusMessage);
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
            string[] strs = method.Split(':');
            if (strs.Length != 2)
            {
                throw new RpcException("不是有效的url请求。");
            }
            if (invokeOption == default)
            {
                invokeOption = InvokeOption.WaitInvoke;
            }

            HttpRequest request = new HttpRequest();

            switch (strs[0])
            {
                case TouchSocketHttpUtility.Get:
                    {
                        request.InitHeaders()
                            .SetHost(RemoteIPHost.Host)
                            .SetUrl(strs[1].Format(parameters))
                            .AsGet();
                        break;
                    }
                case TouchSocketHttpUtility.Post:
                    {
                        request.InitHeaders()
                        .SetHost(RemoteIPHost.Host)
                        .SetUrl(strs[1].Format(parameters))
                        .AsPost();
                        if (parameters.Length > 0)
                        {
                            request.FromJson(parameters[parameters.Length - 1].ToJson());
                        }
                        break;
                    }
                default:
                    break;
            }
            HttpResponse response = RequestContent(request, false, invokeOption.Timeout, invokeOption.Token);
            if (invokeOption.FeedbackType != FeedbackType.WaitInvoke)
            {
                return;
            }

            if (response.StatusCode == "200")
            {
                return;
            }
            else if (response.StatusCode == "422")
            {
                throw new RpcException(response.GetBody().FromJson<ActionResult>().Message);
            }
            else
            {
                throw new RpcException(response.StatusMessage);
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
        public Task InvokeAsync(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return EasyTask.Run(() =>
            {
                Invoke(method, invokeOption, parameters);
            });
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
        public Task<T> InvokeAsync<T>(string method, IInvokeOption invokeOption, params object[] parameters)
        {
            return EasyTask.Run(() =>
            {
                return Invoke<T>(method, invokeOption, parameters);
            });
        }

        #endregion RPC调用
    }
}