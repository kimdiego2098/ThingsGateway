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
    /// 代理身份认证
    /// </summary>
    public class NetworkCredential
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="domain">基本认证应该不需要这个</param>
        /// <param name="roles"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public NetworkCredential(string username, string password, string domain, params string[] roles)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            if (username.Length == 0)
                throw new ArgumentException("An empty string.", "username");
            Username = username;
            Password = password;
            Domain = domain;
            Roles = roles;
        }

        /// <summary>
        /// 凭证用户名
        /// </summary>

        public string Username { get; }

        /// <summary>
        /// 凭证密码
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Domain
        /// </summary>
        public string Domain { get; }

        /// <summary>
        /// Roles
        /// </summary>
        public string[] Roles { get; }
    }
}