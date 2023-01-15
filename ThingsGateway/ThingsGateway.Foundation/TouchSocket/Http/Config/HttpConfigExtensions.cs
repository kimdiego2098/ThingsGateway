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

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// HttpConfigExtensions
    /// </summary>
    public static class HttpConfigExtensions
    {
        #region 创建

        /// <summary>
        /// 构建Http类客户端，并连接
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        public static TClient BuildWithHttpClient<TClient>(this TouchSocketConfig config) where TClient : IHttpClient
        {
            TClient client = config.Container.Resolve<TClient>();
            client.Setup(config);
            client.Connect();
            return client;
        }

        /// <summary>
        /// 构建Http类客户端，并连接
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static HttpClient BuildWithHttpClient(this TouchSocketConfig config)
        {
            return BuildWithHttpClient<HttpClient>(config);
        }

        /// <summary>
        /// 构建Http类服务器，并启动。
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        public static TService BuildWithHttpService<TService>(this TouchSocketConfig config) where TService : IHttpService
        {
            TService service = config.Container.Resolve<TService>();
            service.Setup(config);
            service.Start();
            return service;
        }

        /// <summary>
        /// 构建Http类服务器，并启动。
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static HttpService BuildWithHttpService(this TouchSocketConfig config)
        {
            return BuildWithHttpService<HttpService>(config);
        }

        #endregion 创建

        /// <summary>
        /// Http代理
        /// </summary>
        public static readonly DependencyProperty<HttpProxy> HttpProxyProperty =
            DependencyProperty<HttpProxy>.Register("HttpProxy", typeof(HttpConfigExtensions), null);

        /// <summary>
        ///设置Http代理
        /// </summary>
        /// <param name="config"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TouchSocketConfig SetHttpProxy(this TouchSocketConfig config, HttpProxy value)
        {
            config.SetValue(HttpProxyProperty, value);
            return config;
        }
    }
}