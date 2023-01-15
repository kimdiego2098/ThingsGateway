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
using System.Net;
using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 具有直接发送功能
    /// </summary>
    public interface IUdpDefaultSender : ISenderBase
    {
        /// <summary>
        /// 绕过适配器，直接发送字节流
        /// </summary>
        /// <param name="endPoint">目的终结点</param>
        /// <param name="buffer">数据缓存区</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">数据长度</param>
        /// <exception cref="NotConnectedException">客户端没有连接</exception>
        /// <exception cref="OverlengthException">发送数据超长</exception>
        /// <exception cref="Exception">其他异常</exception>
        void DefaultSend(EndPoint endPoint, byte[] buffer, int offset, int length);

        /// <summary>
        /// 绕过适配器，直接发送字节流
        /// </summary>
        /// <param name="endPoint">目的终结点</param>
        /// <param name="buffer">数据缓存区</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">数据长度</param>
        /// <exception cref="NotConnectedException">客户端没有连接</exception>
        /// <exception cref="OverlengthException">发送数据超长</exception>
        /// <exception cref="Exception">其他异常</exception>
        Task DefaultSendAsync(EndPoint endPoint, byte[] buffer, int offset, int length);
    }
}