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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;


using System.Text.RegularExpressions;

namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// HTTP请求定义
    /// </summary>
    public class HttpRequest : HttpBase
    {
        private bool m_canRead;
        private ITcpClientBase m_client;
        private byte[] m_content;
        private NameValueCollection m_forms;
        private NameValueCollection m_params;
        private NameValueCollection m_query;
        private string m_relativeURL;
        private bool m_sentHeader;
        private int m_sentLength;
        private string m_uRL;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="client"></param>
        /// <param name="isServer"></param>
        public HttpRequest(ITcpClientBase client, bool isServer = false)
        {
            m_client = client;
            if (isServer)
            {
                m_canRead = true;
                CanWrite = false;
            }
            else
            {
                m_canRead = false;
                CanWrite = true;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpRequest()
        {
            m_canRead = false;
            CanWrite = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanRead => m_canRead;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanWrite { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override ITcpClientBase Client => m_client;

        /// <summary>
        /// 表单数据
        /// </summary>
        public NameValueCollection Forms
        {
            get
            {
                if (ContentType == @"application/x-www-form-urlencoded")
                {
                    m_forms ??= GetParameters(this.GetBody());
                    return m_forms;
                }

                return m_forms ??= new NameValueCollection();
            }
        }

        /// <summary>
        /// 获取时候保持连接
        /// </summary>
        public bool KeepAlive
        {
            get
            {
                if (ProtocolVersion == "1.0")
                {
                    return false;
                }
                else
                {
                    if (GetHeader(HttpHeaders.Connection) == "keep-alive")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// HTTP请求方式。
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Body参数
        /// </summary>
        public NameValueCollection Params
        {
            get
            {
                m_params ??= new NameValueCollection();
                return m_params;
            }
        }

        /// <summary>
        /// url参数
        /// </summary>
        public NameValueCollection Query
        {
            get
            {
                m_query ??= new NameValueCollection();
                return m_query;
            }
        }

        /// <summary>
        /// 相对路径（不含参数）
        /// </summary>
        public string RelativeURL => m_relativeURL;

        /// <summary>
        /// Url全地址，包含参数
        /// </summary>
        public string URL => m_uRL;

        /// <summary>
        ///  构建响应数据。
        /// <para>当数据较大时，不建议这样操作，可直接<see cref="WriteContent(byte[], int, int)"/></para>
        /// </summary>
        /// <param name="byteBlock"></param>
        public void Build(ByteBlock byteBlock)
        {
            BuildHeader(byteBlock);
            BuildContent(byteBlock);
        }

        /// <summary>
        /// 构建数据为字节数组。
        /// </summary>
        /// <returns></returns>
        public byte[] BuildAsBytes()
        {
            using ByteBlock byteBlock = new ByteBlock();
            Build(byteBlock);
            return byteBlock.ToArray();
        }

        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="content"></param>
        public override void SetContent(byte[] content)
        {
            m_content = content;
            ContentLength = content.Length;
            ContentComplated = true;
        }

        /// <summary>
        /// 设置Url，必须以“/”开头，可带参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="justValue"></param>
        /// <returns></returns>
        public HttpRequest SetUrl(string url, bool justValue = false)
        {
            if (justValue || url.StartsWith("/"))
            {
                m_uRL = url;
            }
            else
            {
                m_uRL = "/" + url;
            }
            ParseUrl();
            return this;
        }

        /// <summary>
        /// 输出
        /// </summary>
        public override string ToString()
        {
            using (ByteBlock byteBlock = new ByteBlock())
            {
                Build(byteBlock);
                return byteBlock.ToString();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override bool TryGetContent(out byte[] content)
        {
            if (!ContentComplated.HasValue)
            {
                if (m_contentLength == 0)
                {
                    m_content = new byte[0];
                    content = m_content;
                    ContentComplated = true;
                    return true;
                }
                try
                {
                    using MemoryStream block1 = new MemoryStream();
                    using ByteBlock block2 = new ByteBlock();
                    byte[] buffer = block2.Buffer;
                    while (true)
                    {
                        int r = Read(buffer, 0, buffer.Length);
                        if (r == 0)
                        {
                            break;
                        }
                        block1.Write(buffer, 0, r);
                    }
                    ContentComplated = true;
                    m_content = block1.ToArray();
                    content = m_content;
                    return true;
                }
                catch
                {
                    ContentComplated = false;
                    content = null;
                    return false;
                }
                finally
                {
                    m_canRead = false;
                }
            }
            else if (ContentComplated == true)
            {
                content = m_content;
                return true;
            }
            else
            {
                content = null;
                return false;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void WriteContent(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
            {
                throw new NotSupportedException("该对象不支持持续写入内容。");
            }
            if (!m_sentHeader)
            {
                using (ByteBlock byteBlock = new ByteBlock())
                {
                    BuildHeader(byteBlock);
                    m_client.DefaultSend(byteBlock);
                }
                m_sentHeader = true;
            }
            if (m_sentLength + count <= m_contentLength)
            {
                m_client.DefaultSend(buffer, offset, count);
                m_sentLength += count;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            m_client = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// 从内存中读取
        /// </summary>
        protected override void LoadHeaderProterties()
        {
            var first = Regex.Split(RequestLine, @"(\s+)").Where(e => e.Trim() != string.Empty).ToArray();
            if (first.Length > 0) Method = first[0].Trim().ToUpper();
            if (first.Length > 1)
            {
                SetUrl(Uri.UnescapeDataString(first[1]));
            }
            if (first.Length > 2)
            {
                string[] ps = first[2].Split('/');
                if (ps.Length == 2)
                {
                    Protocols = ps[0];
                    ProtocolVersion = ps[1];
                }
            }
        }

        private void BuildContent(ByteBlock byteBlock)
        {
            if (ContentLength > 0)
            {
                byteBlock.Write(m_content);
            }
        }

        /// <summary>
        /// 构建响应头部
        /// </summary>
        /// <returns></returns>
        private void BuildHeader(ByteBlock byteBlock)
        {
            StringBuilder stringBuilder = new StringBuilder();

            string url = null;
            if (!string.IsNullOrEmpty(m_relativeURL))
            {
                if (m_query == null)
                {
                    url = m_relativeURL;
                }
                else
                {
                    StringBuilder urlBuilder = new StringBuilder();
                    urlBuilder.Append(m_relativeURL);
                    urlBuilder.Append("?");
                    int i = 0;
                    foreach (var item in m_query.AllKeys)
                    {
                        urlBuilder.Append($"{item}={m_query[item]}");
                        if (++i < m_query.Count)
                        {
                            urlBuilder.Append("&");
                        }
                    }
                    url = urlBuilder.ToString();
                }
            }

            if (string.IsNullOrEmpty(url))
            {
                stringBuilder.Append($"{Method} / HTTP/{ProtocolVersion}\r\n");
            }
            else
            {
                stringBuilder.Append($"{Method} {url} HTTP/{ProtocolVersion}\r\n");
            }
            if (ContentLength > 0)
            {
                this.SetHeader(HttpHeaders.ContentLength, ContentLength.ToString());
            }
            foreach (var headerkey in Headers.AllKeys)
            {
                stringBuilder.Append($"{headerkey}: ");
                stringBuilder.Append(Headers[headerkey] + "\r\n");
            }

            stringBuilder.Append("\r\n");
            byteBlock.Write(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
        }

        private NameValueCollection GetParameters(string row)
        {
            if (string.IsNullOrEmpty(row))
            {
                return null;
            }
            string[] kvs = row.Split('&');
            if (kvs == null || kvs.Count() == 0)
            {
                return null;
            }

            NameValueCollection pairs = new NameValueCollection();
            foreach (var item in kvs)
            {
                string[] kv = item.SplitFirst('=');
                if (kv.Length == 2)
                {
                    pairs.Add(kv[0], kv[1]);
                }
            }

            return pairs;
        }

        private void ParseUrl()
        {
            if (m_uRL.Contains("?"))
            {
                string[] urls = m_uRL.Split('?');
                if (urls.Length > 0)
                {
                    m_relativeURL = urls[0];
                }
                if (urls.Length > 1)
                {
                    m_query = GetParameters(urls[1]);
                }
            }
            else
            {
                m_relativeURL = m_uRL;
            }
        }
    }
}