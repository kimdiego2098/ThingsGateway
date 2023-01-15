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

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 通过ID发送
    /// </summary>
    public interface IIDSender
    {
        /// <summary>
        /// 向对应ID的客户端发送
        /// </summary>
        /// <param name="id">目标ID</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <exception cref="NotConnectedException">未连接异常</exception>
        /// <exception cref="ClientNotFindException">未找到ID对应的客户端</exception>
        /// <exception cref="Exception">其他异常</exception>
        void Send(string id, byte[] buffer, int offset, int length);

        /// <summary>
        /// 向对应ID的客户端发送
        /// </summary>
        /// <param name="id">目标ID</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <exception cref="NotConnectedException">未连接异常</exception>
        /// <exception cref="ClientNotFindException">未找到ID对应的客户端</exception>
        /// <exception cref="Exception">其他异常</exception>
        Task SendAsync(string id, byte[] buffer, int offset, int length);

        /// <summary>
        /// 向对应ID的客户端发送
        /// </summary>
        /// <param name="id">目标ID</param>
        /// <param name="requestInfo">数据对象</param>
        /// <exception cref="NotConnectedException">未连接异常</exception>
        /// <exception cref="ClientNotFindException">未找到ID对应的客户端</exception>
        /// <exception cref="Exception">其他异常</exception>
        void Send(string id, IRequestInfo requestInfo);

        /// <summary>
        /// 向对应ID的客户端发送
        /// </summary>
        /// <param name="id">目标ID</param>
        /// <param name="requestInfo">数据对象</param>
        /// <exception cref="NotConnectedException">未连接异常</exception>
        /// <exception cref="ClientNotFindException">未找到ID对应的客户端</exception>
        /// <exception cref="Exception">其他异常</exception>
        Task SendAsync(string id, IRequestInfo requestInfo);
    }
}