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



namespace ThingsGateway.Foundation.Http.WebSockets
{
    /// <summary>
    /// WebSocket数据帧
    /// </summary>
    public class WSDataFrame : IDisposable, IRequestInfo
    {
        private bool m_disposedValue;

        /// <summary>
        /// 是否为最后数据帧。
        /// </summary>
        public bool FIN { get; set; }

        /// <summary>
        /// 标识RSV-1。
        /// </summary>
        public bool RSV1 { get; set; }

        /// <summary>
        /// 标识RSV-2。
        /// </summary>
        public bool RSV2 { get; set; }

        /// <summary>
        /// 标识RSV-3。
        /// </summary>
        public bool RSV3 { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public WSDataType Opcode { get; set; }

        /// <summary>
        /// 计算掩码
        /// </summary>
        public bool Mask { get; set; }

        /// <summary>
        /// 有效载荷数据长度
        /// </summary>
        public int PayloadLength { get; set; }

        /// <summary>
        /// 掩码值
        /// </summary>
        public byte[] MaskingKey { get; set; }

        /// <summary>
        /// 有效数据
        /// </summary>
        public ByteBlock PayloadData { get; set; }

        /// <summary>
        /// 构建数据
        /// </summary>
        /// <param name="byteBlock"></param>
        /// <param name="masked"></param>
        /// <returns></returns>
        public bool Build(ByteBlock byteBlock, bool masked)
        {
            if (PayloadData != null)
            {
                PayloadLength = PayloadData.Len;
            }
            else
            {
                PayloadLength = 0;
            }

            if (PayloadData == null)
            {
                return WSTools.Build(byteBlock, this, new byte[0], 0, 0);
            }
            return WSTools.Build(byteBlock, this, PayloadData.Buffer, 0, PayloadLength);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                PayloadData?.Dispose();
                m_disposedValue = true;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~WSDataFrame()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}