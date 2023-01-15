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
    /// TouchRpcServiceExtension
    /// </summary>
    public static class TouchRpcServiceExtension
    {
        /// <inheritdoc cref="ITouchRpcService.Send(string, short, byte[], int, int)"/>
        public static void Send(this ITouchRpcService service, string targetId, short protocol, byte[] buffer)
        {
            service.Send(targetId, protocol, buffer, 0, buffer.Length);
        }

        /// <inheritdoc cref="ITouchRpcService.Send(string, short, byte[], int, int)"/>
        public static void Send(this ITouchRpcService service, string targetId, short protocol)
        {
            service.Send(targetId, protocol, TouchSocketCoreUtility.ZeroBytes, 0, 0);
        }

        /// <inheritdoc cref="ITouchRpcService.Send(string, short, byte[], int, int)"/>
        public static void Send(this ITouchRpcService service, string targetId, short protocol, ByteBlock byteBlock)
        {
            service.Send(targetId, protocol, byteBlock.Buffer, 0, byteBlock.Len);
        }

        /// <inheritdoc cref="ITouchRpcService.SendAsync(string, short, byte[], int, int)"/>
        public static Task SendAsync(this ITouchRpcService service, string targetId, short protocol, byte[] buffer)
        {
            return service.SendAsync(targetId, protocol, buffer, 0, buffer.Length);
        }

        /// <inheritdoc cref="ITouchRpcService.SendAsync(string, short, byte[], int, int)"/>
        public static Task SendAsync(this ITouchRpcService service, string targetId, short protocol)
        {
            return service.SendAsync(targetId, protocol, TouchSocketCoreUtility.ZeroBytes, 0, 0);
        }
    }
}