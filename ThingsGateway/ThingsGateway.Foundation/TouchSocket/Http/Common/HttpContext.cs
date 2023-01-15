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

namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// Http上下文
    /// </summary>
    public class HttpContext
    {
        private HttpResponse m_response;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request"></param>
        public HttpContext(HttpRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public HttpContext(HttpRequest request, HttpResponse response)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            m_response = response ?? throw new ArgumentNullException(nameof(response));
        }

        /// <summary>
        /// Http请求
        /// </summary>
        public HttpRequest Request { get; }

        /// <summary>
        /// Http响应
        /// </summary>
        public HttpResponse Response
        {
            get
            {
                lock (this)
                {
                    if (m_response == null)
                    {
                        m_response = new HttpResponse(Request.Client, true);
                    }
                    return m_response;
                }
            }
        }
    }
}