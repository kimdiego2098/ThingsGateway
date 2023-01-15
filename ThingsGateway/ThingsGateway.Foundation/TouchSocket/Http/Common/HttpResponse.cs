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
using System.Linq;
using System.Text;


using System.Text.RegularExpressions;

namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// Http响应
    /// </summary>
    public class HttpResponse : HttpBase
    {
        private bool m_canRead;
        private bool m_canWrite;
        private ITcpClientBase m_client;
        private byte[] m_content;
        private bool m_responsed;
        private bool m_sentHeader;
        private long m_sentLength;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="client"></param>
        /// <param name="isServer"></param>
        public HttpResponse(ITcpClientBase client, bool isServer = true)
        {
            m_client = client;
            if (isServer)
            {
                m_canRead = false;
                m_canWrite = true;
            }
            else
            {
                m_canRead = true;
                m_canWrite = false;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpResponse()
        {
            m_canRead = false;
            m_canWrite = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanRead => m_canRead;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanWrite => m_canWrite;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override ITcpClientBase Client => m_client;

        /// <summary>
        /// 关闭会话请求
        /// </summary>
        public bool CloseConnection
        {
            get
            {
                return GetHeader(HttpHeaders.Connection).Equals("close", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        /// <summary>
        /// 是否分块
        /// </summary>
        public bool IsChunk { get; set; }

        /// <summary>
        /// 是否代理权限验证。
        /// </summary>
        public bool IsProxyAuthenticationRequired
        {
            get
            {
                return StatusCode == "407";
            }
        }

        /// <summary>
        /// 是否重定向
        /// </summary>
        public bool IsRedirect
        {
            get
            {
                return StatusCode == "301" || StatusCode == "302";
            }
        }

        /// <summary>
        /// 是否已经响应数据。
        /// </summary>
        public bool Responsed => m_responsed;

        /// <summary>
        /// 状态码，默认200
        /// </summary>
        public string StatusCode { get; set; } = "200";

        /// <summary>
        /// 状态消息，默认Success
        /// </summary>
        public string StatusMessage { get; set; } = "Success";

        /// <summary>
        /// 构建数据并回应。
        /// <para>该方法仅在具有Client实例时有效。</para>
        /// </summary>
        public void Answer()
        {
            if (m_responsed)
            {
                return;
            }
            using (ByteBlock byteBlock = new ByteBlock())
            {
                Build(byteBlock);
                if (m_client.CanSend)
                {
                    m_client.DefaultSend(byteBlock);
                }
                m_responsed = true;
            }
        }

        /// <summary>
        ///  构建响应数据。
        /// <para>当数据较大时，不建议这样操作，可直接<see cref="WriteContent(byte[], int, int)"/></para>
        /// </summary>
        /// <param name="byteBlock"></param>
        /// <param name="responsed"></param>
        public void Build(ByteBlock byteBlock, bool responsed = true)
        {
            if (m_responsed)
            {
                throw new Exception("该对象已被响应。");
            }

            BuildHeader(byteBlock);
            BuildContent(byteBlock);
            m_responsed = responsed;
        }

        /// <summary>
        /// 构建数据为字节数组。
        /// </summary>
        /// <returns></returns>
        public byte[] BuildAsBytes()
        {
            using (ByteBlock byteBlock = new ByteBlock())
            {
                Build(byteBlock);
                return byteBlock.ToArray();
            }
        }

        /// <summary>
        /// 当传输模式是Chunk时，用于结束传输。
        /// </summary>
        public void Complete()
        {
            m_canWrite = false;
            if (IsChunk)
            {
                using (ByteBlock byteBlock = new ByteBlock())
                {
                    byteBlock.Write(Encoding.UTF8.GetBytes($"{0:X}\r\n"));
                    byteBlock.Write(Encoding.UTF8.GetBytes("\r\n"));
                    m_client.DefaultSend(byteBlock);
                    m_responsed = true;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="content"></param>
        public override void SetContent(byte[] content)
        {
            m_content = content;
            ContentLength = content.Length;
            ContentComplated = true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override bool TryGetContent(out byte[] content)
        {
            if (!ContentComplated.HasValue)
            {
                if (!IsChunk && m_contentLength == 0)
                {
                    m_content = new byte[0];
                    content = m_content;
                    return true;
                }

                try
                {
                    using (ByteBlock block1 = new ByteBlock(1024 * 1024))
                    {
                        using (ByteBlock block2 = new ByteBlock())
                        {
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
                    }
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
            if (m_responsed)
            {
                throw new Exception("该对象已被响应。");
            }

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
            if (IsChunk)
            {
                using (ByteBlock byteBlock = new ByteBlock(count + 1024))
                {
                    byteBlock.Write(Encoding.UTF8.GetBytes($"{count.ToString("X")}\r\n"));
                    byteBlock.Write(buffer, offset, count);
                    byteBlock.Write(Encoding.UTF8.GetBytes("\r\n"));
                    m_client.DefaultSend(byteBlock);
                    m_sentLength += count;
                }
            }
            else
            {
                if (m_sentLength + count <= m_contentLength)
                {
                    m_client.DefaultSend(buffer, offset, count);
                    m_sentLength += count;
                    if (m_sentLength == ContentLength)
                    {
                        m_canWrite = false;
                        m_responsed = true;
                    }
                }
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
        /// 读取数据
        /// </summary>
        protected override void LoadHeaderProterties()
        {
            var first = Regex.Split(RequestLine, @"(\s+)").Where(e => e.Trim() != string.Empty).ToArray();
            if (first.Length > 0)
            {
                string[] ps = first[0].Split('/');
                if (ps.Length == 2)
                {
                    Protocols = ps[0];
                    ProtocolVersion = ps[1];
                }
            }
            if (first.Length > 1)
            {
                StatusCode = first[1];
            }
            string msg = string.Empty;
            for (int i = 2; i < first.Length; i++)
            {
                msg += first[i] + " ";
            }
            StatusMessage = msg;

            string transferEncoding = GetHeader(HttpHeaders.TransferEncoding);
            if ("chunked".Equals(transferEncoding, StringComparison.OrdinalIgnoreCase))
            {
                IsChunk = true;
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
            stringBuilder.Append($"HTTP/{ProtocolVersion} {StatusCode} {StatusMessage}\r\n");

            if (ContentLength > 0)
            {
                this.SetHeader(HttpHeaders.ContentLength, ContentLength.ToString());
            }
            if (IsChunk)
            {
                this.SetHeader(HttpHeaders.TransferEncoding, "chunked");
            }
            foreach (var headerkey in Headers.AllKeys)
            {
                stringBuilder.Append($"{headerkey}: ");
                stringBuilder.Append(Headers[headerkey] + "\r\n");
            }

            stringBuilder.Append("\r\n");
            byteBlock.Write(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
        }
    }
}