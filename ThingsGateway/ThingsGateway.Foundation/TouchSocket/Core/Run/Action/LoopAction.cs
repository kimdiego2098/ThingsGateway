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
    /// 循环动作
    /// </summary>
    public class LoopAction : EasyTask, IDisposable
    {
        /// <summary>
        /// 析构函数
        /// </summary>
        ~LoopAction()
        {
            Dispose();
        }

        private int executedCount;

        private readonly TimeSpan m_interval;

        private readonly int m_loopCount;

        private readonly EventWaitHandle m_waitHandle;

        private LoopAction(int count, TimeSpan interval, Action<LoopAction> action)
        {
            m_loopCount = count;
            this.action = action;
            m_interval = interval;
            m_waitHandle = new AutoResetEvent(false);
        }

        /// <summary>
        /// 创建可循环操作体
        /// </summary>
        /// <param name="count">循环次数，设为-1时一直循环</param>
        /// <param name="interval">每次循环间隔</param>
        /// <param name="action">执行委托</param>
        /// <returns></returns>
        public static LoopAction CreateLoopAction(int count, TimeSpan interval, Action<LoopAction> action)
        {
            return new LoopAction(count, interval, action);
        }

        /// <summary>
        /// 创建可循环操作体
        /// </summary>
        /// <param name="count">循环次数，设为-1时一直循环</param>
        /// <param name="intervalMS">每次循环间隔，毫秒</param>
        /// <param name="action">执行委托</param>
        /// <returns></returns>
        public static LoopAction CreateLoopAction(int count, int intervalMS, Action<LoopAction> action)
        {
            return new LoopAction(count, TimeSpan.FromMilliseconds(intervalMS), action);
        }

        /// <summary>
        /// 创建可循环操作体
        /// </summary>
        /// <param name="count">循环次数，设为-1时一直循环</param>
        /// <param name="action">执行委托</param>
        /// <returns></returns>
        public static LoopAction CreateLoopAction(int count, Action<LoopAction> action)
        {
            return CreateLoopAction(count, TimeSpan.Zero, action);
        }

        /// <summary>
        /// 创建可循环操作体
        /// </summary>
        /// <param name="interval">每次循环间隔</param>
        /// <param name="action">执行委托</param>
        /// <returns></returns>
        public static LoopAction CreateLoopAction(TimeSpan interval, Action<LoopAction> action)
        {
            return CreateLoopAction(-1, interval, action);
        }

        /// <summary>
        /// 创建可循环操作体
        /// </summary>
        /// <param name="action">执行委托</param>
        /// <returns></returns>
        public static LoopAction CreateLoopAction(Action<LoopAction> action)
        {
            return CreateLoopAction(-1, TimeSpan.Zero, action);
        }

        /// <summary>
        /// 已执行次数
        /// </summary>
        public int ExecutedCount => executedCount;

        /// <summary>
        /// 执行间隔
        /// </summary>
        public TimeSpan Interval => m_interval;

        /// <summary>
        /// 循环次数
        /// </summary>
        public int LoopCount => m_loopCount;

        private readonly Action<LoopAction> action;

        /// <summary>
        /// 执行委托
        /// </summary>
        public Action<LoopAction> ExecuteAction => action;

        private RunStatus runStatus;

        /// <summary>
        /// 是否在运行
        /// </summary>
        public RunStatus RunStatus => runStatus;

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            if (runStatus == RunStatus.None)
            {
                runStatus = RunStatus.Running;
                if (m_loopCount >= 0)
                {
                    for (int i = 0; i < m_loopCount; i++)
                    {
                        if (runStatus == RunStatus.Disposed)
                        {
                            return;
                        }
                        action.Invoke(this);
                        executedCount++;
                        if (runStatus == RunStatus.Paused)
                        {
                            m_waitHandle.WaitOne();
                        }
                        m_waitHandle.WaitOne(m_interval);
                    }
                }
                else
                {
                    while (true)
                    {
                        if (runStatus == RunStatus.Disposed)
                        {
                            return;
                        }
                        action.Invoke(this);
                        executedCount++;
                        if (runStatus == RunStatus.Paused)
                        {
                            m_waitHandle.WaitOne();
                        }
                        m_waitHandle.WaitOne(m_interval);
                    }
                }
                runStatus = RunStatus.Completed;
            }
        }

        /// <summary>
        /// 重新运行
        /// </summary>
        public void Rerun()
        {
            if (runStatus == RunStatus.Disposed)
            {
                throw new Exception("无法利用已释放的资源");
            }
            runStatus = RunStatus.None;
            Run();
        }

        /// <summary>
        /// 以异步重新运行
        /// </summary>
        /// <returns></returns>
        public Task RerunAsync()
        {
            if (runStatus == RunStatus.Disposed)
            {
                throw new Exception("无法利用已释放的资源");
            }
            runStatus = RunStatus.None;
            return RunAsync();
        }

        /// <summary>
        /// 以异步运行
        /// </summary>
        /// <returns></returns>
        public Task RunAsync()
        {
            return EasyTask.Run(() =>
            {
                Run();
            });
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (runStatus == RunStatus.Running)
            {
                m_waitHandle.Reset();
                runStatus = RunStatus.Paused;
            }
        }

        /// <summary>
        /// 回复
        /// </summary>
        public void Resume()
        {
            if (runStatus == RunStatus.Paused)
            {
                runStatus = RunStatus.Running;
                m_waitHandle.Set();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (runStatus == RunStatus.Disposed)
            {
                return;
            }
            if (runStatus == RunStatus.Completed)
            {
                m_waitHandle.Dispose();
            }
            runStatus = RunStatus.Disposed;
        }
    }
}