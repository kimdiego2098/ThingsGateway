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
    /// 传输类型
    /// </summary>
    [Flags]
    public enum TransferType
    {
        /// <summary>
        /// 推送
        /// </summary>
        Push = 0,

        /// <summary>
        /// 拉取
        /// </summary>
        Pull = 1,

        /// <summary>
        /// 分块推送
        /// </summary>
        SectionPush = 2,

        /// <summary>
        /// 分块拉取
        /// </summary>
        SectionPull = 4,

        /// <summary>
        /// 小文件推送
        /// </summary>
        SmallPush = 8,

        /// <summary>
        /// 小文件拉取
        /// </summary>
        SmallPull = 16
    }

    /// <summary>
    /// TransferTypeExtension
    /// </summary>
    public static class TransferTypeExtension
    {
        /// <summary>
        /// 表示当前传输类型是否属于<see cref="TransferType.Pull"/>、<see cref="TransferType.SectionPull"/>、<see cref="TransferType.SmallPull"/>其中的一种。
        /// </summary>
        /// <param name="transferType"></param>
        /// <returns></returns>
        public static bool IsPull(this TransferType transferType)
        {
            return transferType == TransferType.Pull || transferType == TransferType.SmallPull || transferType == TransferType.SectionPull;
        }
    }
}