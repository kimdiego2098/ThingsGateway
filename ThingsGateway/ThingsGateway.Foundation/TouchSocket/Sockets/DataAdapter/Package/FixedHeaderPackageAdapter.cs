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
    /// 固定包头数据包处理适配器，支持Byte、UShort、Int三种类型作为包头。使用<see cref="ThingsGatewayBitConverter.DefaultEndianType"/>大小端设置。
    /// </summary>
    public class FixedHeaderPackageAdapter : DataHandlingAdapter
    {
        private byte[] m_agreementTempBytes;
        private int m_surPlusLength = 0;

        //包剩余长度
        private ByteBlock m_tempByteBlock;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanSendRequestInfo => false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanSplicingSend => true;

        /// <summary>
        /// 设置包头类型，默认为int
        /// </summary>
        public FixedHeaderType FixedHeaderType { get; set; } = FixedHeaderType.Int;

        /// <summary>
        /// 获取或设置包数据的最小值（默认为0）
        /// </summary>
        public int MinPackageSize { get; set; } = 0;

        /// <summary>
        /// 当接收到数据时处理数据
        /// </summary>
        /// <param name="byteBlock">数据流</param>
        protected override void PreviewReceived(ByteBlock byteBlock)
        {
            byte[] buffer = byteBlock.Buffer;
            int r = byteBlock.Len;

            if (CacheTimeoutEnable && DateTime.Now - LastCacheTime > CacheTimeout)
            {
                Reset();
            }

            if (m_agreementTempBytes != null)
            {
                SeamPackage(buffer, r);
            }
            else if (m_tempByteBlock == null)
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
        /// 当发送数据前处理数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        protected override void PreviewSend(byte[] buffer, int offset, int length)
        {
            if (length < MinPackageSize)
            {
                throw new Exception("发送数据小于设定值，相同解析器可能无法收到有效数据，已终止发送");
            }

            if (length > MaxPackageSize)
            {
                throw new Exception("发送数据大于设定值，相同解析器可能无法收到有效数据，已终止发送");
            }

            ByteBlock byteBlock = null;
            byte[] lenBytes = null;

            switch (FixedHeaderType)
            {
                case FixedHeaderType.Byte:
                    {
                        byte dataLen = (byte)(length - offset);
                        byteBlock = BytePool.GetByteBlock(dataLen + 1);
                        lenBytes = new byte[] { dataLen };
                        break;
                    }
                case FixedHeaderType.Ushort:
                    {
                        ushort dataLen = (ushort)(length - offset);
                        byteBlock = BytePool.GetByteBlock(dataLen + 2);
                        lenBytes = ThingsGatewayBitConverter.Default.GetBytes(dataLen);
                        break;
                    }
                case FixedHeaderType.Int:
                    {
                        int dataLen = length - offset;
                        byteBlock = BytePool.GetByteBlock(dataLen + 4);
                        lenBytes = ThingsGatewayBitConverter.Default.GetBytes(dataLen);
                        break;
                    }
            }

            try
            {
                byteBlock.Write(lenBytes);
                byteBlock.Write(buffer, offset, length);
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
            if (transferBytes.Count == 0)
            {
                return;
            }

            int length = 0;
            foreach (var item in transferBytes)
            {
                length += item.Count;
            }

            if (length < MinPackageSize)
            {
                throw new Exception("发送数据小于设定值，相同解析器可能无法收到有效数据，已终止发送");
            }

            if (length > MaxPackageSize)
            {
                throw new Exception("发送数据大于设定值，相同解析器可能无法收到有效数据，已终止发送");
            }

            ByteBlock byteBlock = null;
            byte[] lenBytes = null;

            switch (FixedHeaderType)
            {
                case FixedHeaderType.Byte:
                    {
                        byte dataLen = (byte)length;
                        byteBlock = BytePool.GetByteBlock(dataLen + 1);
                        lenBytes = new byte[] { dataLen };
                        break;
                    }
                case FixedHeaderType.Ushort:
                    {
                        ushort dataLen = (ushort)length;
                        byteBlock = BytePool.GetByteBlock(dataLen + 2);
                        lenBytes = ThingsGatewayBitConverter.Default.GetBytes(dataLen);
                        break;
                    }
                case FixedHeaderType.Int:
                    {
                        byteBlock = BytePool.GetByteBlock(length + 4);
                        lenBytes = ThingsGatewayBitConverter.Default.GetBytes(length);
                        break;
                    }
            }

            try
            {
                byteBlock.Write(lenBytes);
                foreach (var item in transferBytes)
                {
                    byteBlock.Write(item.Array, item.Offset, item.Count);
                }
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
            m_agreementTempBytes = null;
            m_surPlusLength = default;
            m_tempByteBlock?.Dispose();
            m_tempByteBlock = null;
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

        /// <summary>
        /// 缝合包
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="r"></param>
        private void SeamPackage(byte[] buffer, int r)
        {
            ByteBlock byteBlock = BytePool.GetByteBlock(r + m_agreementTempBytes.Length);
            byteBlock.Write(m_agreementTempBytes);
            byteBlock.Write(buffer, 0, r);
            r += m_agreementTempBytes.Length;
            m_agreementTempBytes = null;
            SplitPackage(byteBlock.Buffer, 0, r);
            byteBlock.Dispose();
        }

        /// <summary>
        /// 分解包
        /// </summary>
        /// <param name="dataBuffer"></param>
        /// <param name="index"></param>
        /// <param name="r"></param>
        private void SplitPackage(byte[] dataBuffer, int index, int r)
        {
            while (index < r)
            {
                if (r - index <= (byte)FixedHeaderType)
                {
                    m_agreementTempBytes = new byte[r - index];
                    Array.Copy(dataBuffer, index, m_agreementTempBytes, 0, m_agreementTempBytes.Length);
                    if (UpdateCacheTimeWhenRev)
                    {
                        LastCacheTime = DateTime.Now;
                    }
                    return;
                }
                int length = 0;

                switch (FixedHeaderType)
                {
                    case FixedHeaderType.Byte:
                        length = dataBuffer[index];
                        break;

                    case FixedHeaderType.Ushort:
                        length = ThingsGatewayBitConverter.Default.ToUInt16(dataBuffer, index);
                        break;

                    case FixedHeaderType.Int:
                        length = ThingsGatewayBitConverter.Default.ToInt32(dataBuffer, index);
                        break;
                }

                if (length < 0)
                {
                    throw new Exception("接收数据长度错误，已放弃接收");
                }
                else if (length < MinPackageSize)
                {
                    throw new Exception("接收数据长度小于设定值，已放弃接收");
                }
                else if (length > MaxPackageSize)
                {
                    throw new Exception("接收数据长度大于设定值，已放弃接收");
                }

                int recedSurPlusLength = r - index - (byte)FixedHeaderType;
                if (recedSurPlusLength >= length)
                {
                    ByteBlock byteBlock = BytePool.GetByteBlock(length);
                    byteBlock.Write(dataBuffer, index + (byte)FixedHeaderType, length);
                    PreviewHandle(byteBlock);
                    m_surPlusLength = 0;
                }
                else//半包
                {
                    m_tempByteBlock = BytePool.GetByteBlock(length);
                    m_surPlusLength = length - recedSurPlusLength;
                    m_tempByteBlock.Write(dataBuffer, index + (byte)FixedHeaderType, recedSurPlusLength);
                    if (UpdateCacheTimeWhenRev)
                    {
                        LastCacheTime = DateTime.Now;
                    }
                }
                index += length + (byte)FixedHeaderType;
            }
        }
    }
}