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

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// RpcActorBase
    /// </summary>
    public interface IRpcActorBase
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        ILog Logger { get; }

        /// <summary>
        /// 序列化选择器
        /// </summary>
        SerializationSelector SerializationSelector { get; }

        /// <summary>
        /// 向通信的对方执行ping。
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns>如果返回True，则表示一定在线。如果返回false，则不一定代表不在线。</returns>
        bool Ping(int timeout = 5000);

        /// <summary>
        /// 发送字节
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        void Send(short protocol, byte[] buffer, int offset, int length);

        /// <summary>
        /// 发送字节
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        Task SendAsync(short protocol, byte[] buffer, int offset, int length);
    }
}