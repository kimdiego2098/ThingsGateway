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

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 固定长度数据包处理适配器。
    /// </summary>
    public class FixedSizePackageAdapter : DataHandlingAdapter
    {
        /// <summary>
        /// 包剩余长度
        /// </summary>
        private int m_surPlusLength = 0;

        /// <summary>
        /// 临时包
        /// </summary>
        private ByteBlock m_tempByteBlock;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fixedSize">数据包的长度</param>
        public FixedSizePackageAdapter(int fixedSize)
        {
            FixedSize = fixedSize;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanSendRequestInfo => false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanSplicingSend => true;

        /// <summary>
        /// 获取已设置的数据包的长度
        /// </summary>
        public int FixedSize { get; private set; }

        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="byteBlock"></param>
        protected override void PreviewReceived(ByteBlock byteBlock)
        {
            if (CacheTimeoutEnable && DateTime.Now - LastCacheTime > CacheTimeout)
            {
                Reset();
            }
            byte[] buffer = byteBlock.Buffer;
            int r = byteBlock.Len;
            if (m_tempByteBlock == null)
            {
                SplitPackage(buffer, 0, r);
            }
            else
            {
                if (m_surPlusLength == r)
                {
                    m_tempByteBlock.Write(buffer, 0, m_surPlusLength);
                    PreviewHandle(m_tempByteBlock);
                    m_tempByteBlock = null;
                    m_surPlusLength = 0;
                }
                else if (m_surPlusLength < r)
                {
                    m_tempByteBlock.Write(buffer, 0, m_surPlusLength);
                    PreviewHandle(m_tempByteBlock);
                    m_tempByteBlock = null;
                    SplitPackage(buffer, m_surPlusLength, r);
                }
                else
                {
                    m_tempByteBlock.Write(buffer, 0, r);
                    m_surPlusLength -= r;
                    if (UpdateCacheTimeWhenRev)
                    {
                        LastCacheTime = DateTime.Now;
                    }
                }
            }
        }

        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        protected override void PreviewSend(byte[] buffer, int offset, int length)
        {
            int dataLen = length - offset;
            if (dataLen > FixedSize)
            {
                throw new OverlengthException("发送的数据包长度大于FixedSize");
            }
            ByteBlock byteBlock = BytePool.GetByteBlock(FixedSize);

            byteBlock.Write(buffer, offset, length);
            for (int i = (int)byteBlock.Position; i < FixedSize; i++)
            {
                byteBlock.Buffer[i] = 0;
            }
            byteBlock.SetLength(FixedSize);
            try
            {
                GoSend(byteBlock.Buffer, 0, byteBlock.Len);
            }
            finally
            {
                byteBlock.Dispose();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="transferBytes"></param>
        protected override void PreviewSend(IList<ArraySegment<byte>> transferBytes)
        {
            int length = 0;
            foreach (var item in transferBytes)
            {
                length += item.Count;
            }

            if (length > FixedSize)
            {
                throw new OverlengthException("发送的数据包长度大于FixedSize");
            }
            ByteBlock byteBlock = BytePool.GetByteBlock(FixedSize);

            foreach (var item in transferBytes)
            {
                byteBlock.Write(item.Array, item.Offset, item.Count);
            }

            Array.Clear(byteBlock.Buffer, byteBlock.Pos, FixedSize);
            byteBlock.SetLength(FixedSize);
            try
            {
                GoSend(byteBlock.Buffer, 0, byteBlock.Len);
            }
            finally
            {
                byteBlock.Dispose();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="requestInfo"></param>
        protected override void PreviewSend(IRequestInfo requestInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Reset()
        {
            m_tempByteBlock.SafeDispose();
            m_tempByteBlock = null;
            m_surPlusLength = 0;
        }

        private void PreviewHandle(ByteBlock byteBlock)
        {
            try
            {
                GoReceived(byteBlock, null);
            }
            finally
            {
                byteBlock.Dispose();
            }
        }

        private void SplitPackage(byte[] dataBuffer, int index, int r)
        {
            while (index < r)
            {
                if (r - index >= FixedSize)
                {
                    ByteBlock byteBlock = BytePool.GetByteBlock(FixedSize);
                    byteBlock.Write(dataBuffer, index, FixedSize);
                    PreviewHandle(byteBlock);
                    m_surPlusLength = 0;
                }
                else//半包
                {
                    m_tempByteBlock = BytePool.GetByteBlock(FixedSize);
                    m_surPlusLength = FixedSize - (r - index);
                    m_tempByteBlock.Write(dataBuffer, index, r - index);
                    if (UpdateCacheTimeWhenRev)
                    {
                        LastCacheTime = DateTime.Now;
                    }
                }
                index += FixedSize;
            }
        }
    }
}