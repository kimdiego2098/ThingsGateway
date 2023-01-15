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
    /// <inheritdoc/>
    /// </summary>
    public static class LoggerExtensions
    {
        #region LoggerGroup日志

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出中断日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Critical<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Critical, null, msg, null);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出调试日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Debug<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Debug, null, msg, null);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出错误日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Error<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Error, null, msg, null);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出错误日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void Error<TLog>(this ILog logger, object source, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Error, source, msg, null);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出异常日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void Exception<TLog>(this ILog logger, Exception ex) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Error, null, ex.Message, ex);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出异常日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="ex"></param>
        public static void Exception<TLog>(this ILog logger, object source, Exception ex) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Error, source, ex.Message, ex);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出消息日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Info<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Info, null, msg, null);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出消息日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void Info<TLog>(this ILog logger, object source, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Info, source, msg, null);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="logger"></param>
        public static void Log<TLog>(this ILog logger, LogType logType, object source, string message, Exception exception) where TLog : ILog
        {
            if (logger is LoggerGroup loggerGroup)
            {
                loggerGroup.Log<TLog>(logType, source, message, exception);
            }
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出详细日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Trace<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Trace, null, msg, null);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出警示日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Warning<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Warning, null, msg, null);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出警示日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void Warning<TLog>(this ILog logger, object source, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogType.Warning, source, msg, null);
        }

        #endregion LoggerGroup日志

        #region 日志

        /// <summary>
        /// 输出中断日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Critical(this ILog logger, string msg)
        {
            logger.Log(LogType.Critical, null, msg, null);
        }

        /// <summary>
        /// 输出调试日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Debug(this ILog logger, string msg)
        {
            logger.Log(LogType.Debug, null, msg, null);
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Error(this ILog logger, string msg)
        {
            logger.Log(LogType.Error, null, msg, null);
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void Error(this ILog logger, object source, string msg)
        {
            logger.Log(LogType.Error, source, msg, null);
        }

        /// <summary>
        /// 输出异常日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void Exception(this ILog logger, Exception ex)
        {
            logger.Log(LogType.Error, null, ex.Message, ex);
        }

        /// <summary>
        /// 输出异常日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="ex"></param>
        public static void Exception(this ILog logger, object source, Exception ex)
        {
            logger.Log(LogType.Error, source, ex.Message, ex);
        }

        /// <summary>
        /// 输出消息日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Info(this ILog logger, string msg)
        {
            logger.Log(LogType.Info, null, msg, null);
        }

        /// <summary>
        /// 输出消息日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void Info(this ILog logger, object source, string msg)
        {
            logger.Log(LogType.Info, source, msg, null);
        }

        /// <summary>
        /// 输出详细日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Trace(this ILog logger, string msg)
        {
            logger.Log(LogType.Trace, null, msg, null);
        }

        /// <summary>
        /// 输出警示日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Warning(this ILog logger, string msg)
        {
            logger.Log(LogType.Warning, null, msg, null);
        }

        /// <summary>
        /// 输出警示日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void Warning(this ILog logger, object source, string msg)
        {
            logger.Log(LogType.Warning, source, msg, null);
        }

        #endregion 日志
    }
}