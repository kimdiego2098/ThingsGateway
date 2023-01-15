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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// UDP数据帧
    /// </summary>
    public struct UdpFrame
    {
        /// <summary>
        /// Crc校验
        /// </summary>
        public byte[] Crc { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 是否为终结帧
        /// </summary>
        public bool FIN { get; set; }

        /// <summary>
        /// 数据ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 帧序号
        /// </summary>
        public ushort SN { get; set; }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool Parse(byte[] buffer, int offset, int length)
        {
            if (length > 11)
            {
                ID = ThingsGatewayBitConverter.Default.ToInt64(buffer, offset);
                SN = ThingsGatewayBitConverter.Default.ToUInt16(buffer, 8 + offset);
                FIN = buffer[10 + offset].GetBit(7) == 1;
                if (FIN)
                {
                    if (length > 13)
                    {
                        Data = new byte[length - 13];
                    }
                    else
                    {
                        Data = new byte[0];
                    }
                    Crc = new byte[2] { buffer[length - 2], buffer[length - 1] };
                }
                else
                {
                    Data = new byte[length - 11];
                }

                Array.Copy(buffer, 11, Data, 0, Data.Length);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// UDP数据包
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public class UdpPackage
    {
        private readonly ConcurrentQueue<UdpFrame> m_frames;
        private readonly Timer m_timer;
        private int m_count;
        private int m_length;

        private int m_mtu;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timeout"></param>
        /// <param name="revStore"></param>
        public UdpPackage(long id, int timeout, ConcurrentDictionary<long, UdpPackage> revStore)
        {
            ID = id;
            m_frames = new ConcurrentQueue<UdpFrame>();
            m_timer = new Timer((o) =>
            {
                if (revStore.TryRemove(ID, out UdpPackage udpPackage))
                {
                    udpPackage.m_frames.Clear();
                }
            }, null, timeout, Timeout.Infinite);
        }

        /// <summary>
        /// 当前长度
        /// </summary>
        public int Count => m_count;

        /// <summary>
        /// Crc
        /// </summary>
        public byte[] Crc { get; private set; }

        /// <summary>
        /// 包唯一标识
        /// </summary>
        public long ID { get; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsComplated => TotalCount > 0 ? (TotalCount == m_count ? true : false) : false;

        /// <summary>
        /// 当前数据长度
        /// </summary>
        public int Length => m_length;

        /// <summary>
        /// MTU
        /// </summary>
        public int MTU => m_mtu + 11;

        /// <summary>
        /// 总长度，在收到最后一帧之前，为-1。
        /// </summary>
        public int TotalCount { get; private set; } = -1;

        /// <summary>
        /// 添加帧
        /// </summary>
        /// <param name="frame"></param>
        public void Add(UdpFrame frame)
        {
            Interlocked.Increment(ref m_count);

            if (frame.FIN)
            {
                TotalCount = frame.SN + 1;
                Crc = frame.Crc;
            }
            Interlocked.Add(ref m_length, frame.Data.Length);
            if (frame.SN == 0)
            {
                m_mtu = frame.Data.Length;
            }
            m_frames.Enqueue(frame);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="byteBlock"></param>
        /// <returns></returns>
        public bool TryGetData(ByteBlock byteBlock)
        {
            while (m_frames.TryDequeue(out UdpFrame frame))
            {
                byteBlock.Pos = frame.SN * m_mtu;
                byteBlock.Write(frame.Data);
            }

            if (byteBlock.Len != Length)
            {
                return false;
            }
            var crc = ThingsGateway.Foundation.Core.Crc.Crc16(byteBlock.Buffer, 0, byteBlock.Len);
            if (crc[0] != Crc[0] || crc[1] != Crc[1])
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// UDP数据包的适配器
    /// </summary>
    public class UdpPackageAdapter : UdpDataHandlingAdapter
    {
        private readonly SnowflakeIDGenerator m_iDGenerator;
        private readonly ConcurrentDictionary<long, UdpPackage> revStore;
        private int m_mtu = 1472;

        /// <summary>
        /// 构造函数
        /// </summary>
        public UdpPackageAdapter()
        {
            revStore = new ConcurrentDictionary<long, UdpPackage>();
            m_iDGenerator = new SnowflakeIDGenerator(4);
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
        /// 最大传输单元
        /// </summary>
        public int MTU
        {
            get => m_mtu + 11;
            set => m_mtu = value > 11 ? value : 1472;
        }

        /// <summary>
        /// 接收超时时间，默认5000ms
        /// </summary>
        public int Timeout { get; set; } = 5000;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="byteBlock"></param>
        protected override void PreviewReceived(EndPoint remoteEndPoint, ByteBlock byteBlock)
        {
            UdpFrame udpFrame = new UdpFrame();
            if (udpFrame.Parse(byteBlock.Buffer, 0, byteBlock.Len))
            {
                UdpPackage udpPackage = revStore.GetOrAdd(udpFrame.ID, (i) => new UdpPackage(i, Timeout, revStore));
                udpPackage.Add(udpFrame);
                if (udpPackage.Length > MaxPackageSize)
                {
                    revStore.TryRemove(udpPackage.ID, out _);
                    m_owner?.Logger.Error("数据长度大于设定的最大值。");
                    return;
                }
                if (udpPackage.IsComplated)
                {
                    if (revStore.TryRemove(udpPackage.ID, out _))
                    {
                        using (ByteBlock block = new ByteBlock(udpPackage.Length))
                        {
                            if (udpPackage.TryGetData(block))
                            {
                                GoReceived(remoteEndPoint, block, null);
                            }
                        }
                    }
                }
            }
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
            if (length > MaxPackageSize)
            {
                throw new OverlengthException("发送数据大于设定值，相同解析器可能无法收到有效数据，已终止发送");
            }
            long id = m_iDGenerator.NextID();
            int off = 0;
            int surLen = length;
            int freeRoom = m_mtu - 11;
            ushort sn = 0;
            /*|********|**|*|n|*/
            /*|********|**|*|**|*/
            while (surLen > 0)
            {
                byte[] data = new byte[m_mtu];
                Buffer.BlockCopy(ThingsGatewayBitConverter.Default.GetBytes(id), 0, data, 0, 8);
                Buffer.BlockCopy(ThingsGatewayBitConverter.Default.GetBytes(sn++), 0, data, 8, 2);
                if (surLen > freeRoom)//有余
                {
                    Buffer.BlockCopy(buffer, off, data, 11, freeRoom);
                    off += freeRoom;
                    surLen -= freeRoom;
                    GoSend(endPoint, data, 0, m_mtu);
                }
                else if (surLen + 2 <= freeRoom)//结束且能容纳Crc
                {
                    byte flag = 0;
                    data[10] = flag.SetBit(7, 1);//设置终结帧

                    Buffer.BlockCopy(buffer, off, data, 11, surLen);
                    Buffer.BlockCopy(Crc.Crc16(buffer, offset, length), 0, data, 11 + surLen, 2);

                    GoSend(endPoint, data, 0, surLen + 11 + 2);

                    off += surLen;
                    surLen -= surLen;
                }
                else//结束但不能容纳Crc
                {
                    Buffer.BlockCopy(buffer, off, data, 11, surLen);
                    GoSend(endPoint, data, 0, surLen + 11);
                    off += surLen;
                    surLen -= surLen;

                    byte[] finData = new byte[13];
                    Buffer.BlockCopy(ThingsGatewayBitConverter.Default.GetBytes(id), 0, finData, 0, 8);
                    Buffer.BlockCopy(ThingsGatewayBitConverter.Default.GetBytes(sn++), 0, finData, 8, 2);
                    byte flag = 0;
                    finData[10] = flag.SetBit(7, 1);
                    Buffer.BlockCopy(Crc.Crc16(buffer, offset, length), 0, finData, 11, 2);
                    GoSend(endPoint, finData, 0, finData.Length);
                }
            }
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
                PreviewSend(endPoint, byteBlock.Buffer, 0, byteBlock.Len);
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
        }
    }
}