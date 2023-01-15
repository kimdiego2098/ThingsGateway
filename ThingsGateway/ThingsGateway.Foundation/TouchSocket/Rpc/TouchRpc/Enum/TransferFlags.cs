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
    /// 传输标识
    /// </summary>
    [Flags]
    public enum TransferFlags : byte
    {
        /// <summary>
        /// 无任何标识
        /// </summary>
        None = 0,

        /// <summary>
        /// 断点续传。
        /// <para>使用该标识时，会使用文件长度验证续传的有效性。如果需要，也可以附加<see cref="TransferFlags.MD5Verify"/>验证。</para>
        /// </summary>
        BreakpointResume = 1,

        /// <summary>
        /// MD5验证。该标识在文件传输完成时，也会再次验证文件长度。
        /// </summary>
        MD5Verify = 2,

        /// <summary>
        /// 当传输失败时，删除所有缓存文件。
        /// <para>注意：当启用断点续传时，该标识无效</para>
        /// </summary>
        DeleteWhenFail = 4
    }
}