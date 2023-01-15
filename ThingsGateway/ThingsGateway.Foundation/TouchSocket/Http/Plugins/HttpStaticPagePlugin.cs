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

namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// Http静态内容插件
    /// </summary>
    public class HttpStaticPagePlugin : HttpPluginBase
    {
        private readonly FileCachePool fileCache;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpStaticPagePlugin()
        {
            fileCache = new FileCachePool();
        }

        /// <summary>
        /// 静态文件缓存。
        /// </summary>
        public FileCachePool FileCache => fileCache;

        /// <summary>
        /// 添加静态
        /// </summary>
        /// <param name="path">Static content path</param>
        /// <param name="prefix">Cache prefix (default is "/")</param>
        /// <param name="filter">Cache filter (default is "*.*")</param>
        /// <param name="timeout">Refresh cache timeout (default is 1 hour)</param>
        public void AddFolder(string path, string prefix = "/", string filter = "*.*", TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromHours(1);

            fileCache.InsertPath(path, prefix, filter, timeout.Value, null);
        }

        /// <summary>
        /// Clear static content cache
        /// </summary>
        public void ClearFolder()
        {
            fileCache.Clear();
        }

        /// <summary>
        /// Remove static content cache
        /// </summary>
        /// <param name="path">Static content path</param>
        public void RemoveFolder(string path)
        {
            fileCache.RemovePath(path);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected override void OnGet(ITcpClientBase client, HttpContextEventArgs e)
        {
            if (fileCache.Find(e.Context.Request.RelativeURL, out byte[] data))
            {
                e.Context.Response
                    .SetStatus()
                    .SetContentTypeByExtension(Path.GetExtension(e.Context.Request.RelativeURL))
                    .SetContentLength(data.Length)
                    .WriteContent(data);
                e.Handled = true;
            }
            base.OnGet(client, e);
        }
    }
}