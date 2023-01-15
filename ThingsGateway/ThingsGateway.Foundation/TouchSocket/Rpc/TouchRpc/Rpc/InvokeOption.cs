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
    /// Rpc调用设置
    /// </summary>
    public struct InvokeOption : IInvokeOption
    {
        /// <summary>
        /// 默认设置。
        /// Timeout=5000ms
        /// </summary>
        public static InvokeOption OnlySend;

        /// <summary>
        /// 默认设置。
        /// Timeout=5000ms
        /// </summary>
        public static InvokeOption WaitInvoke;

        /// <summary>
        /// 默认设置。
        /// Timeout=5000 ms
        /// </summary>
        public static InvokeOption WaitSend;

        private int m_timeout;

        static InvokeOption()
        {
            OnlySend = new InvokeOption(timeout: 5000);
            OnlySend.FeedbackType = FeedbackType.OnlySend;

            WaitSend = new InvokeOption(timeout: 5000);
            WaitSend.FeedbackType = FeedbackType.WaitSend;

            WaitInvoke = new InvokeOption(timeout: 5000);
            WaitInvoke.FeedbackType = FeedbackType.WaitInvoke;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="feedbackType"></param>
        /// <param name="serializationType"></param>
        /// <param name="cancellationToken"></param>
        public InvokeOption(int timeout = 5000, FeedbackType feedbackType = FeedbackType.WaitInvoke, SerializationType serializationType = SerializationType.FastBinary,
            CancellationToken cancellationToken = default) : this()
        {
            Timeout = timeout;
            FeedbackType = feedbackType;
            SerializationType = serializationType;
            Token = cancellationToken;
        }

        /// <summary>
        /// 调用反馈
        /// </summary>
        public FeedbackType FeedbackType { get; set; }

        /// <summary>
        /// TouchRpc序列化类型
        /// </summary>
        public SerializationType SerializationType { get; set; }

        /// <summary>
        /// 调用超时，
        /// min=1000，默认5000 ms
        /// </summary>
        public int Timeout
        {
            get
            {
                if (m_timeout < 1000)
                {
                    return 5000;
                }
                return m_timeout;
            }
            set => m_timeout = value;
        }

        /// <summary>
        /// 可以取消的调用令箭
        /// </summary>
        public CancellationToken Token { get; set; }
    }
}