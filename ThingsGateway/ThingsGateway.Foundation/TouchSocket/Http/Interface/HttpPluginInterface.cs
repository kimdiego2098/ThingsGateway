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

namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// Http接口
    /// </summary>
    public interface IHttpPlugin : IPlugin
    {
        /// <summary>
        /// 在收到Delete时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnDelete(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 在收到Delete时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnDeleteAsync(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 在收到Get时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnGet(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 在收到Get时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnGetAsync(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 在收到Post时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnPost(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 在收到Post时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnPostAsync(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 在收到Put时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnPut(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 在收到Put时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnPutAsync(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 在收到其他Http请求时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnReceivedOtherHttpRequest(ITcpClientBase client, HttpContextEventArgs e);

        /// <summary>
        /// 在收到其他Http请求时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnReceivedOtherHttpRequestAsync(ITcpClientBase client, HttpContextEventArgs e);
    }
}