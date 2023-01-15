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
    /// 具有发送动作的基类。
    /// </summary>
    public interface ISenderBase
    {
        /// <summary>
        /// 表示对象能否顺利执行发送操作。
        /// <para>由于高并发，当该值为True时，也不一定完全能执行。</para>
        /// </summary>
        bool CanSend { get; }
    }
}