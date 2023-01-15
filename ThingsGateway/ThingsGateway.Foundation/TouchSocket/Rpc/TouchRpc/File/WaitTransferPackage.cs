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
    /// 等待传输
    /// </summary>
    internal class WaitTransferPackage : WaitRouterPackage
    {
        /// <summary>
        /// 通道标识
        /// </summary>
        public int ChannelID { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 流位置
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// 事件Code
        /// </summary>
        public int EventHashCode { get; set; }

        public override void PackageBody(ByteBlock byteBlock)
        {
            base.PackageBody(byteBlock);
            byteBlock.Write(ChannelID);
            byteBlock.Write(Path);
            byteBlock.Write(Position);
            byteBlock.Write(EventHashCode);
        }

        public override void UnpackageBody(ByteBlock byteBlock)
        {
            base.UnpackageBody(byteBlock);
            this.ChannelID = byteBlock.ReadInt32();
            this.Path = byteBlock.ReadString();
            this.Position = byteBlock.ReadInt64();
            this.EventHashCode = byteBlock.ReadInt32();
        }
    }
}