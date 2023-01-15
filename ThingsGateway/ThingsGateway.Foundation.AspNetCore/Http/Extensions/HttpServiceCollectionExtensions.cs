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
using ThingsGateway.Foundation.Http;
using ThingsGateway.Foundation.Sockets;

using TouchSocket.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// HttpServiceCollectionExtensions
    /// </summary>
    public static class HttpServiceCollectionExtensions
    {
        #region HttpService

        /// <summary>
        /// 添加HttpService服务。
        /// </summary>
        /// <typeparam name="ServiceInterface">注册服务的接口</typeparam>
        /// <param name="service"></param>
        /// <param name="tcpService">实例化的服务器</param>
        /// <returns></returns>
        public static IHttpService AddHttpService<ServiceInterface>(this IServiceCollection service, ITcpService tcpService) where ServiceInterface : IHttpService
        {
            tcpService.Config.Container.RegisterSingleton<ServiceInterface>(tcpService);
            return (ServiceInterface)tcpService;
        }

        /// <summary>
        ///  添加HttpService服务。
        /// </summary>
        /// <typeparam name="ServiceInterface">注册服务的接口</typeparam>
        /// <param name="service"></param>
        /// <param name="configAction">设置配置相关信息</param>
        /// <returns></returns>
        public static IHttpService AddHttpService<ServiceInterface>(this IServiceCollection service, Action<TouchSocketConfig> configAction) where ServiceInterface : IHttpService
        {
            var config = new TouchSocketConfig()
                .UseAspNetCoreContainer(service);
            configAction?.Invoke(config);
            IHttpService tcpService = new HttpService();
            tcpService.Setup(config)
                .Start();

            return AddHttpService<ServiceInterface>(service, tcpService);
        }

        /// <summary>
        ///  以<see cref="IHttpService"/>作为注入接口，添加HttpService服务。
        /// </summary>
        /// <param name="service"></param>
        /// <param name="configAction">设置配置相关信息</param>
        /// <returns></returns>
        public static IHttpService AddHttpService(this IServiceCollection service, Action<TouchSocketConfig> configAction)
        {
            return AddHttpService<IHttpService>(service, configAction);
        }

        /// <summary>
        /// 以<see cref="IHttpService"/>作为注入接口，添加HttpService服务。
        /// </summary>
        /// <param name="service"></param>
        /// <param name="port">监听端口</param>
        /// <returns></returns>
        public static IHttpService AddHttpService(this IServiceCollection service, int port)
        {
            return AddHttpService<IHttpService>(service, config =>
            {
                config.SetListenIPHosts(new IPHost[] { new IPHost(port) });
            });
        }

        #endregion HttpService
    }
}