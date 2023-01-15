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

using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Http.WebSockets
{
    /// <summary>
    /// WebSocketServerExtensions
    /// </summary>
    public static class WebSocketServerExtensions
    {
        /// <summary>
        /// 转化Protocol协议标识为<see cref="Protocol.WebSocket"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="httpContext">Http上下文</param>
        public static bool SwitchProtocolToWebSocket<TClient>(this TClient client, HttpContext httpContext) where TClient : HttpSocketClient
        {
            if (client.Protocol == Protocol.WebSocket)
            {
                return true;
            }
            if (client.Protocol == Protocol.Http)
            {
                if (WSTools.TryGetResponse(httpContext.Request, httpContext.Response))
                {
                    HttpContextEventArgs args = new HttpContextEventArgs(new HttpContext(httpContext.Request, httpContext.Response))
                    {
                        IsPermitOperation = true
                    };
                    client.PluginsManager?.Raise<IWebSocketPlugin>(nameof(IWebSocketPlugin.OnHandshaking), client, args);

                    if (args.IsPermitOperation)
                    {
                        client.SetDataHandlingAdapter(new WebSocketDataHandlingAdapter());
                        client.Protocol = Protocol.WebSocket;
                        client.SetValue(WebSocketServerPlugin.HandshakedProperty, true);//设置握手状态

                        using (ByteBlock byteBlock = new ByteBlock())
                        {
                            args.Context.Response.Build(byteBlock);
                            client.DefaultSend(byteBlock);
                        }
                        client.PluginsManager?.Raise<IWebSocketPlugin>(nameof(IWebSocketPlugin.OnHandshaked), client, new HttpContextEventArgs(httpContext));
                        return true;
                    }
                    else
                    {
                        args.Context.Response.SetStatus("403", "Forbidden");
                        using (ByteBlock byteBlock = new ByteBlock())
                        {
                            args.Context.Response.Build(byteBlock);
                            client.DefaultSend(byteBlock);
                        }

                        client.Close("主动拒绝WebSocket连接");
                    }
                }
                else
                {
                    client.Close("WebSocket连接协议不正确");
                }
            }
            return false;
        }

        /// <summary>
        /// 转化Protocol协议标识为<see cref="Protocol.WebSocket"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="httpContext">Http上下文</param>
        public static Task<bool> SwitchProtocolToWebSocketAsync<TClient>(this TClient client, HttpContext httpContext) where TClient : HttpSocketClient
        {
            return EasyTask.Run(() =>
             {
                 return SwitchProtocolToWebSocket(client, httpContext);
             });
        }
    }
}