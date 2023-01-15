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
    /// 服务器接口
    /// </summary>
    public interface IService : IDisposable
    {
        /// <summary>
        /// 服务器状态
        /// </summary>
        ServerState ServerState { get; }

        /// <summary>
        /// 获取服务器配置
        /// </summary>
        TouchSocketConfig Config { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// 配置服务器
        /// </summary>
        /// <param name="serverConfig">配置</param>
        /// <exception cref="Exception"></exception>
        /// <returns>设置的服务实例</returns>
        IService Setup(TouchSocketConfig serverConfig);

        /// <summary>
        /// 配置服务器
        /// </summary>
        /// <param name="port"></param>
        /// <exception cref="Exception"></exception>
        /// <returns>设置的服务实例</returns>
        IService Setup(int port);

        /// <summary>
        /// 启动
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>设置的服务实例</returns>
        IService Start();

        /// <summary>
        /// 停止
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns>设置的服务实例</returns>
        IService Stop();
    }
}