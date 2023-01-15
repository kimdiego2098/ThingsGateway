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



using ThingsGateway.Foundation.Rpc.TouchRpc;

namespace ThingsGateway.Foundation.Rpc
{
    /// <summary>
    /// 调用上下文
    /// </summary>
    public interface ICallContext
    {
        /// <summary>
        /// 调用此服务的主体。
        /// <para>
        /// <list type="bullet">
        /// <item>当该服务在<see cref="ITcpService"/>及派生中调用时，该值一般为<see cref="ISocketClient"/>对象。</item>
        /// <item>当该服务在<see cref="ITcpClient"/>及派生中调用时，该值一般为<see cref="ITcpClient"/>对象。</item>
        /// <item>当该服务在<see cref="IUdpSession"/>及派生中调用时，该值一般为<see cref="UdpCaller"/>对象。</item>
        /// </list>
        /// </para>
        /// </summary>
        object Caller { get; }

        /// <summary>
        /// 本次调用的<see cref="MethodInstance"/>
        /// </summary>
        MethodInstance MethodInstance { get; }

        /// <summary>
        /// 可取消的调用令箭
        /// </summary>
        CancellationTokenSource TokenSource { get; }
    }
}