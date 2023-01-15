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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 阻塞式读取。
    /// </summary>
    public abstract class BlockReader : DisposableObject
    {
        private byte[] m_buffer;
        private readonly AutoResetEvent m_inputEvent;
        private volatile int m_offset;
        private readonly AutoResetEvent m_readEvent;
        private volatile int m_surLength;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BlockReader()
        {
            m_readEvent = new AutoResetEvent(false);
            m_inputEvent = new AutoResetEvent(false);
            ReadTimeout = 5000;
        }

        /// <summary>
        /// 可读
        /// </summary>
        public abstract bool CanRead { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int ReadTimeout { get; set; }

        /// <summary>
        /// 阻塞读取，但不会移动游标。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual int PeekRead(byte[] buffer, int offset, int count)
        {
            return PrivateRead(true, buffer, offset, count);
        }

        /// <summary>
        /// 阻塞读取。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual int Read(byte[] buffer, int offset, int count)
        {
            return PrivateRead(false, buffer, offset, count);
        }

        private int PrivateRead(bool peek, byte[] buffer, int offset, int count)
        {
            if (!CanRead)
            {
                throw new Exception("该流不允许读取。");
            }
            int r;
            if (m_surLength > 0)
            {
                if (m_surLength > count)
                {
                    //按count读取
                    Array.Copy(m_buffer, m_offset, buffer, offset, count);
                    if (!peek)
                    {
                        m_surLength -= count;
                        m_offset += count;
                    }
                    r = count;
                }
                else
                {
                    //会读完本次
                    Array.Copy(m_buffer, m_offset, buffer, offset, m_surLength);
                    r = m_surLength;
                    if (!peek)
                    {
                        Reset();
                    }
                }
            }
            else
            {
                //无数据，须等待
                if (m_readEvent.WaitOne(ReadTimeout))
                {
                    if (m_surLength == 0)
                    {
                        Reset();
                        r = 0;
                    }
                    else if (m_surLength > count)
                    {
                        //按count读取
                        Array.Copy(m_buffer, m_offset, buffer, offset, count);
                        if (!peek)
                        {
                            m_surLength -= count;
                            m_offset += count;
                        }
                        r = count;
                    }
                    else
                    {
                        //会读完本次
                        Array.Copy(m_buffer, m_offset, buffer, offset, m_surLength);
                        r = m_surLength;
                        if (!peek)
                        {
                            Reset();
                        }
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
        /// 传输输入.
        /// 当以length为0结束。
        /// 否则读取端会超时。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected bool Input(byte[] buffer, int offset, int length)
        {
            //if (this.disposedValue)
            //{
            //    return false;
            //}
            m_inputEvent.Reset();
            m_buffer = buffer;
            m_offset = offset;
            m_surLength = length;
            m_readEvent.Set();
            return m_inputEvent.WaitOne(ReadTimeout);
        }

        /// <summary>
        /// 输入完成
        /// </summary>
        protected bool InputComplate()
        {
            return Input(new byte[0], 0, 0);
        }

        private void Reset()
        {
            m_buffer = null;
            m_offset = 0;
            m_surLength = 0;
            m_readEvent.Reset();
            m_inputEvent.Set();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Reset();
            m_readEvent.SafeDispose();
            m_inputEvent.SafeDispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~BlockReader()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }
    }
}