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

using System.Collections.Generic;


using System.Net;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// Udp数据处理适配器
    /// </summary>
    public abstract class UdpDataHandlingAdapter
    {
        internal IUdpSession m_owner;

        /// <summary>
        /// 是否允许发送<see cref="IRequestInfo"/>对象。
        /// </summary>
        public abstract bool CanSendRequestInfo { get; }

        /// <summary>
        /// 拼接发送
        /// </summary>
        public abstract bool CanSplicingSend { get; }

        /// <summary>
        /// 获取或设置适配器能接收的最大数据包长度。默认1024*1024 Byte。
        /// </summary>
        public int MaxPackageSize { get; set; } = 1024 * 1024;

        /// <summary>
        /// 适配器拥有者。
        /// </summary>
        public IUdpSession Owner => m_owner;

        /// <summary>
        /// 当接收数据处理完成后，回调该函数执行接收
        /// </summary>
        public Action<EndPoint, ByteBlock, IRequestInfo> ReceivedCallBack { get; set; }

        /// <summary>
        /// 当接收数据处理完成后，回调该函数执行发送
        /// </summary>
        public Action<EndPoint, byte[], int, int> SendCallBack { get; set; }

        /// <summary>
        /// 收到数据的切入点，该方法由框架自动调用。
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="byteBlock"></param>
        public void ReceivedInput(EndPoint remoteEndPoint, ByteBlock byteBlock)
        {
            try
            {
                PreviewReceived(remoteEndPoint, byteBlock);
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
            }
        }

        /// <summary>
        /// 发送数据的切入点，该方法由框架自动调用。
        /// </summary>
        /// <param name="requestInfo"></param>
        public void SendInput(IRequestInfo requestInfo)
        {
            PreviewSend(requestInfo);
        }

        /// <summary>
        /// 发送数据的切入点，该方法由框架自动调用。
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void SendInput(EndPoint endPoint, byte[] buffer, int offset, int length)
        {
            PreviewSend(endPoint, buffer, offset, length);
        }

        /// <summary>
        /// 发送数据的切入点，该方法由框架自动调用。
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="transferBytes"></param>
        public void SendInput(EndPoint endPoint, IList<ArraySegment<byte>> transferBytes)
        {
            PreviewSend(endPoint, transferBytes);
        }

        /// <summary>
        /// 处理已经经过预先处理后的数据
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="byteBlock">以二进制形式传递</param>
        /// <param name="requestInfo">以解析实例传递</param>
        protected void GoReceived(EndPoint remoteEndPoint, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            ReceivedCallBack.Invoke(remoteEndPoint, byteBlock, requestInfo);
        }

        /// <summary>
        /// 发送已经经过预先处理后的数据
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        protected void GoSend(EndPoint endPoint, byte[] buffer, int offset, int length)
        {
            SendCallBack.Invoke(endPoint, buffer, offset, length);
        }

        /// <summary>
        /// 在解析时发生错误。
        /// </summary>
        /// <param name="error">错误异常</param>
        /// <param name="reset">是否调用<see cref="Reset"/></param>
        /// <param name="log">是否记录日志</param>
        protected virtual void OnError(string error, bool reset = true, bool log = true)
        {
            if (reset)
            {
                Reset();
            }
            if (log && m_owner != null && m_owner.Logger != null)
            {
                m_owner.Logger.Error(error);
            }
        }

        /// <summary>
        /// 当接收到数据后预先处理数据,然后调用<see cref="GoReceived(EndPoint,ByteBlock, IRequestInfo)"/>处理数据
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="byteBlock"></param>
        protected abstract void PreviewReceived(EndPoint remoteEndPoint, ByteBlock byteBlock);

        /// <summary>
        /// 当发送数据前预先处理数据
        /// </summary>
        /// <param name="requestInfo"></param>
        protected abstract void PreviewSend(IRequestInfo requestInfo);

        /// <summary>
        /// 当发送数据前预先处理数据
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        protected abstract void PreviewSend(EndPoint endPoint, byte[] buffer, int offset, int length);

        /// <summary>
        /// 组合发送预处理数据，
        /// 当属性SplicingSend实现为True时，系统才会调用该方法。
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="transferBytes">代发送数据组合</param>
        protected abstract void PreviewSend(EndPoint endPoint, IList<ArraySegment<byte>> transferBytes);

        /// <summary>
        /// 重置解析器到初始状态，一般在<see cref="OnError(string, bool, bool)"/>被触发时，由返回值指示是否调用。
        /// </summary>
        protected abstract void Reset();
    }
}