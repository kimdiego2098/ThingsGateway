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
    /// 接收类型
    /// </summary>
    public enum ReceiveType : byte
    {
        /// <summary>
        /// 该模式下会自动接收数据，然后主动触发。
        /// </summary>
        Auto,

        /// <summary>
        /// 在该模式下，不会投递接收申请，用户可通过<see cref="ITcpClientBase.GetStream"/>，获取到流以后，自己处理接收。
        /// <para>注意：连接端不会感知主动断开</para>
        /// </summary>
        None
    }
}