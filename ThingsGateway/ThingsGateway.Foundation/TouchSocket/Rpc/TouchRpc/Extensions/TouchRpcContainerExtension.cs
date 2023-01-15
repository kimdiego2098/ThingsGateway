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

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// TouchRpcContainerExtension
    /// </summary>
    public static class TouchRpcContainerExtension
    {
        /// <summary>
        /// 设置文件资源控制器。
        /// </summary>
        /// <param name="container"></param>
        /// <param name="value"></param>
        public static IContainer SetFileResourceController(this IContainer container, IFileResourceController value)
        {
            return container.RegisterSingleton<IFileResourceController>(value);
        }

        /// <summary>
        /// 设置文件资源控制器。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        public static IContainer SetFileResourceController<T>(this IContainer container) where T : class, IFileResourceController
        {
            return container.RegisterSingleton<IFileResourceController, T>();
        }

        /// <summary>
        /// 默认的全局资源控制器。
        /// </summary>
        public static readonly FileResourceController FileController = new FileResourceController();

        /// <summary>
        /// 获取文件资源控制器。如果没有注册的话，会新建一个<see cref="FileResourceController"/>
        /// </summary>
        /// <param name="container"></param>
        public static IFileResourceController GetFileResourceController(this IContainer container)
        {
            if (container.IsRegistered(typeof(IFileResourceController)))
            {
                return container.Resolve<IFileResourceController>();
            }
            return FileController;
        }
    }
}