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
using Microsoft.AspNetCore.Http;


using Microsoft.Extensions.Logging;

using System.Threading.Tasks;
namespace ThingsGateway.Foundation.AspNetCore
{
    /// <summary>
    /// WSTouchRpc中间件
    /// </summary>
    public class WSTouchRpcMiddleware
    {
        private readonly RequestDelegate m_next;
        private readonly IWSTouchRpcService m_rpcService;
        private string m_url = "/wstouchrpc";
        private readonly ILogger<WSTouchRpcMiddleware> m_logger;

        /// <summary>
        /// 实例化一个中间件
        /// </summary>
        /// <param name="m_url"></param>
        /// <param name="next"></param>
        /// <param name="rpcService"></param>
        /// <param name="loggerFactory"></param>
        public WSTouchRpcMiddleware(string m_url, RequestDelegate next, IWSTouchRpcService rpcService, ILoggerFactory loggerFactory)
        {
            Url = m_url;
            m_next = next ?? throw new ArgumentNullException(nameof(next));
            m_rpcService = rpcService;
            m_logger = loggerFactory.CreateLogger<WSTouchRpcMiddleware>();
        }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get => m_url; set => m_url = string.IsNullOrEmpty(value) ? "/wstouchrpc" : value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<Task> Invoke(HttpContext context)
        {
            if (context.Request.Path.Equals(Url, StringComparison.CurrentCultureIgnoreCase))
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    try
                    {
                        await m_rpcService.SwitchClientAsync(webSocket);
                    }
                    catch (Exception ex)
                    {
                        m_logger.LogError(ex.Message);
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
                return Task.CompletedTask;
            }
            else
            {
                return m_next(context);
            }
        }
    }
}