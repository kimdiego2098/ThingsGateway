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
using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 等待数据对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WaitData<T> : DisposableObject
    {
        private readonly AutoResetEvent m_waitHandle;
        private WaitDataStatus m_status;
        private T m_waitResult;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WaitData()
        {
            m_waitHandle = new AutoResetEvent(false);
        }

        /// <summary>
        /// 延迟模式
        /// </summary>
        public bool DelayModel { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public WaitDataStatus Status => m_status;

        /// <summary>
        /// 等待数据结果
        /// </summary>
        public T WaitResult => m_waitResult;

        /// <summary>
        /// 取消任务
        /// </summary>
        public void Cancel()
        {
            m_status = WaitDataStatus.Canceled;
            if (!DelayModel)
            {
                m_waitHandle.Set();
            }
        }

        /// <summary>
        /// Reset。
        /// 设置<see cref="WaitResult"/>为null。然后重置状态为<see cref="WaitDataStatus.Default"/>，waitHandle.Reset()
        /// </summary>
        public bool Reset()
        {
            m_status = WaitDataStatus.Default;
            m_waitResult = default;
            if (!DelayModel)
            {
                return m_waitHandle.Reset();
            }
            return true;
        }

        /// <summary>
        /// 使等待的线程继续执行
        /// </summary>
        public bool Set()
        {
            m_status = WaitDataStatus.SetRunning;
            if (!DelayModel)
            {
                return m_waitHandle.Set();
            }
            return true;
        }

        /// <summary>
        /// 使等待的线程继续执行
        /// </summary>
        /// <param name="waitResult">等待结果</param>
        public bool Set(T waitResult)
        {
            m_waitResult = waitResult;
            m_status = WaitDataStatus.SetRunning;
            if (!DelayModel)
            {
                return m_waitHandle.Set();
            }
            return true;
        }

        /// <summary>
        /// 加载取消令箭
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void SetCancellationToken(CancellationToken cancellationToken)
        {
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(Cancel);
            }
        }

        /// <summary>
        /// 载入结果
        /// </summary>
        public void SetResult(T result)
        {
            m_waitResult = result;
        }

        /// <summary>
        /// 等待指定时间
        /// </summary>
        /// <param name="timeSpan"></param>
        public WaitDataStatus Wait(TimeSpan timeSpan)
        {
            return this.Wait((int)timeSpan.TotalMilliseconds);
        }

        /// <summary>
        /// 等待指定毫秒
        /// </summary>
        /// <param name="millisecond"></param>
        public WaitDataStatus Wait(int millisecond)
        {
            if (DelayModel)
            {
                for (int i = 0; i < millisecond / 10.0; i++)
                {
                    if (m_status != WaitDataStatus.Default)
                    {
                        return m_status;
                    }
                    Task.Delay(10).GetAwaiter().GetResult();
                }
                m_status = WaitDataStatus.Overtime;
                return m_status;
            }
            else
            {
                if (!m_waitHandle.WaitOne(millisecond))
                {
                    m_status = WaitDataStatus.Overtime;
                }
                return m_status;
            }
        }

        /// <summary>
        /// 等待指定时间
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public Task<WaitDataStatus> WaitAsync(TimeSpan timeSpan)
        {
            return this.WaitAsync((int)timeSpan.TotalMilliseconds);
        }

        /// <summary>
        /// 等待指定毫秒
        /// </summary>
        /// <param name="millisecond"></param>
        public async Task<WaitDataStatus> WaitAsync(int millisecond)
        {
            if (DelayModel)
            {
                for (int i = 0; i < millisecond / 10.0; i++)
                {
                    if (m_status != WaitDataStatus.Default)
                    {
                        return m_status;
                    }
                    await Task.Delay(10);
                }
                m_status = WaitDataStatus.Overtime;
                return m_status;
            }
            else
            {
                if (!m_waitHandle.WaitOne(millisecond))
                {
                    m_status = WaitDataStatus.Overtime;
                }
                return m_status;
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            m_status = WaitDataStatus.Disposed;
            m_waitResult = default;
            m_waitHandle.SafeDispose();
            base.Dispose(disposing);
        }
    }
}