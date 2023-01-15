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



namespace ThingsGateway.Foundation
{
    /// <summary>
    /// IPluginsManagerExtension
    /// </summary>
    public static class SocketPluginsManagerExtension
    {
        /// <summary>
        /// 使用断线重连。
        /// <para>该效果仅客户端在完成首次连接，且为被动断开时有效。</para>
        /// </summary>
        /// <param name="pluginsManager"></param>
        /// <param name="successCallback">成功回调函数</param>
        /// <param name="tryCount">尝试重连次数，设为-1时则永远尝试连接</param>
        /// <param name="printLog">是否输出日志。</param>
        /// <param name="sleepTime">失败时，停留时间</param>
        /// <returns></returns>
        public static IPluginsManager UseReconnection(this IPluginsManager pluginsManager, int tryCount = 10,
            bool printLog = false, int sleepTime = 1000, Action<ITcpClient> successCallback = null)
        {
            bool first = true;
            var reconnectionPlugin = new ReconnectionPlugin<ITcpClient>(client =>
            {
                int tryT = tryCount;
                while (tryCount < 0 || tryT-- > 0)
                {
                    try
                    {
                        if (client.Online)
                        {
                            return true;
                        }
                        else
                        {
                            if (first) Thread.Sleep(1000);
                            first = false;
                            client.Connect();
                            first = true;
                        }
                        successCallback?.Invoke(client);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        if (printLog)
                        {
                            client.Logger.Log(LogType.Error, client, "断线重连失败。", ex);
                        }
                        Thread.Sleep(sleepTime);
                    }
                }
                return true;
            });

            pluginsManager.Add(reconnectionPlugin);
            return pluginsManager;
        }

        /// <summary>
        ///  检查连接客户端活性插件。
        ///  <para>当在设置的周期内，没有接收/发送任何数据，则判定该客户端掉线。执行清理。默认配置：60秒为一个周期，同时检测发送和接收。</para>
        ///  仅服务器适用。
        /// </summary>
        /// <param name="pluginsManager"></param>
        /// <returns></returns>
        public static CheckClearPlugin UseCheckClear(this IPluginsManager pluginsManager)
        {
            return pluginsManager.Add<CheckClearPlugin>();
        }

        /// <summary>
        /// 使用断线重连。
        /// <para>该效果仅客户端在完成首次连接，且为被动断开时有效。</para>
        /// </summary>
        /// <param name="pluginsManager"></param>
        /// <param name="sleepTime">失败时间隔时间</param>
        /// <param name="failCallback">失败时回调（参数依次为：客户端，本轮尝试重连次数，异常信息）。如果回调为null或者返回false，则终止尝试下次连接。</param>
        /// <param name="successCallback">成功连接时回调。</param>
        /// <returns></returns>
        public static IPluginsManager UseReconnection(this IPluginsManager pluginsManager, int sleepTime,
            Func<ITcpClient, int, Exception, bool> failCallback,
            Action<ITcpClient> successCallback)
        {
            bool first = true;
            var reconnectionPlugin = new ReconnectionPlugin<ITcpClient>(client =>
            {
                int tryT = 0;
                while (true)
                {
                    try
                    {
                        if (client.Online)
                        {
                            return true;
                        }
                        else
                        {
                            if (first) Thread.Sleep(1000);
                            first = false;
                            client.Connect();
                            first = true;
                        }

                        successCallback?.Invoke(client);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(sleepTime);
                        if (failCallback?.Invoke(client, ++tryT, ex) != true)
                        {
                            return true;
                        }
                    }
                }
            });

            pluginsManager.Add(reconnectionPlugin);
            return pluginsManager;
        }
    }
}