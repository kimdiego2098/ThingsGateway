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
using System.ComponentModel;

namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// 请求头枚举
    /// </summary>
    public enum HttpHeaders : byte
    {
        /// <summary>
        /// Cache-Control 标头，指定请求/响应链上所有缓存控制机制必须服从的指令。
        /// </summary>
        [Description("cache-control")]
        CacheControl = 0,

        /// <summary>
        /// Connection 标头，指定特定连接需要的选项。
        /// </summary>
        [Description("connection")]
        Connection = 1,

        /// <summary>
        /// Date 标头，指定开始创建请求的日期和时间。
        /// </summary>
        [Description("date")]
        Date = 2,

        /// <summary>
        /// Keep-Alive 标头，指定用以维护持久性连接的参数。
        /// </summary>
        [Description("keep-alive")]
        KeepAlive = 3,

        /// <summary>
        /// Pragma 标头，指定可应用于请求/响应链上的任何代理的特定于实现的指令。
        /// </summary>
        [Description("pragma")]
        Pragma = 4,

        /// <summary>
        /// Trailer 标头，指定标头字段显示在以 chunked 传输编码方式编码的消息的尾部。
        /// </summary>
        [Description("trailer")]
        Trailer = 5,

        /// <summary>
        /// Transfer-Encoding 标头，指定对消息正文应用的转换的类型（如果有）。
        /// </summary>
        [Description("transfer-encoding")]
        TransferEncoding = 6,

        /// <summary>
        /// Upgrade 标头，指定客户端支持的附加通信协议。
        /// </summary>
        [Description("upgrade")]
        Upgrade = 7,

        /// <summary>
        /// Via 标头，指定网关和代理程序要使用的中间协议。
        /// </summary>
        [Description("via")]
        Via = 8,

        /// <summary>
        /// Warning 标头，指定关于可能未在消息中反映的消息的状态或转换的附加信息。
        /// </summary>
        [Description("warning")]
        Warning = 9,

        /// <summary>
        /// Allow 标头，指定支持的 HTTP 方法集。
        /// </summary>
        [Description("allow")]
        Allow = 10,

        /// <summary>
        /// Content-Length 标头，指定伴随正文数据的长度（以字节为单位）。
        /// </summary>
        [Description("content-length")]
        ContentLength = 11,

        /// <summary>
        /// Content-Type 标头，指定伴随正文数据的 MIME 类型。
        /// </summary>
        [Description("content-type")]
        ContentType = 12,

        /// <summary>
        /// Content-Encoding 标头，指定已应用于伴随正文数据的编码。
        /// </summary>
        [Description("content-encoding")]
        ContentEncoding = 13,

        /// <summary>
        /// Content-Langauge 标头，指定伴随正文数据的自然语言。
        /// </summary>
        [Description("content-langauge")]
        ContentLanguage = 14,

        /// <summary>
        /// Content-Location 标头，指定可从其中获得伴随正文的 URI。
        /// </summary>
        [Description("content-location")]
        ContentLocation = 15,

        /// <summary>
        /// Content-MD5 标头，指定伴随正文数据的 MD5 摘要，用于提供端到端消息完整性检查。
        /// </summary>
        [Description("content-md5")]
        ContentMd5 = 16,

        /// <summary>
        /// Content-Range 标头，指定在完整正文中应用伴随部分正文数据的位置。
        /// </summary>
        [Description("content-range")]
        ContentRange = 17,

        /// <summary>
        /// Expires 标头，指定日期和时间，在此之后伴随的正文数据应视为陈旧的。
        /// </summary>
        [Description("expires")]
        Expires = 18,

        /// <summary>
        /// Last-Modified 标头，指定上次修改伴随的正文数据的日期和时间。
        /// </summary>
        [Description("last-modified")]
        LastModified = 19,

        /// <summary>
        /// Accept 标头，指定响应可接受的 MIME 类型。
        /// </summary>
        [Description("accept")]
        Accept = 20,

        /// <summary>
        /// Accept-Charset 标头，指定响应可接受的字符集。
        /// </summary>
        [Description("accept-charset")]
        AcceptCharset = 21,

        /// <summary>
        /// Accept-Encoding 标头，指定响应可接受的内容编码。
        /// </summary>
        [Description("accept-encoding")]
        AcceptEncoding = 22,

        /// <summary>
        /// Accept-Langauge 标头，指定响应首选的自然语言。
        /// </summary>
        [Description("accept-langauge")]
        AcceptLanguage = 23,

        /// <summary>
        /// Authorization 标头，指定客户端为向服务器验证自身身份而出示的凭据。
        /// </summary>
        [Description("authorization")]
        Authorization = 24,

        /// <summary>
        /// Cookie 标头，指定向服务器提供的 Cookie 数据。
        /// </summary>
        [Description("cookie")]
        Cookie = 25,

        /// <summary>
        /// Expect 标头，指定客户端要求的特定服务器行为。
        /// </summary>
        [Description("expect")]
        Expect = 26,

        /// <summary>
        /// From 标头，指定控制请求用户代理的用户的 Internet 电子邮件地址。
        /// </summary>
        [Description("from")]
        From = 27,

        /// <summary>
        /// Host 标头，指定所请求资源的主机名和端口号。
        /// </summary>
        [Description("host")]
        Host = 28,

        /// <summary>
        /// If-Match 标头，指定仅当客户端的指示资源的缓存副本是最新的时，才执行请求的操作。
        /// </summary>
        [Description("if-match")]
        IfMatch = 29,

        /// <summary>
        /// If-Modified-Since 标头，指定仅当自指示的数据和时间之后修改了请求的资源时，才执行请求的操作。
        /// </summary>
        [Description("if-modified-since")]
        IfModifiedSince = 30,

        /// <summary>
        /// If-None-Match 标头，指定仅当客户端的指示资源的缓存副本都不是最新的时，才执行请求的操作。
        /// </summary>
        [Description("if-none-match")]
        IfNoneMatch = 31,

        /// <summary>
        /// If-Range 标头，指定如果客户端的缓存副本是最新的，仅发送指定范围的请求资源。
        /// </summary>
        [Description("if-range")]
        IfRange = 32,

        /// <summary>
        /// If-Unmodified-Since 标头，指定仅当自指示的日期和时间之后修改了请求的资源时，才执行请求的操作。
        /// </summary>
        [Description("if-unmodified-since")]
        IfUnmodifiedSince = 33,

        /// <summary>
        /// Max-Forwards 标头，指定一个整数，表示此请求还可转发的次数。
        /// </summary>
        [Description("max-forwards")]
        MaxForwards = 34,

        /// <summary>
        /// Proxy-Authorization 标头，指定客户端为向代理验证自身身份而出示的凭据。
        /// </summary>
        [Description("proxy-authorization")]
        ProxyAuthorization = 35,

        /// <summary>
        /// Referer 标头，指定从中获得请求 URI 的资源的 URI。
        /// </summary>
        [Description("referer")]
        Referer = 36,

        /// <summary>
        /// Range 标头，指定代替整个响应返回的客户端请求的响应的子范围。
        /// </summary>
        [Description("range")]
        Range = 37,

        /// <summary>
        /// TE 标头，指定响应可接受的传输编码方式。
        /// </summary>
        [Description("te")]
        Te = 38,

        /// <summary>
        /// Translate 标头，与 WebDAV 功能一起使用的 HTTP 规范的 Microsoft 扩展。
        /// </summary>
        [Description("translate")]
        Translate = 39,

        /// <summary>
        /// User-Agent 标头，指定有关客户端代理的信息。
        /// </summary>
        [Description("user-agent")]
        UserAgent = 40,

        /// <summary>
        /// Accept-Ranges 标头，指定服务器接受的范围。
        /// </summary>
        [Description("accept-ranges")]
        AcceptRanges = 41,

        /// <summary>
        /// Age 标头，指定自起始服务器生成响应以来的时间长度（以秒为单位）。
        /// </summary>
        [Description("age")]
        Age = 42,

        /// <summary>
        /// Etag 标头，指定请求的变量的当前值。
        /// </summary>
        [Description("etag")]
        ETag = 43,

        /// <summary>
        /// Location 标头，指定为获取请求的资源而将客户端重定向到的 URI。
        /// </summary>
        [Description("location")]
        Location = 44,

        /// <summary>
        /// Proxy-Authenticate 标头，指定客户端必须对代理验证其自身。
        /// </summary>
        [Description("proxy-authenticate")]
        ProxyAuthenticate = 45,

        /// <summary>
        /// Retry-After 标头，指定某个时间（以秒为单位）或日期和时间，在此时间之后客户端可以重试其请求。
        /// </summary>
        [Description("retry-after")]
        RetryAfter = 46,

        /// <summary>
        /// Server 标头，指定关于起始服务器代理的信息。
        /// </summary>
        [Description("server")]
        Server = 47,

        /// <summary>
        /// Set-Cookie 标头，指定提供给客户端的 Cookie 数据。
        /// </summary>
        [Description("set-cookie")]
        SetCookie = 48,

        /// <summary>
        /// Vary 标头，指定用于确定缓存的响应是否为新响应的请求标头。
        /// </summary>
        [Description("vary")]
        Vary = 49,

        /// <summary>
        /// WWW-Authenticate 标头，指定客户端必须对服务器验证其自身。
        /// </summary>
        [Description("www-authenticate")]
        WwwAuthenticate = 50,

        /// <summary>
        /// Origin。
        /// </summary>
        [Description("origin")]
        Origin = 51,

        /// <summary>
        /// Content-Disposition
        /// </summary>
        [Description("content-disposition")]
        ContentDisposition = 52
    }
}