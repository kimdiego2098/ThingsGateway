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
    /// MessageQueueClient
    /// </summary>
    public abstract class MessageQueueClient
    {
        /// <summary>
        /// 序列化转换器。
        /// </summary>
        public BytesConverter Converter { get; set; }

        ///// <summary>
        ///// 队列
        ///// </summary>
        ///// <param name="queue"></param>
        ///// <param name="buffer"></param>
        ///// <param name="offset"></param>
        ///// <param name="length"></param>
        //public abstract void Publish(string queue, byte[] buffer,int offset,int length);

        //public abstract void Subscribe<T>(string queue,Action<> data);
    }

    internal class InternalMessageQueueClient : MessageQueueClient
    {
        private RpcActor m_rpcActor;

        public InternalMessageQueueClient(RpcActor rpcActor, BytesConverter converter)
        {
            m_rpcActor = rpcActor;
            Converter = converter;
        }
    }
}