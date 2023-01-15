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


namespace ThingsGateway.Foundation
{
    /// <summary>
    /// RpcExtensions
    /// </summary>
    public static class RpcConfigExtensions
    {
        /// <summary>
        /// 指定RpcStore的创建。
        /// </summary>
        public static readonly DependencyProperty<RpcStore> RpcStoreProperty =
            DependencyProperty<RpcStore>.Register("RpcStore", typeof(RpcConfigExtensions), null);

        /// <summary>
        /// 配置RpcStore的创建。
        /// </summary>
        /// <param name="config"></param>
        /// <param name="action">当RpcStore完成配置时回调</param>
        /// <param name="value">可以使用现有的值，如果赋值为null，则会重新创建。</param>
        public static TouchSocketConfig ConfigureRpcStore(this TouchSocketConfig config, Action<RpcStore> action, RpcStore value = default)
        {
            if (value == default)
            {
                value = new RpcStore(config.Container);
            }
            action?.Invoke(value);
            config.SetValue(RpcStoreProperty, value);
            return config;
        }
    }
}