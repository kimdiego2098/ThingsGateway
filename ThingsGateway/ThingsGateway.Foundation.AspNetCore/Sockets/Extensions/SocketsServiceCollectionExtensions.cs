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
using ThingsGateway.Foundation.Sockets;

using TouchSocket.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// SocketsServiceCollectionExtensions
    /// </summary>
    public static class SocketsServiceCollectionExtensions
    {
        #region TcpService
        /// <summary>
        /// 添加TcpService服务。
        /// </summary>
        /// <typeparam name="ServiceInterface">注册服务的接口</typeparam>
        /// <param name="service"></param>
        /// <param name="tcpService">实例化的服务器</param>
        /// <returns></returns>
        public static ITcpService AddTcpService<ServiceInterface>(this IServiceCollection service, ITcpService tcpService) where ServiceInterface : ITcpService
        {
            tcpService.Config.Container.RegisterSingleton<ServiceInterface>(tcpService);
            return (ServiceInterface)tcpService;
        }

        /// <summary>
        /// 添加TcpService服务。
        /// </summary>
        /// <typeparam name="ServiceInterface">注册服务的接口</typeparam>
        /// <param name="service"></param>
        /// <param name="configAction">设置配置相关信息</param>
        /// <returns></returns>
        public static ITcpService AddTcpService<ServiceInterface>(this IServiceCollection service, Action<TouchSocketConfig> configAction) where ServiceInterface : ITcpService
        {
            var config = new TouchSocketConfig()
                .UseAspNetCoreContainer(service);
            configAction?.Invoke(config);
            ITcpService tcpService = new TcpService();
            tcpService.Setup(config)
                .Start();

            return AddTcpService<ServiceInterface>(service, tcpService);
        }

        /// <summary>
        ///  以<see cref="ITcpService"/>作为注入接口，添加TcpService服务。
        /// </summary>
        /// <param name="service"></param>
        /// <param name="configAction">设置配置相关信息</param>
        /// <returns></returns>
        public static ITcpService AddTcpService(this IServiceCollection service, Action<TouchSocketConfig> configAction)
        {
            return AddTcpService<ITcpService>(service, configAction);
        }

        /// <summary>
        /// 以<see cref="ITcpService"/>作为注入接口，添加TcpService服务。
        /// </summary>
        /// <param name="service"></param>
        /// <param name="port">监听端口</param>
        /// <returns></returns>
        public static ITcpService AddTcpService(this IServiceCollection service, int port)
        {
            return AddTcpService<ITcpService>(service, config =>
            {
                config.SetListenIPHosts(new IPHost[] { new IPHost(port) });
            });
        }

        #endregion TcpService
    }
}
