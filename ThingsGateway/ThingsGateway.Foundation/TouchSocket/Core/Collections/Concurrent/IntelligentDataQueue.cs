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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 队列数据
    /// </summary>
    public interface IQueueData
    {
        /// <summary>
        /// 数据长度
        /// </summary>
        int Size { get; }
    }

    /// <summary>
    /// 传输字节
    /// </summary>
    public class QueueDataBytes : IQueueData
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public QueueDataBytes(byte[] buffer, int offset, int length)
        {
            Offset = offset;
            Length = length;
            Buffer = buffer;
            Size = length;
        }

        /// <summary>
        /// 从指定内存创建一个新对象，且内存也为新创建。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static QueueDataBytes CreateNew(byte[] buffer, int offset, int length)
        {
            byte[] buf = new byte[length];
            Array.Copy(buffer, offset, buf, 0, length);
            return new QueueDataBytes(buf);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="buffer"></param>
        public QueueDataBytes(byte[] buffer) : this(buffer, 0, buffer.Length)
        {
        }

        /// <summary>
        /// 数据内存
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 偏移
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public int Size { get; }
    }

    /// <summary>
    /// 智能数据安全队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntelligentDataQueue<T> : ConcurrentQueue<T> where T : IQueueData
    {
        private long m_actualSize;
        private bool m_free;
        private long m_maxSize;
        private Action<bool> m_onQueueChanged;
        private bool m_overflowWait;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxSize"></param>
        public IntelligentDataQueue(long maxSize)
        {
            m_free = true;
            m_overflowWait = true;
            MaxSize = maxSize;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IntelligentDataQueue() : this(1024 * 1024 * 10)
        {
        }

        /// <summary>
        /// 实际尺寸
        /// </summary>
        public long ActualSize => m_actualSize;

        /// <summary>
        /// 是否有空位允许入队
        /// </summary>
        public bool Free => m_free;

        /// <summary>
        /// 允许的最大长度
        /// </summary>
        public long MaxSize
        {
            get => m_maxSize;
            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                m_maxSize = value;
            }
        }

        /// <summary>
        /// 在队列修改时
        /// </summary>
        public Action<bool> OnQueueChanged
        {
            get => m_onQueueChanged;
            set => m_onQueueChanged = value;
        }

        /// <summary>
        /// 溢出等待
        /// </summary>
        public bool OverflowWait
        {
            get => m_overflowWait;
            set => m_overflowWait = value;
        }

        /// <summary>
        /// 超时时间。默认1000*30ms；
        /// </summary>
        public int Timeout { get; set; } = 1000 * 30;

        /// <summary>
        /// 清空队列
        /// </summary>
        public void Clear(Action<T> onClear)
        {
            while (base.TryDequeue(out T t))
            {
                onClear?.Invoke(t);
            }
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="item"></param>
        public new void Enqueue(T item)
        {
            lock (this)
            {
                bool free = m_actualSize < m_maxSize;
                if (m_free != free)
                {
                    m_free = free;
                    m_onQueueChanged?.Invoke(m_free);
                }

                if (m_overflowWait)
                {
                    SpinWait.SpinUntil(Check, Timeout);
                }

                Interlocked.Add(ref m_actualSize, item.Size);
                base.Enqueue(item);
            }
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public new bool TryDequeue(out T result)
        {
            if (base.TryDequeue(out result))
            {
                Interlocked.Add(ref m_actualSize, -result.Size);
                bool free = m_actualSize < m_maxSize;
                if (m_free != free)
                {
                    m_free = free;
                    m_onQueueChanged?.Invoke(m_free);
                }
                return true;
            }
            return false;
        }

        private bool Check()
        {
            return m_actualSize < m_maxSize;
        }
    }
}