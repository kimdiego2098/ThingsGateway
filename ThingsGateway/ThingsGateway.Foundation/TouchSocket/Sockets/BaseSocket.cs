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
    /// 通讯基类
    /// </summary>
    public abstract class BaseSocket : DependencyObject, ISocket
    {
        /// <summary>
        /// 通讯基类
        /// </summary>
        public BaseSocket()
        {
            SyncRoot = new object();
        }

        private int m_bufferLength;

        /// <summary>
        /// 数据交互缓存池限制，min=1024 byte
        /// </summary>
        public virtual int BufferLength
        {
            get => m_bufferLength;
            set
            {
                if (value < 1024)
                {
                    value = 1024 * 10;
                }
                m_bufferLength = value;
            }
        }

        /// <summary>
        /// 同步根。
        /// </summary>
        protected object SyncRoot;

        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILog Logger { get; set; }
    }
}