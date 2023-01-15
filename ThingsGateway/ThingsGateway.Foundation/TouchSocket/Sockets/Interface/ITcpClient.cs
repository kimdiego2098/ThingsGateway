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

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// TCP客户端终端接口
    /// </summary>
    public interface ITcpClient : ITcpClientBase, IClientSender, IPluginObject
    {
        /// <summary>
        /// 成功连接到服务器
        /// </summary>
        MessageEventHandler<ITcpClient> Connected { get; set; }

        /// <summary>
        /// 准备连接的时候
        /// </summary>
        ConnectingEventHandler<ITcpClient> Connecting { get; set; }

        /// <summary>
        /// 远程IPHost。
        /// </summary>
        IPHost RemoteIPHost { get; }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        ITcpClient Connect(int timeout = 5000);

        /// <summary>
        /// 异步连接服务器
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        Task<ITcpClient> ConnectAsync(int timeout = 5000);

        /// <summary>
        /// 配置服务器
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="Exception"></exception>
        ITcpClient Setup(TouchSocketConfig config);

        /// <summary>
        /// 配置服务器
        /// </summary>
        /// <param name="ipHost"></param>
        /// <returns></returns>
        ITcpClient Setup(string ipHost);
    }
}