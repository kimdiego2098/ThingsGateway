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

using System.Diagnostics;
using System.IO;
using System.Text;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 字节块流
    /// </summary>
    [DebuggerDisplay("Len={Len},Pos={Pos},Capacity={Capacity}")]
    public sealed class ByteBlock : Stream, IByteBlock
    {
        private static float m_ratio = 1.5f;
        private readonly bool m_needDis;
        private byte[] m_buffer;
        private int m_dis = 1;
        private bool m_holding;
        private long m_length;
        private long m_position;
        private bool m_using;

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="byteSize"></param>
        /// <param name="equalSize"></param>
        public ByteBlock(int byteSize = 1024 * 64, bool equalSize = false)
        {
            m_needDis = true;
            m_buffer = BytePool.GetByteCore(byteSize, equalSize);
            m_using = true;
        }

        /// <summary>
        /// 实例化一个已知内存的对象。且该内存不会被回收。
        /// </summary>
        /// <param name="bytes"></param>
        public ByteBlock(byte[] bytes)
        {
            m_buffer = bytes ?? throw new ArgumentNullException(nameof(bytes));
            m_length = bytes.Length;
            m_using = true;
        }

        /// <summary>
        /// 扩容增长比，默认为1.5，
        /// min：1.5
        /// </summary>
        public static float Ratio
        {
            get => m_ratio;
            set
            {
                if (value < 1.5)
                {
                    value = 1.5f;
                }
                m_ratio = value;
            }
        }

        /// <summary>
        /// 字节实例
        /// </summary>
        public byte[] Buffer => m_buffer;

        /// <summary>
        /// 仅当内存块可用，且<see cref="CanReadLen"/>>0时为True。
        /// </summary>
        public override bool CanRead => m_using && CanReadLen > 0;

        /// <summary>
        /// 还能读取的长度，计算为<see cref="Len"/>与<see cref="Pos"/>的差值。
        /// </summary>
        public int CanReadLen => Len - Pos;

        /// <summary>
        /// 还能读取的长度，计算为<see cref="Len"/>与<see cref="Pos"/>的差值。
        /// </summary>
        public long CanReadLength => m_length - m_position;

        /// <summary>
        /// 支持查找
        /// </summary>
        public override bool CanSeek => m_using;

        /// <summary>
        /// 可写入
        /// </summary>
        public override bool CanWrite => m_using;

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity => m_buffer.Length;

        /// <summary>
        /// 空闲长度，准确掌握该值，可以避免内存扩展，计算为<see cref="Capacity"/>与<see cref="Pos"/>的差值。
        /// </summary>
        public int FreeLength => Capacity - Pos;

        /// <summary>
        /// 表示持续性持有，为True时，Dispose将调用无效。
        /// </summary>
        public bool Holding => m_holding;

        /// <summary>
        /// Int真实长度
        /// </summary>
        public int Len => (int)m_length;

        /// <summary>
        /// 真实长度
        /// </summary>
        public override long Length => m_length;

        /// <summary>
        /// int型流位置
        /// </summary>
        public int Pos
        {
            get => (int)m_position;
            set => m_position = value;
        }

        /// <summary>
        /// 流位置
        /// </summary>
        public override long Position
        {
            get => m_position;
            set => m_position = value;
        }

        /// <summary>
        /// 使用状态
        /// </summary>
        public bool Using => m_using;

        /// <summary>
        /// 直接完全释放，游离该对象，然后等待GC
        /// </summary>
        public void AbsoluteDispose()
        {
            if (Interlocked.Decrement(ref m_dis) == 0)
            {
                Dis();
            }
        }

        /// <summary>
        /// 清空所有内存数据
        /// </summary>
        /// <exception cref="ObjectDisposedException">内存块已释放</exception>
        public void Clear()
        {
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            Array.Clear(m_buffer, 0, m_buffer.Length);
        }

        /// <summary>
        /// 无实际效果
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// 读取数据，然后递增Pos
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public override int Read(byte[] buffer, int offset, int length)
        {
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            int len = m_length - m_position > length ? length : CanReadLen;
            Array.Copy(m_buffer, m_position, buffer, offset, len);
            m_position += len;
            return len;
        }

        /// <summary>
        /// 读取数据，然后递增Pos
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int Read(byte[] buffer)
        {
            return Read(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 读取数据，然后递增Pos
        /// </summary>

        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public int Read(out byte[] buffer, int length)
        {
            buffer = new byte[length];
            return Read(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 从当前流位置读取一个<see cref="byte"/>值
        /// </summary>
        public override int ReadByte()
        {
            byte value = m_buffer[m_position];
            m_position++;
            return value;
        }

        /// <summary>
        /// 将内存块初始化到刚申请的状态。
        /// <para>仅仅重置<see cref="Position"/>和<see cref="Length"/>属性。</para>
        /// </summary>
        /// <exception cref="ObjectDisposedException">内存块已释放</exception>
        public void Reset()
        {
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            m_position = 0;
            m_length = 0;
        }

        /// <summary>
        /// 设置流位置
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            switch (origin)
            {
                case SeekOrigin.Begin:
                    m_position = offset;
                    break;

                case SeekOrigin.Current:
                    m_position += offset;
                    break;

                case SeekOrigin.End:
                    m_position = m_length + offset;
                    break;
            }
            return m_position;
        }

        /// <summary>
        /// 移动游标
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ByteBlock Seek(int position)
        {
            Position = position;
            return this;
        }

        /// <summary>
        /// 设置游标到末位
        /// </summary>
        /// <returns></returns>
        public ByteBlock SeekToEnd()
        {
            Position = Length;
            return this;
        }

        /// <summary>
        /// 设置游标到首位
        /// </summary>
        /// <returns></returns>
        public ByteBlock SeekToStart()
        {
            Position = 0;
            return this;
        }

        /// <summary>
        /// 重新设置容量
        /// </summary>
        /// <param name="size">新尺寸</param>
        /// <param name="retainedData">是否保留元数据</param>
        /// <exception cref="ObjectDisposedException"></exception>
        public void SetCapacity(int size, bool retainedData = false)
        {
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            byte[] bytes = new byte[size];

            if (retainedData)
            {
                Array.Copy(m_buffer, 0, bytes, 0, m_buffer.Length);
            }
            BytePool.Recycle(m_buffer);
            m_buffer = bytes;
        }

        /// <summary>
        /// 设置持续持有属性，当为True时，调用Dispose会失效，表示该对象将长期持有，直至设置为False。
        /// 当为False时，会自动调用Dispose。
        /// </summary>
        /// <param name="holding"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public void SetHolding(bool holding)
        {
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            m_holding = holding;
            if (!holding)
            {
                Dispose();
            }
        }

        /// <summary>
        /// 设置实际长度
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public override void SetLength(long value)
        {
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (value > m_buffer.Length)
            {
                throw new Exception("设置值超出容量");
            }
            m_length = value;
        }

        /// <summary>
        /// 从指定位置转化到指定长度的有效内存
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] ToArray(int offset, int length)
        {
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            byte[] buffer = new byte[length];
            Array.Copy(m_buffer, offset, buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 转换为有效内存
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return ToArray(0, Len);
        }

        /// <summary>
        /// 从指定位置转化到有效内存
        /// </summary>

        /// <param name="offset"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public byte[] ToArray(int offset)
        {
            return ToArray(offset, Len - offset);
        }

        /// <summary>
        /// 转换为UTF-8字符
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(0, Len);
        }

        /// <summary>
        /// 转换为UTF-8字符
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public string ToString(int offset, int length)
        {
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            return Encoding.UTF8.GetString(m_buffer, offset, length);
        }

        /// <summary>
        /// 转换为UTF-8字符
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public string ToString(int offset)
        {
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            return Encoding.UTF8.GetString(m_buffer, offset, Len - offset);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count == 0)
            {
                return;
            }
            if (!m_using)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (m_buffer.Length - m_position < count)
            {
                int need = m_buffer.Length + count - ((int)(m_buffer.Length - m_position));
                int lend = m_buffer.Length;
                while (need > lend)
                {
                    lend = (int)(lend * m_ratio);
                }
                SetCapacity(lend, true);
            }
            //int len = count + offset;
            //for (int i = offset; i < len; i++)
            //{
            //    this.m_buffer[this.m_position] = buffer[i];
            //    this.m_position++;
            //}

            Array.Copy(buffer, offset, m_buffer, m_position, count);
            m_position += count;
            m_length = Math.Max(m_position, m_length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override sealed void Dispose(bool disposing)
        {
            if (m_holding)
            {
                return;
            }

            if (m_needDis)
            {
                if (Interlocked.Decrement(ref m_dis) == 0)
                {
                    GC.SuppressFinalize(this);
                    BytePool.Recycle(m_buffer);
                    Dis();
                }
            }

            base.Dispose(disposing);
        }

        private void Dis()
        {
            m_holding = false;
            m_using = false;
            m_position = 0;
            m_length = 0;
            m_buffer = null;
        }

        #region BytesPackage

        /// <summary>
        /// 从当前流位置读取一个独立的<see cref="byte"/>数组包
        /// </summary>
        public byte[] ReadBytesPackage()
        {
            byte status = (byte)ReadByte();
            if (status == 0)
            {
                return null;
            }
            int length = ReadInt32();
            byte[] data = new byte[length];
            Array.Copy(Buffer, Pos, data, 0, length);
            Pos += length;
            return data;
        }

        /// <summary>
        /// 尝试获取数据包信息，方便从Buffer操作数据
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public bool TryReadBytesPackageInfo(out int pos, out int len)
        {
            byte status = (byte)ReadByte();
            if (status == 0)
            {
                pos = 0;
                len = 0;
                return false;
            }
            len = ReadInt32();
            pos = Pos;
            return true;
        }

        /// <summary>
        /// 写入一个独立的<see cref="byte"/>数组包，值可以为null。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public ByteBlock WriteBytesPackage(byte[] value, int offset, int length)
        {
            if (value == null)
            {
                Write((byte)0);
            }
            else
            {
                Write((byte)1);
                Write(length);
                Write(value, offset, length);
            }
            return this;
        }

        /// <summary>
        /// 写入一个独立的<see cref="byte"/>数组包。值可以为null。
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock WriteBytesPackage(byte[] value)
        {
            if (value == null)
            {
                return WriteBytesPackage(value, 0, 0);
            }
            return WriteBytesPackage(value, 0, value.Length);
        }

        #endregion BytesPackage

        #region Int32

        /// <summary>
        /// 从当前流位置读取一个<see cref="int"/>值
        /// </summary>
        public int ReadInt32()
        {
            int value = ThingsGatewayBitConverter.Default.ToInt32(Buffer, Pos);
            Pos += 4;
            return value;
        }

        /// <summary>
        /// 写入<see cref="int"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(int value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value));
            return this;
        }

        #endregion Int32

        #region Int16

        /// <summary>
        /// 从当前流位置读取一个<see cref="short"/>值
        /// </summary>
        public short ReadInt16()
        {
            short value = ThingsGatewayBitConverter.Default.ToInt16(Buffer, Pos);
            Pos += 2;
            return value;
        }

        /// <summary>
        /// 写入<see cref="short"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(short value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value));
            return this;
        }

        #endregion Int16

        #region Int64

        /// <summary>
        /// 从当前流位置读取一个<see cref="long"/>值
        /// </summary>
        public long ReadInt64()
        {
            long value = ThingsGatewayBitConverter.Default.ToInt64(Buffer, Pos);
            Pos += 8;
            return value;
        }

        /// <summary>
        /// 写入<see cref="long"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(long value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value));
            return this;
        }

        #endregion Int64

        #region Boolean

        /// <summary>
        /// 从当前流位置读取一个<see cref="bool"/>值
        /// </summary>
        public bool ReadBoolean()
        {
            bool value = ThingsGatewayBitConverter.Default.ToBoolean(Buffer, Pos);
            Pos += 1;
            return value;
        }

        /// <summary>
        /// 写入<see cref="bool"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(bool value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value));
            return this;
        }

        #endregion Boolean

        #region Byte

        /// <summary>
        /// 写入<see cref="byte"/>值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ByteBlock Write(byte value)
        {
            Write(new byte[] { value }, 0, 1);
            return this;
        }

        #endregion Byte

        #region String

        /// <summary>
        /// 从当前流位置读取一个<see cref="string"/>值
        /// </summary>
        public string ReadString()
        {
            int len = this.ReadInt32();
            if (len < 0)
            {
                return null;
            }
            else
            {
                string str = Encoding.UTF8.GetString(Buffer, Pos, len);
                Pos += len;
                return str;
            }
        }

        /// <summary>
        /// 写入<see cref="string"/>值。值可以为null，或者空。
        /// <para>注意：该操作不具备通用性，读取时必须使用ReadString。或者得先做出判断，由默认端序的int32值标识，具体如下：</para>
        /// <list type="bullet">
        /// <item>小于0，表示字符串为null</item>
        /// <item>等于0，表示字符串为""</item>
        /// <item>大于0，表示字符串在utf-8编码下的字节长度。</item>
        /// </list>
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(string value)
        {
            if (value == null)
            {
                Write(-1);
            }
            else
            {
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                Write(buffer.Length);
                Write(buffer);
            }
            return this;
        }

        /// <summary>
        /// 写入<see cref="string"/>值。值必须为有效值。可通用解析。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        public ByteBlock WriteString(string value, Encoding encoding = null)
        {
            Write((encoding ?? Encoding.UTF8).GetBytes(value));
            return this;
        }

        #endregion String

        #region Char

        /// <summary>
        /// 从当前流位置读取一个<see cref="char"/>值
        /// </summary>
        public char ReadChar()
        {
            char value = ThingsGatewayBitConverter.Default.ToChar(Buffer, Pos);
            Pos += 2;
            return value;
        }

        /// <summary>
        /// 写入<see cref="char"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(char value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value));
            return this;
        }

        #endregion Char

        #region Double

        /// <summary>
        /// 从当前流位置读取一个<see cref="double"/>值
        /// </summary>
        public double ReadDouble()
        {
            double value = ThingsGatewayBitConverter.Default.ToDouble(Buffer, Pos);
            Pos += 8;
            return value;
        }

        /// <summary>
        /// 写入<see cref="double"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(double value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value));
            return this;
        }

        #endregion Double

        #region Float

        /// <summary>
        /// 从当前流位置读取一个<see cref="float"/>值
        /// </summary>
        public float ReadFloat()
        {
            float value = ThingsGatewayBitConverter.Default.ToSingle(Buffer, Pos);
            Pos += 4;
            return value;
        }

        /// <summary>
        /// 写入<see cref="float"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(float value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value));
            return this;
        }

        #endregion Float

        #region UInt16

        /// <summary>
        /// 从当前流位置读取一个<see cref="ushort"/>值
        /// </summary>
        public ushort ReadUInt16()
        {
            ushort value = ThingsGatewayBitConverter.Default.ToUInt16(Buffer, Pos);
            Pos += 2;
            return value;
        }

        /// <summary>
        /// 写入<see cref="ushort"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(ushort value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value));
            return this;
        }

        #endregion UInt16

        #region UInt32

        /// <summary>
        /// 从当前流位置读取一个<see cref="uint"/>值
        /// </summary>
        public uint ReadUInt32()
        {
            uint value = ThingsGatewayBitConverter.Default.ToUInt32(Buffer, Pos);
            Pos += 4;
            return value;
        }

        /// <summary>
        /// 写入<see cref="uint"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(uint value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value));
            return this;
        }

        #endregion UInt32

        #region UInt64

        /// <summary>
        /// 从当前流位置读取一个<see cref="ulong"/>值
        /// </summary>
        public ulong ReadUInt64()
        {
            ulong value = ThingsGatewayBitConverter.Default.ToUInt64(Buffer, Pos);
            Pos += 8;
            return value;
        }

        /// <summary>
        /// 写入<see cref="ulong"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(ulong value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value));
            return this;
        }

        #endregion UInt64

        #region Null

        /// <summary>
        /// 从当前流位置读取一个标识值，判断是否为null。
        /// </summary>
        public bool ReadIsNull()
        {
            var status = ReadByte();
            if (status == 0)
            {
                return true;
            }
            else if (status == 1)
            {
                return false;
            }
            else
            {
                throw new Exception("标识既非Null，也非NotNull，可能是流位置发生了错误。");
            }
        }

        /// <summary>
        /// 判断该值是否为Null，然后写入标识值
        /// </summary>
        public ByteBlock WriteIsNull<T>(T t) where T : class
        {
            if (t == null)
            {
                WriteNull();
            }
            else
            {
                WriteNotNull();
            }
            return this;
        }

        /// <summary>
        /// 写入一个标识非Null值
        /// </summary>
        public ByteBlock WriteNotNull()
        {
            Write((byte)1);
            return this;
        }

        /// <summary>
        /// 写入一个标识Null值
        /// </summary>
        public ByteBlock WriteNull()
        {
            Write((byte)0);
            return this;
        }

        #endregion Null

        #region Package

        /// <summary>
        /// 读取一个指定类型的包
        /// </summary>
        /// <typeparam name="TPackage"></typeparam>
        /// <returns></returns>
        public TPackage ReadPackage<TPackage>() where TPackage : class, IPackage, new()
        {
            if (ReadIsNull())
            {
                return default;
            }
            else
            {
                TPackage package = new TPackage();
                package.Unpackage(this);
                return package;
            }
        }

        /// <summary>
        /// 以包进行写入。允许null值。
        /// 读取时调用<see cref="ReadPackage"/>，解包。或者先判断<see cref="ReadIsNull"/>，然后自行解包。
        /// </summary>
        /// <typeparam name="TPackage"></typeparam>
        /// <param name="package"></param>
        /// <returns></returns>
        public ByteBlock WritePackage<TPackage>(TPackage package) where TPackage : class, IPackage
        {
            WriteIsNull(package);
            if (package != null)
            {
                package.Package(this);
            }

            return this;
        }

        #endregion Package

        #region DateTime

        /// <summary>
        /// 从当前流位置读取一个<see cref="DateTime"/>值
        /// </summary>
        public DateTime ReadDateTime()
        {
            long value = ThingsGatewayBitConverter.Default.ToInt64(Buffer, Pos);
            Pos += 8;
            return DateTime.FromBinary(value);
        }

        /// <summary>
        /// 写入<see cref="DateTime"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(DateTime value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value.ToBinary()));
            return this;
        }

        #endregion DateTime

        #region TimeSpan

        /// <summary>
        /// 从当前流位置读取一个<see cref="TimeSpan"/>值
        /// </summary>
        public TimeSpan ReadTimeSpan()
        {
            long value = ThingsGatewayBitConverter.Default.ToInt64(Buffer, Pos);
            Pos += 8;
            return TimeSpan.FromTicks(value);
        }

        /// <summary>
        /// 写入<see cref="TimeSpan"/>值
        /// </summary>
        /// <param name="value"></param>
        public ByteBlock Write(TimeSpan value)
        {
            Write(ThingsGatewayBitConverter.Default.GetBytes(value.Ticks));
            return this;
        }

        #endregion TimeSpan

        #region Object

        /// <summary>
        ///  从当前流位置读取一个泛型值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializationType"></param>
        /// <returns></returns>
        public T ReadObject<T>(SerializationType serializationType = SerializationType.FastBinary)
        {
            int length = ReadInt32();

            if (length == 0)
            {
                return default;
            }

            T obj;

            switch (serializationType)
            {
                case SerializationType.FastBinary:
                    {
                        obj = SerializeConvert.FastBinaryDeserialize<T>(Buffer, Pos);
                    }
                    break;

                case SerializationType.Json:
                    {
                        string jsonString = Encoding.UTF8.GetString(Buffer, Pos, length);
                        obj = SerializeConvert.JsonDeserializeFromString<T>(jsonString);
                    }
                    break;

                case SerializationType.Xml:
                    {
                        string jsonString = Encoding.UTF8.GetString(Buffer, Pos, length);
                        obj = SerializeConvert.XmlDeserializeFromString<T>(jsonString);
                    }
                    break;

                case SerializationType.SystemBinary:
                    {
                        obj = SerializeConvert.BinaryDeserialize<T>(Buffer, Pos, length);
                    }
                    break;

                default:
                    throw new Exception("未定义的序列化类型");
            }

            Pos += length;
            return obj;
        }

        /// <summary>
        /// 写入<see cref="object"/>值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="serializationType"></param>
        public ByteBlock WriteObject(object value, SerializationType serializationType = SerializationType.FastBinary)
        {
            if (value == null)
            {
                Write(0);
                return this;
            }
            byte[] data;
            switch (serializationType)
            {
                case SerializationType.FastBinary:
                    {
                        data = SerializeConvert.FastBinarySerialize(value);
                    }
                    break;

                case SerializationType.Json:
                    {
                        data = SerializeConvert.JsonSerializeToBytes(value);
                    }
                    break;

                case SerializationType.Xml:
                    {
                        data = Encoding.UTF8.GetBytes(SerializeConvert.XmlSerializeToString(value));
                    }
                    break;

                case SerializationType.SystemBinary:
                    {
                        data = SerializeConvert.BinarySerialize(value);
                    }
                    break;

                default:
                    throw new Exception("未定义的序列化类型");
            }

            Write(data.Length);
            Write(data);
            return this;
        }

        #endregion Object
    }
}