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
using System.IO;
using System.Text;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 文件日志记录器
    /// <para>会在指定目录下，生成logs文件夹，然后按[yyyy-MM-dd].log的形式，每日生成日志</para>
    /// </summary>
    public class FileLogger : LoggerBase
    {
        private static string m_rootPath;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rootPath">日志根目录</param>
        public FileLogger(string rootPath = null)
        {
            lock (typeof(FileLogger))
            {
                rootPath ??= AppDomain.CurrentDomain.BaseDirectory;

                if (m_rootPath.IsNullOrEmpty())
                {
                    m_rootPath = Path.Combine(rootPath, "logs");
                }
                else if (m_rootPath != Path.Combine(rootPath, "logs"))
                {
                    throw new Exception($"{GetType().Name}无法指向不同的根路径。");
                }
            }
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
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff"));
            stringBuilder.Append(" | ");
            stringBuilder.Append(logType.ToString());
            stringBuilder.Append(" | ");
            stringBuilder.Append(message);

            if (exception != null)
            {
                stringBuilder.Append(" | ");
                stringBuilder.Append($"【异常消息】：{exception.Message}");
                stringBuilder.Append($"【堆栈】：{(exception == null ? "未知" : exception.StackTrace)}");
            }
            stringBuilder.AppendLine();

            Print(stringBuilder.ToString());
        }

        private static FileStorageWriter m_writer;

        private void Print(string logString)
        {
            try
            {
                lock (typeof(FileLogger))
                {
                    string dir = Path.Combine(m_rootPath, DateTime.Now.ToString("[yyyy-MM-dd]"));
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    if (m_writer == null)
                    {
                        int count = 0;
                        string path = null;
                        while (true)
                        {
                            path = Path.Combine(dir, $"{count:0000}" + ".log");
                            if (!File.Exists(path))
                            {
                                m_writer = FilePool.GetWriter(path);
                                break;
                            }
                            count++;
                        }
                    }
                    m_writer.Write(Encoding.UTF8.GetBytes(logString));
                    if (m_writer.FileStorage.Length > 1024 * 1024)
                    {
                        m_writer.SafeDispose();
                        m_writer = null;
                    }
                }
            }
            catch
            {
                m_writer.SafeDispose();
                m_writer = null;
            }
        }
    }
}