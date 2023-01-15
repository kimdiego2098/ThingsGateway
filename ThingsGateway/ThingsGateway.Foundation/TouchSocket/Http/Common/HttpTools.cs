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
    /// Http工具
    /// </summary>
    public static class HttpTools
    {
        /// <summary>
        /// 从扩展名获取ContentType
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetContentTypeFromExtension(string extension)
        {
            switch (extension.ToLower())
            {
                case ".html":
                    return "text/html";

                case ".css":
                    return "text/css";

                case ".js":
                    return "text/javascript";

                case ".xml":
                    return "text/xml";

                case ".gzip":
                    return "application/gzip";

                case ".json":
                    return "application/json";

                case ".map":
                    return "application/json";

                case ".pdf":
                    return "application/pdf";

                case ".zip":
                    return "application/zip";

                case ".mp3":
                    return "audio/mpeg";

                case ".jpg":
                    return "image/jpeg";

                case ".gif":
                    return "image/gif";

                case ".png":
                    return "image/png";

                case ".svg":
                    return "image/svg+xml";

                case ".mp4":
                    return "video/mp4";

                case ".atom":
                    return "application/atom+xml";

                case ".fastsoap":
                    return "application/fastsoap";

                case ".ps":
                    return "application/postscript";

                case ".soap":
                    return "application/soap+xml";

                case ".sql":
                    return "application/sql";

                case ".xslt":
                    return "application/xslt+xml";

                case ".zlib":
                    return "application/zlib";

                case ".aac":
                    return "audio/aac";

                case ".ac3":
                    return "audio/ac3";

                case ".ogg":
                    return "audio/ogg";

                case ".ttf":
                    return "font/ttf";

                case ".bmp":
                    return "image/bmp";

                case ".jpm":
                    return "image/jpm";

                case ".jpx":
                    return "image/jpx";

                case ".jrx":
                    return "image/jrx";

                case ".tiff":
                    return "image/tiff";

                case ".emf":
                    return "image/emf";

                case ".wmf":
                    return "image/wmf";

                case ".http":
                    return "message/http";

                case ".s-http":
                    return "message/s-http";

                case ".mesh":
                    return "model/mesh";

                case ".vrml":
                    return "model/vrml";

                case ".csv":
                    return "text/csv";

                case ".plain":
                    return "text/plain";

                case ".richtext":
                    return "text/richtext";

                case ".rtf":
                    return "text/rtf";

                case ".rtx":
                    return "text/rtx";

                case ".sgml":
                    return "text/sgml";

                case ".strings":
                    return "text/strings";

                case ".url":
                    return "text/uri-list";

                case ".H264":
                    return "video/H264";

                case ".H265":
                    return "video/H265";

                case ".mpeg":
                    return "video/mpeg";

                case ".raw":
                    return "video/raw";

                default:
                    return "application/octet-stream";
            }
        }
    }
}