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
using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 流量控制
    /// </summary>
    public class FlowGate
    {
        private readonly Stopwatch m_stopwatch;

        private long m_timeTick;

        private long m_transferLength;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FlowGate()
        {
            m_stopwatch = new Stopwatch();
        }

        /// <summary>
        /// 最大值
        /// </summary>
        public long Maximum { get; set; } = long.MaxValue;

        /// <summary>
        /// 最长休眠周期。默认为5*1000ms.
        /// <para>当设置为5000时，假如设置的<see cref="Maximum"/>=10，而一次递增了100，则理应会休眠10s，但是会休眠5s。反之，如果设置1，则每秒周期都会清空。</para>
        /// </summary>
        public int MaximumPeriod { get; set; } = 5000;

        /// <summary>
        /// 检测等待
        /// </summary>
        public void AddCheckWait(int increment)
        {
            if (GetNowTick() - m_timeTick > 0)
            {
                //时间过了一秒
                m_timeTick = FlowGate.GetNowTick();
                m_transferLength = 0;
                m_stopwatch.Restart();
            }
            else
            {
                //在这一秒中
                if (Interlocked.Add(ref m_transferLength, increment) > Maximum)
                {
                    //上传饱和
                    m_stopwatch.Stop();
                    int sleepTime = 1000 - (int)m_stopwatch.ElapsedMilliseconds <= 0 ? 0 : GetBaseNum() - (int)m_stopwatch.ElapsedMilliseconds;
                    Thread.Sleep(sleepTime);
                }
            }
        }

        /// <summary>
        /// 检测等待
        /// </summary>
        /// <param name="increment"></param>
        /// <returns></returns>
        public async Task AddCheckWaitAsync(int increment)
        {
            if (GetNowTick() - m_timeTick > 0)
            {
                //时间过了一秒
                m_timeTick = FlowGate.GetNowTick();
                m_transferLength = 0;
                m_stopwatch.Restart();
            }
            else
            {
                //在这一秒中
                if (Interlocked.Add(ref m_transferLength, increment) > Maximum)
                {
                    //上传饱和
                    m_stopwatch.Stop();
                    int sleepTime = 1000 - (int)m_stopwatch.ElapsedMilliseconds <= 0 ? 0 : GetBaseNum() - (int)m_stopwatch.ElapsedMilliseconds;
                    await Task.Delay(sleepTime);
                }
            }
        }

        private static long GetNowTick()
        {
            return DateTime.Now.Ticks / 10000000;
        }

        private int GetBaseNum()
        {
            return Math.Min((int)((double)m_transferLength / Maximum * 1000), MaximumPeriod);
        }
    }
}