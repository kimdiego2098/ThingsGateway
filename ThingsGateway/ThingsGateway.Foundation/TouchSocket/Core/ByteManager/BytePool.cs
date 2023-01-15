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
using System.Linq;
using System.Runtime.CompilerServices;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 字节池
    /// </summary>
    public static class BytePool
    {
        private static readonly ConcurrentDictionary<long, BytesQueue> bytesDictionary = new ConcurrentDictionary<long, BytesQueue>();
        private static readonly Timer m_timer;
        private static long m_fullSize;
        private static long m_maxSize;

        static BytePool()
        {
            m_timer = new Timer((o) =>
            {
                Clear();
            }, null, 1000 * 60 * 60, 1000 * 60 * 60);
            KeyCapacity = 100;
            AutoZero = false;
            m_maxSize = 1024 * 1024 * 512;
            SetBlockSize(1024, 1024 * 1024 * 20);
            AddSizeKey(10240);
        }

        /// <summary>
        /// 表示内存池是否可用。
        /// <para>当业务太轻量级，且要求超高并发时（千万数量级别），可禁用内存池。</para>
        /// </summary>
        public static bool Disabled { get; set; }

        /// <summary>
        /// 回收内存时，自动归零
        /// </summary>
        public static bool AutoZero { get; set; }

        /// <summary>
        /// 键容量
        /// </summary>
        public static int KeyCapacity { get; set; }

        /// <summary>
        /// 单个块最大值
        /// </summary>
        public static int MaxBlockSize { get; private set; }

        /// <summary>
        /// 允许的内存池最大值
        /// </summary>
        public static long MaxSize
        {
            get => m_maxSize;
            set
            {
                if (value < 1024)
                {
                    value = 1024;
                }
                m_maxSize = value;
            }
        }

        /// <summary>
        /// 单个块最小值
        /// </summary>
        public static int MinBlockSize { get; private set; }

        /// <summary>
        /// 添加尺寸键
        /// </summary>
        /// <param name="byteSize"></param>
        /// <returns></returns>
        public static bool AddSizeKey(int byteSize)
        {
            if (bytesDictionary.TryAdd(byteSize, new BytesQueue(byteSize)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清理
        /// </summary>
        public static void Clear()
        {
            bytesDictionary.Clear();
            GC.Collect();
        }

        /// <summary>
        /// 确定是否包含指定尺寸键
        /// </summary>
        /// <param name="byteSize"></param>
        /// <returns></returns>
        public static bool ContainsSizeKey(int byteSize)
        {
            return bytesDictionary.ContainsKey(byteSize);
        }

        /// <summary>
        /// 获取所以内存键
        /// </summary>
        /// <returns></returns>
        public static long[] GetAllSizeKeys()
        {
            return bytesDictionary.Keys.ToArray();
        }

        /// <summary>
        /// 获取ByteBlock
        /// </summary>
        /// <param name="byteSize">长度</param>
        /// <param name="equalSize">要求长度相同</param>
        /// <returns></returns>
        public static ByteBlock GetByteBlock(int byteSize, bool equalSize)
        {
            return new ByteBlock(byteSize, equalSize);
        }

        /// <summary>
        ///  获取ValueByteBlock
        /// </summary>
        /// <param name="byteSize"></param>
        /// <param name="equalSize"></param>
        /// <returns></returns>
        public static ValueByteBlock GetValueByteBlock(int byteSize, bool equalSize)
        {
            return new ValueByteBlock(byteSize, equalSize);
        }

        /// <summary>
        /// 获取ByteBlock
        /// </summary>
        /// <param name="byteSize"></param>
        /// <returns></returns>
        public static ByteBlock GetByteBlock(int byteSize)
        {
            return new ByteBlock(byteSize, false);
        }

        /// <summary>
        /// 获取ValueByteBlock
        /// </summary>
        /// <param name="byteSize"></param>
        /// <returns></returns>
        public static ValueByteBlock GetValueByteBlock(int byteSize)
        {
            return new ValueByteBlock(byteSize, false);
        }

        /// <summary>
        /// 获取内存核心。获取的核心可以不用归还。
        /// 如果要调用<see cref="Recycle(byte[])"/>归还，切记不要有持久性引用。
        /// </summary>
        /// <param name="byteSize"></param>
        /// <param name="equalSize"></param>
        /// <returns></returns>
        public static byte[] GetByteCore(int byteSize, bool equalSize = false)
        {
            if (Disabled)
            {
                return new byte[byteSize];
            }

            BytesQueue bytesCollection;
            if (equalSize)
            {
                //等长
                if (bytesDictionary.TryGetValue(byteSize, out bytesCollection))
                {
                    if (bytesCollection.TryGet(out byte[] bytes))
                    {
                        m_fullSize -= byteSize;
                        return bytes;
                    }
                }
                else
                {
                    CheckKeyCapacity(byteSize);
                }
                return new byte[byteSize];
            }
            else
            {
                byteSize = HitSize(byteSize);
                //搜索已创建集合
                if (bytesDictionary.TryGetValue(byteSize, out bytesCollection))
                {
                    if (bytesCollection.TryGet(out byte[] bytes))
                    {
                        m_fullSize -= byteSize;
                        return bytes;
                    }
                }
                else
                {
                    CheckKeyCapacity(byteSize);
                }
                return new byte[byteSize];
            }
        }

        /// <summary>
        /// 获取内存池容量
        /// </summary>
        /// <returns></returns>
        public static long GetPoolSize()
        {
            long size = 0;
            foreach (var item in bytesDictionary.Values)
            {
                size += item.FullSize;
            }
            return size;
        }

        /// <summary>
        /// 回收内存核心。
        /// <para>注意：回收的内存，必须百分百确定该对象没有再被其他引用。不然这属于危险操作。</para>
        /// </summary>
        /// <param name="bytes"></param>
        public static void Recycle(byte[] bytes)
        {
            if (Disabled)
            {
                return;
            }
            if (bytes == null || bytes.Length > MaxBlockSize || bytes.Length < MinBlockSize)
            {
                return;
            }
            if (m_maxSize > m_fullSize)
            {
                if (bytesDictionary.TryGetValue(bytes.Length, out BytesQueue bytesQueue))
                {
                    if (AutoZero)
                    {
                        Array.Clear(bytes, 0, bytes.Length);
                    }
                    m_fullSize += bytes.Length;
                    bytesQueue.Add(bytes);
                }
            }
            else
            {
                long size = 0;
                foreach (var collection in bytesDictionary.Values)
                {
                    size += collection.FullSize;
                }
                m_fullSize = size;
            }
        }

        /// <summary>
        /// 移除尺寸键
        /// </summary>
        /// <param name="byteSize"></param>
        /// <returns></returns>
        public static bool RemoveSizeKey(int byteSize)
        {
            if (bytesDictionary.TryRemove(byteSize, out BytesQueue queue))
            {
                queue.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置内存块参数
        /// </summary>
        /// <param name="minBlockSize"></param>
        /// <param name="maxBlockSize"></param>
        public static void SetBlockSize(int minBlockSize, int maxBlockSize)
        {
            BytePool.MaxBlockSize = maxBlockSize;
            BytePool.MinBlockSize = minBlockSize;
            bytesDictionary.Clear();
        }

        private static void CheckKeyCapacity(int byteSize)
        {
            if (byteSize < MinBlockSize || byteSize > MaxBlockSize)
            {
                return;
            }
            if (bytesDictionary.Count < KeyCapacity)
            {
                bytesDictionary.TryAdd(byteSize, new BytesQueue(byteSize));
            }
            else
            {
                List<BytesQueue> bytesQueues = bytesDictionary.Values.ToList();
                bytesQueues.Sort((x, y) => { return x.m_referenced > y.m_referenced ? -1 : 1; });
                for (int i = (int)(bytesQueues.Count * 0.2); i < bytesQueues.Count; i++)
                {
                    if (bytesDictionary.TryRemove(bytesQueues[i].m_size, out BytesQueue queue))
                    {
                        queue.Clear();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int HitSize(int num)
        {
            if (num < MinBlockSize)
            {
                num = MinBlockSize;
            }

            if (num <= 10240)//10k
            {
                return 10240;
            }
            else if (num <= 65536)//64k
            {
                return 65536;
            }
            else if (num <= 102400)//100k
            {
                return 102400;
            }
            else if (num <= 524288) //512k
            {
                return 524288;
            }
            else if (num <= 1048576)//1Mb
            {
                return 1048576;
            }
            else if (num <= 1048576 * 2)//2Mb
            {
                return 1048576 * 2;
            }
            else if (num <= 1048576 * 3)//3Mb
            {
                return 1048576 * 3;
            }
            else if (num <= 1048576 * 4)//4Mb
            {
                return 1048576 * 4;
            }
            else if (num <= 1048576 * 5)//5Mb
            {
                return 1048576 * 5;
            }
            else if (num <= 1048576 * 6)//6Mb
            {
                return 1048576 * 6;
            }
            else if (num <= 1048576 * 7)//7Mb
            {
                return 1048576 * 7;
            }
            else if (num <= 1048576 * 8)//8Mb
            {
                return 1048576 * 8;
            }
            else if (num <= 1048576 * 9)//9Mb
            {
                return 1048576 * 9;
            }
            else if (num <= 10485760)//10Mb
            {
                return 10485760;
            }
            else if (num <= 1024 * 1024 * 12)//12Mb
            {
                return 1024 * 1024 * 12;
            }
            else if (num <= 1024 * 1024 * 15)//15Mb
            {
                return 1024 * 1024 * 15;
            }
            else if (num <= 1024 * 1024 * 18)//18Mb
            {
                return 1024 * 1024 * 18;
            }
            else if (num <= 1024 * 1024 * 20)//20Mb
            {
                return 1024 * 1024 * 20;
            }
            else if (num <= 1024 * 1024 * 30)//30Mb
            {
                return 1024 * 1024 * 30;
            }
            else if (num <= 1024 * 1024 * 40)//40Mb
            {
                return 1024 * 1024 * 40;
            }
            else if (num <= 1024 * 1024 * 50)//50Mb
            {
                return 1024 * 1024 * 50;
            }
            else if (num <= 1024 * 1024 * 100)//100Mb
            {
                return 1024 * 1024 * 100;
            }
            else if (num <= 1024 * 1024 * 500)//500Mb
            {
                return 1024 * 1024 * 500;
            }
            else if (num <= 1024 * 1024 * 1024)//1Gb
            {
                return 1024 * 1024 * 1024;
            }
            else
            {
                return num;
            }
        }
    }
}