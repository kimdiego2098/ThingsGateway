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
using System.IO;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 阻塞式单项读取流。
    /// </summary>
    public abstract class BlockReadStream : Stream, IWrite
    {
        private readonly AutoResetEvent m_inputEvent;
        private readonly AutoResetEvent m_readEvent;
        private byte[] m_buffer;
        private bool m_dis;
        private volatile int m_length;
        private volatile int m_offset;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BlockReadStream()
        {
            m_readEvent = new AutoResetEvent(false);
            m_inputEvent = new AutoResetEvent(false);
            ReadTimeout = 5000;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 还剩余的未读取的长度
        /// </summary>
        public int CanReadLen
        {
            get
            {
                if (m_dis)
                {
                    return 0;
                }
                return m_length - m_offset;
            }
        }

        /// <summary>
        /// 不可使用
        /// </summary>
        public override bool CanSeek => false;

        /// <summary>
        /// 不可使用
        /// </summary>
        public override long Length => throw new NotImplementedException();

        /// <summary>
        ///  不可使用
        /// </summary>
        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override int ReadTimeout { get; set; }

        /// <summary>
        /// 阻塞读取。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
            {
                throw new Exception("该流不允许读取。");
            }
            int r;
            if (m_length > 0)
            {
                if (m_length > count)
                {
                    //按count读取
                    Array.Copy(m_buffer, m_offset, buffer, offset, count);
                    m_length -= count;
                    m_offset += count;
                    r = count;
                }
                else
                {
                    //会读完本次
                    Array.Copy(m_buffer, m_offset, buffer, offset, m_length);
                    r = m_length;
                    Reset();
                }
            }
            else
            {
                //无数据，须等待
                if (m_readEvent.WaitOne(ReadTimeout))
                {
                    if (m_length == 0)
                    {
                        Reset();
                        r = 0;
                    }
                    else if (m_length > count)
                    {
                        //按count读取
                        Array.Copy(m_buffer, m_offset, buffer, offset, count);
                        m_length -= count;
                        m_offset += count;
                        r = count;
                    }
                    else
                    {
                        //会读完本次
                        Array.Copy(m_buffer, m_offset, buffer, offset, m_length);
                        r = m_length;
                        Reset();
                    }
                }
                else
                {
                    throw new TimeoutException();
                }
            }
            return r;
        }

        /// <summary>
        /// 不可使用
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 不可使用
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (m_dis)
            {
                return;
            }
            m_dis = true;
            Reset();
            m_readEvent.SafeDispose();
            m_inputEvent.SafeDispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// 传输输入.
        /// 必须以length为0结束。读取端会超时。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected bool Input(byte[] buffer, int offset, int length)
        {
            m_inputEvent.Reset();
            m_buffer = buffer;
            m_offset = offset;
            m_length = length;
            m_readEvent.Set();
            return m_inputEvent.WaitOne(ReadTimeout);
        }

        private void Reset()
        {
            m_buffer = null;
            m_offset = 0;
            m_length = 0;
            m_readEvent.Reset();
            m_inputEvent.Set();
        }
    }
}