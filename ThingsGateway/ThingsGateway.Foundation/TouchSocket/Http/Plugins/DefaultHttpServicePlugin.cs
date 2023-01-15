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



namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// 默认的Http服务。为Http做兜底拦截。该插件应该最后添加。
    /// </summary>
    public class DefaultHttpServicePlugin : HttpPluginBase<HttpSocketClient>
    {
        /// <summary>
        /// 默认的Http服务。为Http做兜底拦截。该插件应该最后添加。
        /// </summary>
        public DefaultHttpServicePlugin()
        {
            Order = int.MinValue;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        protected override void OnLoadingConfig(object sender, ConfigEventArgs e)
        {
            if (!(sender is IService))
            {
                throw new Exception("该插件仅可用于服务器。");
            }
            base.OnLoadingConfig(sender, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnGet(HttpSocketClient client, HttpContextEventArgs e)
        {
            e.Context.Response.UrlNotFind().Answer();
            base.OnGet(client, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnPost(HttpSocketClient client, HttpContextEventArgs e)
        {
            e.Context.Response.UrlNotFind().Answer();
            base.OnPost(client, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnPut(HttpSocketClient client, HttpContextEventArgs e)
        {
            e.Context.Response.UrlNotFind().Answer();
            base.OnPut(client, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnDelete(HttpSocketClient client, HttpContextEventArgs e)
        {
            e.Context.Response.UrlNotFind().Answer();
            base.OnDelete(client, e);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnReceivedOtherHttpRequest(HttpSocketClient client, HttpContextEventArgs e)
        {
            switch (e.Context.Request.Method.ToUpper())
            {
                case "OPTIONS":
                    {
                        e.Context.Response
                            .SetStatus()
                            .SetHeader("Access-Control-Allow-Origin", "*")
                            .SetHeader("Access-Control-Allow-Headers", "*")
                            .SetHeader("Allow", "OPTIONS, GET, POST")
                            .SetHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS")
                            .Answer();
                        break;
                    }
                default:
                    e.Context.Response.UrlNotFind().Answer();
                    break;
            }
            base.OnReceivedOtherHttpRequest(client, e);
        }
    }
}