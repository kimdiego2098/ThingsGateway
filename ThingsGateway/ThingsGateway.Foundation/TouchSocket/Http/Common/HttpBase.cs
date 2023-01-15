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
using System.Text;


using System.Text.RegularExpressions;

namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// Http基础头部
    /// </summary>
    public abstract class HttpBase : BlockReader, IRequestInfo
    {
        /// <summary>
        /// 服务器版本
        /// </summary>
        public static readonly string ServerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// 内容长度
        /// </summary>
        protected long m_contentLength;

        private static readonly byte[] m_rnrnCode = Encoding.UTF8.GetBytes("\r\n\r\n");

        private IgnoreCaseNameValueCollection m_headers;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpBase()
        {
            ReadTimeout = 1000 * 30;
        }

        /// <summary>
        /// 能否写入。
        /// </summary>
        public abstract bool CanWrite { get; }

        /// <summary>
        /// 客户端
        /// </summary>
        public abstract ITcpClientBase Client { get; }

        /// <summary>
        /// 内容填充完成
        /// </summary>
        public bool? ContentComplated { get; protected set; } = null;

        /// <summary>
        /// int类型，内容长度
        /// </summary>
        public int ContentLen
        {
            get => (int)m_contentLength;
            set => m_contentLength = value;
        }

        /// <summary>
        /// 内容长度
        /// </summary>
        public long ContentLength
        {
            get => m_contentLength;
            set => m_contentLength = value;
        }

        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 传递标识
        /// </summary>
        public object Flag { get; set; }

        /// <summary>
        /// 请求头集合
        /// </summary>
        public IgnoreCaseNameValueCollection Headers
        {
            get
            {
                m_headers ??= new IgnoreCaseNameValueCollection();
                return m_headers;
            }
        }

        /// <summary>
        /// 协议名称，默认HTTP
        /// </summary>
        public string Protocols { get; set; } = "HTTP";

        /// <summary>
        /// HTTP协议版本，默认1.1
        /// </summary>
        public string ProtocolVersion { get; set; } = "1.1";

        /// <summary>
        /// 请求行
        /// </summary>
        public string RequestLine { get; private set; }

        /// <summary>
        /// 获取头值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetHeader(string fieldName)
        {
            return GetHeaderByKey(fieldName);
        }

        /// <summary>
        /// 获取头集合的值
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public string GetHeader(HttpHeaders header)
        {
            var fieldName = header.GetDescription();
            if (fieldName == null) return null;
            return Headers.Get(fieldName);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="byteBlock"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool ParsingHeader(ByteBlock byteBlock, int length)
        {
            int index = byteBlock.Buffer.IndexOfFirst(byteBlock.Pos, length, m_rnrnCode);
            if (index > 0)
            {
                int headerLength = index - byteBlock.Pos;
                ReadHeaders(byteBlock.Buffer, byteBlock.Pos, headerLength);
                byteBlock.Pos += headerLength;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 从Request中持续读取数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return base.Read(buffer, offset, count);
        }

        /// <summary>
        /// 从内存中读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void ReadHeaders(byte[] buffer, int offset, int length)
        {
            string data = Encoding.UTF8.GetString(buffer, offset, length);
            string[] rows = Regex.Split(data, "\r\n");

            //Request URL & Method & Version
            RequestLine = rows[0];

            //Request Headers
            GetRequestHeaders(rows);
            long.TryParse(GetHeader(HttpHeaders.ContentLength), out m_contentLength);
            LoadHeaderProterties();
        }

        /// <summary>
        /// 设置一次性内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract void SetContent(byte[] content);

        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public HttpBase SetHeaderByKey(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(fieldName)) return this;
            Headers.Add(fieldName, value);
            return this;
        }

        /// <summary>
        /// 获取一次性内容。
        /// </summary>
        /// <returns></returns>
        public abstract bool TryGetContent(out byte[] content);

        /// <summary>
        /// 持续写入内容。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public abstract void WriteContent(byte[] buffer, int offset, int count);

        internal bool InternalInput(byte[] buffer, int offset, int length)
        {
            return Input(buffer, offset, length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!DisposedValue && CanRead)
            {
                TryGetContent(out _);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// 读取信息
        /// </summary>
        protected abstract void LoadHeaderProterties();

        private string GetHeaderByKey(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) return null;
            return Headers.Get(fieldName);
        }

        private void GetRequestHeaders(IEnumerable<string> rows)
        {
            if (rows == null || rows.Count() <= 0)
            {
                return;
            }

            foreach (var item in rows)
            {
                string[] kv = item.SplitFirst(':');
                if (kv.Length == 2)
                {
                    string key = kv[0].ToLower();
                    Headers.Add(key, kv[1]);
                }
            }

            ContentType = GetHeader(HttpHeaders.ContentType);
        }
    }
}