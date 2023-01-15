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
    /// MessageQueueClientExtensions
    /// </summary>
    public static class MessageQueueClientExtensions
    {
        /// <summary>
        /// 获取或设置MessageQueueClient的注入键。
        /// </summary>
        public static readonly DependencyProperty<MessageQueueClient> MessageQueueClientProperty =
            DependencyProperty<MessageQueueClient>.Register("MessageQueueClient", typeof(MessageQueueClientExtensions), null);

        /// <summary>
        /// 获取MessageQueueClient
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="client"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static MessageQueueClient GetMessageQueueClient<TClient>(this TClient client) where TClient : IDependencyTouchRpc, IDependencyObject
        {
            return client.GetValue(MessageQueueClientProperty);
        }
    }
}