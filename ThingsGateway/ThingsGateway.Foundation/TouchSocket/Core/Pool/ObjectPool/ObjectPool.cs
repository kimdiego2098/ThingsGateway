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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> : IObjectPool where T : IPoolObject
    {
        private readonly ConcurrentQueue<T> m_queue = new ConcurrentQueue<T>();

        private bool m_autoCreate = true;

        private int m_freeSize;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="capacity"></param>
        public ObjectPool(int capacity)
        {
            Capacity = capacity;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ObjectPool()
        {
        }

        /// <summary>
        /// 是否自动生成
        /// </summary>
        public bool AutoCreate
        {
            get => m_autoCreate;
            set => m_autoCreate = value;
        }

        /// <summary>
        /// 对象池容量
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 可使用（创建）数量
        /// </summary>
        public int FreeSize => m_freeSize;

        /// <summary>
        /// 清除池中所有对象
        /// </summary>
        public void Clear()
        {
            while (m_queue.TryDequeue(out _))
            {
            }
        }

        /// <summary>
        /// 注销对象
        /// </summary>
        /// <param name="t"></param>
        public void DestroyObject(T t)
        {
            t.Destroy();
            if (m_freeSize < Capacity)
            {
                Interlocked.Increment(ref m_freeSize);
                m_queue.Enqueue(t);
            }
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            Clear();
        }

        /// <summary>
        /// 获取所有对象
        /// </summary>
        /// <returns></returns>
        public T[] GetAllObject()
        {
            List<T> ts = new List<T>();
            while (m_queue.TryDequeue(out T t))
            {
                ts.Add(t);
            }
            return ts.ToArray();
        }

        /// <summary>
        /// 获取对象T
        /// </summary>
        /// <returns></returns>
        public T GetObject()
        {
            if (m_queue.TryDequeue(out T t))
            {
                t.Recreate();
                t.NewCreate = false;
                Interlocked.Decrement(ref m_freeSize);
                return t;
            }
            if (m_autoCreate)
            {
                t = (T)Activator.CreateInstance(typeof(T));
                t.Create();
                t.NewCreate = true;
            }
            return t;
        }

        /// <summary>
        /// 预获取
        /// </summary>
        /// <returns></returns>
        public T PreviewGetObject()
        {
            m_queue.TryPeek(out T t);
            return t;
        }
    }
}