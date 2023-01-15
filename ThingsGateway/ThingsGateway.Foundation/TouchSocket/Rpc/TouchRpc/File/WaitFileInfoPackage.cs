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
    /// 文件信息
    /// </summary>
    internal class WaitFileInfoPackage : WaitRouterPackage
    {
        /// <summary>
        /// 元数据
        /// </summary>
        public Metadata Metadata { get; set; }

        /// <summary>
        /// 文件信息
        /// </summary>
        public TouchRpcFileInfo FileInfo { get; set; }

        /// <summary>
        /// 传输标识
        /// </summary>
        public TransferFlags Flags { get; set; }

        /// <summary>
        /// 存放路径，
        /// 可输入绝对路径，也可以输入相对路径。
        /// 但是必须包含文件名及扩展名。
        /// </summary>
        public string SavePath { get; set; }

        /// <summary>
        /// 请求文件路径，
        /// 可输入绝对路径，也可以输入相对路径。
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// 事件Code
        /// </summary>
        public int EventHashCode { get; set; }

        public override void PackageBody(ByteBlock byteBlock)
        {
            base.PackageBody(byteBlock);
            byteBlock.WritePackage(Metadata);
            byteBlock.WritePackage(FileInfo);
            byteBlock.Write((byte)Flags);
            byteBlock.Write(SavePath);
            byteBlock.Write(ResourcePath);
            byteBlock.Write(EventHashCode);
        }

        public override void UnpackageBody(ByteBlock byteBlock)
        {
            base.UnpackageBody(byteBlock);
            this.Metadata = byteBlock.ReadPackage<Metadata>();
            this.FileInfo = byteBlock.ReadPackage<TouchRpcFileInfo>();
            this.Flags = (TransferFlags)byteBlock.ReadByte();
            this.SavePath = byteBlock.ReadString();
            this.ResourcePath = byteBlock.ReadString();
            this.EventHashCode = byteBlock.ReadInt32();
        }
    }
}