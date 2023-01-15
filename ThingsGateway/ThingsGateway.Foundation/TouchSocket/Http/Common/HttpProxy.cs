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
    /// Http代理
    /// </summary>
    public class HttpProxy
    {
        /// <summary>
        /// 不带基本验证的代理
        /// </summary>
        /// <param name="host"></param>
        public HttpProxy(IPHost host)
        {
            Host = host;
        }

        /// <summary>
        /// 带基本验证的代理
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        public HttpProxy(IPHost host, string userName, string passWord)
        {
            Host = host;
            Credential = new NetworkCredential(userName, passWord, $"{host.IP}:{host.Port}");
        }

        /// <summary>
        /// 验证代理
        /// </summary>
        public NetworkCredential Credential { get; set; }

        /// <summary>
        /// 代理的地址
        /// </summary>
        public IPHost Host { get; set; }
    }
}