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
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ThingsGateway.Foundation;
//using ThingsGateway.Foundation;

//namespace ThingsGateway.Foundation.Sockets
//{
//    /// <summary>
//    /// IServicePlugin
//    /// </summary>
//    public interface IServicePlugin:IPlugin
//    {
//        /// <summary>
//        /// 当服务器成功启动时
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        [AsyncRaiser]
//        void OnStart(object sender, TouchSocketEventArgs e);

//        /// <summary>
//        /// 当服务器成功启动时
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        Task OnStartAsync(object sender, TouchSocketEventArgs e);

//        /// <summary>
//        /// 当服务器停止、释放时
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        [AsyncRaiser]
//        void OnStop(object sender, TouchSocketEventArgs e);

//        /// <summary>
//        /// 当服务器停止、释放时
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        Task OnStopAsync(object sender, TouchSocketEventArgs e);
//    }
//}