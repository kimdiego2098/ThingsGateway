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
    /// 不可重入的Timer
    /// </summary>
    public class SingleTimer : DisposableObject
    {
        private readonly Action<SingleTimer> m_action1;
        private readonly Action<SingleTimer, object> m_action2;
        private readonly object m_state;
        private readonly Timer m_timer;
        private readonly Action m_action3;
        private int m_signal = 1;

        /// <summary>
        /// 是否暂停执行。
        /// </summary>
        public bool Pause { get; set; }

        /// <summary>
        /// 自启动以来执行的次数。
        /// </summary>
        public long Count { get; private set; }

        /// <summary>
        /// 不可重入的Timer
        /// </summary>
        /// <param name="action"></param>
        /// <param name="period"></param>
        public SingleTimer(int period, Action action)
        {
            m_timer = new Timer(OnTimer, null, 0, period);
            m_action3 = action;
            m_state = null;
        }

        /// <summary>
        /// 不可重入的Timer
        /// </summary>
        /// <param name="action"></param>
        /// <param name="period"></param>
        public SingleTimer(TimeSpan period, Action action)
        {
            m_timer = new Timer(OnTimer, null, TimeSpan.Zero, period);
            m_action3 = action;
            m_state = null;
        }

        /// <summary>
        /// 不可重入的Timer
        /// </summary>
        /// <param name="action"></param>
        /// <param name="period"></param>
        public SingleTimer(int period, Action<SingleTimer> action)
        {
            m_timer = new Timer(OnTimer, null, 0, period);
            m_action1 = action;
            m_state = null;
        }

        /// <summary>
        /// 不可重入的Timer
        /// </summary>
        /// <param name="action"></param>
        /// <param name="period"></param>
        public SingleTimer(int period, Action<SingleTimer, object> action)
        {
            m_timer = new Timer(OnTimer, null, 0, period);
            m_action2 = action;
            m_state = null;
        }

        /// <summary>
        /// 不可重入的Timer
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="period"></param>
        public SingleTimer(object state, TimeSpan period, Action<SingleTimer> action)
        {
            m_timer = new Timer(OnTimer, state, TimeSpan.Zero, period);
            m_action1 = action;
            m_state = state;
        }

        /// <summary>
        /// 不可重入的Timer
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="period"></param>
        public SingleTimer(object state, TimeSpan period, Action<SingleTimer, object> action)
        {
            m_timer = new Timer(OnTimer, state, TimeSpan.Zero, period);
            m_action2 = action;
            m_state = state;
        }

        /// <summary>
        /// 不可重入的Timer
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="period"></param>
        public SingleTimer(object state, int period, Action<SingleTimer> action)
        {
            m_timer = new Timer(OnTimer, state, 0, period);
            m_action1 = action;
            m_state = state;
        }

        /// <summary>
        /// 不可重入的Timer
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="period"></param>
        public SingleTimer(object state, int period, Action<SingleTimer, object> action)
        {
            m_timer = new Timer(OnTimer, state, 0, period);
            m_action2 = action;
            m_state = state;
        }

        private void OnTimer(object state)
        {
            if (Pause)
            {
                return;
            }
            if (Interlocked.Decrement(ref m_signal) == 0)
            {
                try
                {
                    Count++;
                    m_action1?.Invoke(this);
                    m_action2?.Invoke(this, m_state);
                    m_action3?.Invoke();
                }
                catch
                {
                }
                finally
                {
                    m_signal = 1;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            m_timer.SafeDispose();
            base.Dispose(disposing);
        }
    }
}