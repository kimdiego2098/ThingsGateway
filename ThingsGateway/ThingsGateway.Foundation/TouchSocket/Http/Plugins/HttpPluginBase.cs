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
    /// Http扩展基类
    /// </summary>
    public class HttpPluginBase : HttpPluginBase<ITcpClientBase>
    {
    }

    /// <summary>
    /// Http扩展基类
    /// </summary>
    public abstract class HttpPluginBase<TClient> : TcpPluginBase<TClient>, IHttpPlugin
    {
        void IHttpPlugin.OnDelete(ITcpClientBase client, HttpContextEventArgs e)
        {
            OnDelete((TClient)client, e);
        }

        Task IHttpPlugin.OnDeleteAsync(ITcpClientBase client, HttpContextEventArgs e)
        {
            return OnDeleteAsync((TClient)client, e);
        }

        void IHttpPlugin.OnGet(ITcpClientBase client, HttpContextEventArgs e)
        {
            OnGet((TClient)client, e);
        }

        Task IHttpPlugin.OnGetAsync(ITcpClientBase client, HttpContextEventArgs e)
        {
            return OnGetAsync((TClient)client, e);
        }

        void IHttpPlugin.OnPost(ITcpClientBase client, HttpContextEventArgs e)
        {
            OnPost((TClient)client, e);
        }

        Task IHttpPlugin.OnPostAsync(ITcpClientBase client, HttpContextEventArgs e)
        {
            return OnPostAsync((TClient)client, e);
        }

        void IHttpPlugin.OnPut(ITcpClientBase client, HttpContextEventArgs e)
        {
            OnPut((TClient)client, e);
        }

        Task IHttpPlugin.OnPutAsync(ITcpClientBase client, HttpContextEventArgs e)
        {
            return OnPutAsync((TClient)client, e);
        }

        void IHttpPlugin.OnReceivedOtherHttpRequest(ITcpClientBase client, HttpContextEventArgs e)
        {
            OnReceivedOtherHttpRequest((TClient)client, e);
        }

        Task IHttpPlugin.OnReceivedOtherHttpRequestAsync(ITcpClientBase client, HttpContextEventArgs e)
        {
            return OnReceivedOtherHttpRequestAsync((TClient)client, e);
        }

        #region 虚函数

        /// <summary>
        /// <inheritdoc cref="IHttpPlugin.OnDelete(ITcpClientBase, HttpContextEventArgs)"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnDelete(TClient client, HttpContextEventArgs e)
        {
        }

        /// <summary>
        /// 在收到Delete时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnDeleteAsync(TClient client, HttpContextEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// <inheritdoc cref="IHttpPlugin.OnGet(ITcpClientBase, HttpContextEventArgs)"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnGet(TClient client, HttpContextEventArgs e)
        {
        }

        /// <summary>
        /// 在收到Get时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnGetAsync(TClient client, HttpContextEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// <inheritdoc cref="IHttpPlugin.OnPost(ITcpClientBase, HttpContextEventArgs)"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnPost(TClient client, HttpContextEventArgs e)
        {
        }

        /// <summary>
        /// 在收到Post时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnPostAsync(TClient client, HttpContextEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// <inheritdoc cref="IHttpPlugin.OnPut(ITcpClientBase, HttpContextEventArgs)"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnPut(TClient client, HttpContextEventArgs e)
        {
        }

        /// <summary>
        ///  在收到Put时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnPutAsync(TClient client, HttpContextEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// <inheritdoc cref="IHttpPlugin.OnReceivedOtherHttpRequest(ITcpClientBase, HttpContextEventArgs)"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnReceivedOtherHttpRequest(TClient client, HttpContextEventArgs e)
        {
        }

        /// <summary>
        /// 在收到其他Http请求时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnReceivedOtherHttpRequestAsync(TClient client, HttpContextEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        #endregion 虚函数
    }
}