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
using System.Security.Cryptography.X509Certificates;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 服务器Ssl设置
    /// </summary>
    public class ServiceSslOption : SslOption
    {
        private X509Certificate certificate;

        /// <summary>
        /// 证书
        /// </summary>
        public X509Certificate Certificate
        {
            get => certificate;
            set => certificate = value;
        }

        private bool clientCertificateRequired;

        /// <summary>
        /// 该值指定是否向客户端请求证书用于进行身份验证。 请注意，这只是一个请求 - 如果没有提供任何证书，服务器仍然可接受连接请求
        /// </summary>
        public bool ClientCertificateRequired
        {
            get => clientCertificateRequired;
            set => clientCertificateRequired = value;
        }
    }
}