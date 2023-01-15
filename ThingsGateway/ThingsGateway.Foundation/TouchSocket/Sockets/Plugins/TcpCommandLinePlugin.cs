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


using System.Reflection;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// TCP命令行插件。
    /// </summary>
    public abstract class TcpCommandLinePlugin : TcpPluginBase
    {
        private readonly Dictionary<string, Method> m_pairs = new Dictionary<string, ThingsGateway.Foundation.Core.Method>();
        private ILog m_logger;

        /// <summary>
        /// TCP命令行插件。
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected TcpCommandLinePlugin(ILog logger)
        {
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Converter = new StringConverter();
            var ms = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(a => a.Name.EndsWith("Command"));
            foreach (var item in ms)
            {
                m_pairs.Add(item.Name.Replace("Command", string.Empty), new Method(item));
            }
        }

        /// <summary>
        /// 字符串转换器，默认支持基础类型和Json。可以自定义。
        /// </summary>
        public StringConverter Converter { get; }

        /// <summary>
        /// 是否返回执行异常。
        /// </summary>
        public bool ReturnException { get; set; } = true;

        /// <summary>
        /// 当有执行异常时，不返回异常。
        /// </summary>
        /// <returns></returns>
        public TcpCommandLinePlugin NoReturnException()
        {
            ReturnException = false;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnReceivedData(ITcpClientBase client, ReceivedDataEventArgs e)
        {
            try
            {
                string[] strs = e.ByteBlock.ToString().Split(' ');
                if (strs.Length > 0 && m_pairs.TryGetValue(strs[0], out Method method))
                {
                    var ps = method.Info.GetParameters();
                    object[] os = new object[ps.Length];
                    int index = 0;
                    for (int i = 0; i < ps.Length; i++)
                    {
                        if (ps[i].ParameterType.IsInterface && typeof(ITcpClientBase).IsAssignableFrom(ps[i].ParameterType))
                        {
                            os[i] = client;
                        }
                        else
                        {
                            os[i] = Converter.ConvertFrom(strs[index + 1], ps[i].ParameterType);
                            index++;
                        }
                    }
                    e.Handled = true;

                    try
                    {
                        object result = method.Invoke(this, os);
                        if (method.HasReturn)
                        {
                            client.Send(Converter.ConvertTo(result));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ReturnException)
                        {
                            client.Send(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_logger.Log(LogType.Error, this, ex.Message, ex);
            }
            base.OnReceivedData(client, e);
        }
    }
}