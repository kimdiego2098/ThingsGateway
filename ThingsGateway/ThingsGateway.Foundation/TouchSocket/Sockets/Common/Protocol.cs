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

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 协议类
    /// </summary>
    public struct Protocol
    {
        /// <summary>
        /// 值
        /// </summary>
        private readonly string value;

        /// <summary>
        /// 表示无协议
        /// </summary>
        public static readonly Protocol None = new Protocol();

        /// <summary>
        /// 获取http协议
        /// </summary>
        public static readonly Protocol Http = new Protocol("http");

        /// <summary>
        /// TCP协议
        /// </summary>
        public static readonly Protocol TCP = new Protocol("tcp");

        /// <summary>
        /// UDP协议
        /// </summary>
        public static readonly Protocol UDP = new Protocol("udp");

        /// <summary>
        /// 获取WebSocket协议
        /// </summary>
        public static readonly Protocol WebSocket = new Protocol("ws");

        /// <summary>
        /// 表示
        /// </summary>
        /// <param name="value">值</param>
        public Protocol(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException();
            }
            this.value = value;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(value))
            {
                return "None";
            }
            return value;
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (value == null)
            {
                return string.Empty.GetHashCode();
            }
            return value.ToLower().GetHashCode();
        }

        /// <summary>
        /// 比较是否和目标相等
        /// </summary>
        /// <param name="obj">目标</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Protocol)
            {
                return GetHashCode() == obj.GetHashCode();
            }
            return false;
        }

        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Protocol a, Protocol b)
        {
            if (string.IsNullOrEmpty(a.value) && string.IsNullOrEmpty(b.value))
            {
                return true;
            }
            return string.Equals(a.value, b.value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 不等于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Protocol a, Protocol b)
        {
            var state = a == b;
            return !state;
        }
    }
}