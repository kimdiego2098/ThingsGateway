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
    /// 终端接口
    /// </summary>
    public interface IClient : IDependencyObject, IDisposable
    {
        /// <summary>
        /// 处理未经过适配器的数据。返回值表示是否继续向下传递。
        /// </summary>
        Func<ByteBlock, bool> OnHandleRawBuffer { get; set; }

        /// <summary>
        /// 处理经过适配器后的数据。返回值表示是否继续向下传递。
        /// </summary>
        Func<ByteBlock, IRequestInfo, bool> OnHandleReceivedData { get; set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        ILog Logger { get; }

        /// <summary>
        /// 终端协议
        /// </summary>
        Protocol Protocol { get; set; }

        /// <summary>
        /// 简单IOC容器
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        /// 最后一次接收到数据的时间
        /// </summary>
        DateTime LastReceivedTime { get; }

        /// <summary>
        /// 最后一次发送数据的时间
        /// </summary>
        DateTime LastSendTime { get; }
    }
}