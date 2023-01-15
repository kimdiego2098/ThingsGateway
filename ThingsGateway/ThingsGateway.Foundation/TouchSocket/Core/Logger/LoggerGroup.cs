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
using System.Collections.Generic;
using System.Linq;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 一组日志记录器
    /// </summary>
    public class LoggerGroup : LoggerBase
    {
        private readonly Dictionary<string, ILog> m_logs = new Dictionary<string, ILog>();

        /// <summary>
        /// 一组日志记录器
        /// </summary>
        /// <param name="logs"></param>
        public LoggerGroup(params ILog[] logs)
        {
            if (logs is null)
            {
                throw new ArgumentNullException(nameof(logs));
            }
            foreach (var log in logs)
            {
                this.AddLogger(log);
            }
        }

        /// <summary>
        ///  一组日志记录器
        /// </summary>
        [DependencyInject]
        public LoggerGroup()
        {
        }

        /// <summary>
        /// 组内的日志记录器
        /// </summary>
        public ILog[] Logs => m_logs.Values.ToArray();

        /// <summary>
        /// 添加日志组件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="logger"></param>
        public void AddLogger(string key, ILog logger)
        {
            m_logs.Add(key, logger);
        }

        /// <summary>
        /// 添加日志组件
        /// </summary>
        /// <param name="logger"></param>
        public void AddLogger(ILog logger)
        {
            m_logs.Add(logger.GetType().FullName, logger);
        }

        /// <summary>
        /// 指定输出<see cref="LoggerGroup"/>中的特定类型的日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Log<TLog>(LogType logType, object source, string message, Exception exception) where TLog : ILog
        {
            try
            {
                for (int i = 0; i < m_logs.Count; i++)
                {
                    ILog log = Logs[i];
                    if (log.GetType() == typeof(TLog))
                    {
                        log.Log(logType, source, message, exception);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 移除日志
        /// </summary>
        /// <param name="logger"></param>
        public bool RemoveLogger(ILog logger)
        {
            return this.RemoveLogger(logger.GetType().FullName);
        }

        /// <summary>
        /// 移除日志
        /// </summary>
        public bool RemoveLogger(Type loggerType)
        {
            return this.RemoveLogger(loggerType.FullName);
        }

        /// <summary>
        /// 移除对应键的日志
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveLogger(string key)
        {
            return m_logs.Remove(key);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        protected override void WriteLog(LogType logType, object source, string message, Exception exception)
        {
            try
            {
                for (int i = 0; i < m_logs.Count; i++)
                {
                    Logs[i].Log(logType, source, message, exception);
                }
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// 一组日志记录器
    /// </summary>
    /// <typeparam name="TLog1"></typeparam>
    /// <typeparam name="TLog2"></typeparam>
    public class LoggerGroup<TLog1, TLog2> : LoggerGroup
        where TLog1 : ILog
        where TLog2 : ILog
    {
        /// <summary>
        /// 一组日志记录器
        /// </summary>
        public LoggerGroup(TLog1 log1, TLog2 log2) : base(log1, log2)
        {
        }
    }

    /// <summary>
    /// 一组日志记录器
    /// </summary>
    /// <typeparam name="TLog1"></typeparam>
    /// <typeparam name="TLog2"></typeparam>
    /// <typeparam name="TLog3"></typeparam>
    public class LoggerGroup<TLog1, TLog2, TLog3> : LoggerGroup
        where TLog1 : ILog
        where TLog2 : ILog
        where TLog3 : ILog
    {
        /// <summary>
        /// 一组日志记录器
        /// </summary>
        public LoggerGroup(TLog1 log1, TLog2 log2, TLog3 log3) : base(log1, log2, log3)
        {
        }
    }

    /// <summary>
    /// 一组日志记录器
    /// </summary>
    /// <typeparam name="TLog1"></typeparam>
    /// <typeparam name="TLog2"></typeparam>
    /// <typeparam name="TLog3"></typeparam>
    /// <typeparam name="TLog4"></typeparam>
    public class LoggerGroup<TLog1, TLog2, TLog3, TLog4> : LoggerGroup
        where TLog1 : ILog
        where TLog2 : ILog
        where TLog3 : ILog
        where TLog4 : ILog
    {
        /// <summary>
        /// 一组日志记录器
        /// </summary>
        public LoggerGroup(TLog1 log1, TLog2 log2, TLog3 log3, TLog4 log4) : base(log1, log2, log3, log4)
        {
        }
    }

    /// <summary>
    /// 一组日志记录器
    /// </summary>
    /// <typeparam name="TLog1"></typeparam>
    /// <typeparam name="TLog2"></typeparam>
    /// <typeparam name="TLog3"></typeparam>
    /// <typeparam name="TLog4"></typeparam>
    /// <typeparam name="TLog5"></typeparam>
    public class LoggerGroup<TLog1, TLog2, TLog3, TLog4, TLog5> : LoggerGroup
        where TLog1 : ILog
        where TLog2 : ILog
        where TLog3 : ILog
        where TLog4 : ILog
        where TLog5 : ILog
    {
        /// <summary>
        /// 一组日志记录器
        /// </summary>
        public LoggerGroup(TLog1 log1, TLog2 log2, TLog3 log3, TLog4 log4, TLog5 log5) : base(log1, log2, log3, log4, log5)
        {
        }
    }

    /// <summary>
    /// 一组日志记录器
    /// </summary>
    /// <typeparam name="TLog1"></typeparam>
    /// <typeparam name="TLog2"></typeparam>
    /// <typeparam name="TLog3"></typeparam>
    /// <typeparam name="TLog4"></typeparam>
    /// <typeparam name="TLog5"></typeparam>
    /// <typeparam name="TLog6"></typeparam>
    public class LoggerGroup<TLog1, TLog2, TLog3, TLog4, TLog5, TLog6> : LoggerGroup
       where TLog1 : ILog
       where TLog2 : ILog
       where TLog3 : ILog
       where TLog4 : ILog
       where TLog5 : ILog
       where TLog6 : ILog
    {
        /// <summary>
        /// 一组日志记录器
        /// </summary>
        public LoggerGroup(TLog1 log1, TLog2 log2, TLog3 log3, TLog4 log4, TLog5 log5, TLog6 log6) : base(log1, log2, log3, log4, log5, log6)
        {
        }
    }
}