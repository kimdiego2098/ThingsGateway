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

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// 路由类型
    /// </summary>
    public readonly struct RouteType
    {
        private readonly string value;

        /// <summary>
        /// 路由类型
        /// </summary>
        /// <param name="value"></param>
        public RouteType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"“{nameof(value)}”不能为 null 或空。", nameof(value));
            }

            this.value = value.ToLower().Trim();
        }

        /// <summary>
        /// 判断RouteType是相等。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(RouteType a, RouteType b)
        {
            return a.value == b.value;
        }

        /// <summary>
        /// 判断RouteType不相等。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(RouteType a, RouteType b)
        {
            return a.value != b.value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is RouteType type)
            {
                return this == type;
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => value;

        /// <summary>
        /// 一个Ping探测路由包
        /// </summary>
        public static readonly RouteType Ping = new RouteType("Ping");

        /// <summary>
        /// 创建通道路由包。
        /// </summary>
        public static readonly RouteType CreateChannel = new RouteType("CreateChannel");

        /// <summary>
        /// Rpc调用的路由包
        /// </summary>
        public static readonly RouteType Rpc = new RouteType("Rpc");

        /// <summary>
        /// 拉取文件的路由包
        /// </summary>
        public static readonly RouteType PullFile = new RouteType("PullFile");

        /// <summary>
        /// 加载远程流数据
        /// </summary>
        public static readonly RouteType LoadRemoteStream = new RouteType("LoadRemoteStream");

        /// <summary>
        /// 推送文件的路由包
        /// </summary>
        public static readonly RouteType PushFile = new RouteType("PushFile");
    }
}