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
    /// 日志基类
    /// </summary>
    public abstract class LoggerBase : ILog
    {
        /// <summary>
        /// 全部的日志类型
        /// </summary>
        public const LogType All = LogType.None | LogType.Trace | LogType.Debug | LogType.Info | LogType.Warning | LogType.Error | LogType.Critical;

        /// <summary>
        /// 日志基类
        /// </summary>
        protected LoggerBase()
        {
            LogType = All;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public LogType LogType { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Log(LogType logType, object source, string message, Exception exception)
        {
            if (LogType.HasFlag(logType))
            {
                WriteLog(logType, source, message, exception);
            }
        }

        /// <summary>
        /// 筛选日志后输出
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        protected abstract void WriteLog(LogType logType, object source, string message, Exception exception);
    }
}