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
using System.Collections.Generic;


using System.Net;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 常规UDP数据处理适配器
    /// </summary>
    public class NormalUdpDataHandlingAdapter : UdpDataHandlingAdapter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanSplicingSend => true;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanSendRequestInfo => false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="byteBlock"></param>
        protected override void PreviewReceived(EndPoint remoteEndPoint, ByteBlock byteBlock)
        {
            GoReceived(remoteEndPoint, byteBlock, null);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        protected override void PreviewSend(EndPoint endPoint, byte[] buffer, int offset, int length)
        {
            GoSend(endPoint, buffer, offset, length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="transferBytes"></param>
        protected override void PreviewSend(EndPoint endPoint, IList<ArraySegment<byte>> transferBytes)
        {
            int length = 0;
            foreach (var item in transferBytes)
            {
                length += item.Count;
            }

            if (length > MaxPackageSize)
            {
                throw new OverlengthException("发送数据大于设定值，相同解析器可能无法收到有效数据，已终止发送");
            }

            using (ByteBlock byteBlock = new ByteBlock(length))
            {
                foreach (var item in transferBytes)
                {
                    byteBlock.Write(item.Array, item.Offset, item.Count);
                }
                GoSend(endPoint, byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="requestInfo"></param>
        protected override void PreviewSend(IRequestInfo requestInfo)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Reset()
        {
        }
    }
}