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
    /// RpcActorExtensions
    /// </summary>
    public static partial class RpcActorExtensions
    {
        #region 尝试发送

        /// <summary>
        /// 尝试发送
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool TrySend<TRpcActor>(this TRpcActor rpcActor, short protocol, byte[] buffer, int offset, int length) where TRpcActor : IRpcActor
        {
            try
            {
                rpcActor.Send(protocol, buffer, offset, length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试发送
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static bool TrySend<TRpcActor>(this TRpcActor rpcActor, short protocol, byte[] buffer) where TRpcActor : IRpcActor
        {
            try
            {
                rpcActor.Send(protocol, buffer, 0, buffer.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试发送
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public static bool TrySend<TRpcActor>(this TRpcActor rpcActor, short protocol) where TRpcActor : IRpcActor
        {
            try
            {
                rpcActor.Send(protocol);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试发送
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <param name="byteBlock"></param>
        /// <returns></returns>
        public static bool TrySend<TRpcActor>(this TRpcActor rpcActor, short protocol, ByteBlock byteBlock) where TRpcActor : IRpcActor
        {
            try
            {
                rpcActor.Send(protocol, byteBlock);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion 尝试发送

        #region 尝试异步发送

        /// <summary>
        /// 尝试发送
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static async Task<bool> TrySendAsync<TRpcActor>(this TRpcActor rpcActor, short protocol, byte[] buffer, int offset, int length) where TRpcActor : IRpcActor
        {
            try
            {
                await rpcActor.SendAsync(protocol, buffer, offset, length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试发送
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static async Task<bool> TrySendAsync<TRpcActor>(this TRpcActor rpcActor, short protocol, byte[] buffer) where TRpcActor : IRpcActor
        {
            try
            {
                await rpcActor.SendAsync(protocol, buffer, 0, buffer.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试发送
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public static async Task<bool> TrySendAsync<TRpcActor>(this TRpcActor rpcActor, short protocol) where TRpcActor : IRpcActor
        {
            try
            {
                await rpcActor.SendAsync(protocol);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion 尝试异步发送

        #region 发送

        /// <summary>
        /// <inheritdoc cref="IRpcActorBase.Send(short, byte[], int, int)"/>
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        public static void Send<TRpcActor>(this TRpcActor rpcActor, short protocol, byte[] buffer) where TRpcActor : IRpcActor
        {
            rpcActor.Send(protocol, buffer, 0, buffer.Length);
        }

        /// <summary>
        ///  <inheritdoc cref="IRpcActorBase.Send(short, byte[], int, int)"/>
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        public static void Send<TRpcActor>(this TRpcActor rpcActor, short protocol) where TRpcActor : IRpcActor
        {
            rpcActor.Send(protocol, TouchSocketCoreUtility.ZeroBytes, 0, 0);
        }

        /// <summary>
        ///  <inheritdoc cref="IRpcActorBase.Send(short, byte[], int, int)"/>
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <param name="byteBlock"></param>
        public static void Send<TRpcActor>(this TRpcActor rpcActor, short protocol, ByteBlock byteBlock) where TRpcActor : IRpcActor
        {
            rpcActor.Send(protocol, byteBlock.Buffer, 0, byteBlock.Len);
        }

        /// <summary>
        ///  <inheritdoc cref="IRpcActorBase.SendAsync(short, byte[], int, int)"/>
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Task SendAsync<TRpcActor>(this TRpcActor rpcActor, short protocol, byte[] buffer) where TRpcActor : IRpcActor
        {
            return rpcActor.SendAsync(protocol, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// <inheritdoc cref="IRpcActorBase.SendAsync(short, byte[], int, int)"/>
        /// </summary>
        /// <typeparam name="TRpcActor"></typeparam>
        /// <param name="rpcActor"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public static Task SendAsync<TRpcActor>(this TRpcActor rpcActor, short protocol) where TRpcActor : IRpcActor
        {
            return rpcActor.SendAsync(protocol, TouchSocketCoreUtility.ZeroBytes, 0, 0);
        }

        #endregion 发送
    }
}