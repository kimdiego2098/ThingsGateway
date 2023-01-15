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
    /// 等待处理数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WaitHandlePool<T> : IDisposable where T : IWaitResult
    {
        private readonly ConcurrentDictionary<long, WaitData<T>> m_waitDic;
        private readonly ConcurrentQueue<WaitData<T>> m_waitQueue;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WaitHandlePool()
        {
            m_waitDic = new ConcurrentDictionary<long, WaitData<T>>();
            m_waitQueue = new ConcurrentQueue<WaitData<T>>();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="waitData"></param>
        public void Destroy(WaitData<T> waitData)
        {
            if (waitData.DisposedValue)
            {
                throw new ObjectDisposedException(nameof(waitData));
            }
            if (m_waitDic.TryRemove(waitData.WaitResult.Sign, out _))
            {
                waitData.Reset();
                m_waitQueue.Enqueue(waitData);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            foreach (var item in m_waitDic.Values)
            {
                item.Dispose();
            }
            foreach (var item in m_waitQueue)
            {
                item.Dispose();
            }
            m_waitDic.Clear();

            m_waitQueue.Clear();
        }

        /// <summary>
        /// 取消全部
        /// </summary>
        public void CancelAll()
        {
            foreach (var item in m_waitDic.Values)
            {
                item.Cancel();
            }
        }

        /// <summary>
        /// 延迟模式
        /// </summary>
        public bool DelayModel { get; set; } = false;

        private long m_waitCount;
        private long m_waitReverseCount;

        /// <summary>
        ///  获取一个可等待对象
        /// </summary>
        /// <param name="result"></param>
        /// <param name="autoSign">设置为false时，不会生成sign</param>
        /// <returns></returns>
        public WaitData<T> GetWaitData(T result, bool autoSign = true)
        {
            if (m_waitQueue.TryDequeue(out var waitData))
            {
                if (autoSign)
                {
                    result.Sign = Interlocked.Increment(ref m_waitCount);
                }
                waitData.SetResult(result);
                m_waitDic.TryAdd(result.Sign, waitData);
                return waitData;
            }

            waitData = new WaitData<T>();
            waitData.DelayModel = DelayModel;
            if (autoSign)
            {
                result.Sign = Interlocked.Increment(ref m_waitCount);
            }
            waitData.SetResult(result);
            m_waitDic.TryAdd(result.Sign, waitData);
            return waitData;
        }

        /// <summary>
        ///  获取一个Sign为负数的可等待对象
        /// </summary>
        /// <param name="result"></param>
        /// <param name="autoSign">设置为false时，不会生成sign</param>
        /// <returns></returns>
        public WaitData<T> GetReverseWaitData(T result, bool autoSign = true)
        {
            if (m_waitQueue.TryDequeue(out var waitData))
            {
                if (autoSign)
                {
                    result.Sign = Interlocked.Decrement(ref m_waitReverseCount);
                }
                waitData.SetResult(result);
                m_waitDic.TryAdd(result.Sign, waitData);
                return waitData;
            }

            waitData = new WaitData<T>();
            waitData.DelayModel = DelayModel;
            if (autoSign)
            {
                result.Sign = Interlocked.Decrement(ref m_waitReverseCount);
            }
            waitData.SetResult(result);
            m_waitDic.TryAdd(result.Sign, waitData);
            return waitData;
        }

        /// <summary>
        /// 让等待对象恢复运行
        /// </summary>
        /// <param name="sign"></param>
        public void SetRun(long sign)
        {
            WaitData<T> waitData;
            if (m_waitDic.TryGetValue(sign, out waitData))
            {
                waitData.Set();
            }
        }

        /// <summary>
        /// 让等待对象恢复运行
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="waitResult"></param>
        public void SetRun(long sign, T waitResult)
        {
            WaitData<T> waitData;
            if (m_waitDic.TryGetValue(sign, out waitData))
            {
                waitData.Set(waitResult);
            }
        }

        /// <summary>
        /// 让等待对象恢复运行
        /// </summary>
        /// <param name="waitResult"></param>
        public void SetRun(T waitResult)
        {
            if (m_waitDic.TryGetValue(waitResult.Sign, out WaitData<T> waitData))
            {
                waitData.Set(waitResult);
            }
        }
    }
}