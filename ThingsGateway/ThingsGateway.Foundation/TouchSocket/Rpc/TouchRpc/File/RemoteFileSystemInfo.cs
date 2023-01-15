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
using System

.IO;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// 远程文件系统信息
    /// </summary>
    public abstract class RemoteFileSystemInfo : PackageBase
    {
        /// <summary>
        /// 目录或文件的名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 目录或文件的完整目录。
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 上次写入当前文件或目录的时间
        /// </summary>
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// 上次访问当前文件或目录的时间
        /// </summary>
        public DateTime LastAccessTime { get; set; }

        /// <summary>
        /// 当前文件或目录的创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 当前文件或目录的特性
        /// </summary>
        public FileAttributes Attributes { get; set; }

        /// <inheritdoc/>
        public override void Package(ByteBlock byteBlock)
        {
            byteBlock.Write(LastWriteTime);
            byteBlock.Write(LastAccessTime);
            byteBlock.Write(CreationTime);
            byteBlock.Write((int)Attributes);
            byteBlock.Write(FullName);
            byteBlock.Write(Name);
        }

        /// <inheritdoc/>
        public override void Unpackage(ByteBlock byteBlock)
        {
            LastWriteTime = byteBlock.ReadDateTime();
            LastAccessTime = byteBlock.ReadDateTime();
            CreationTime = byteBlock.ReadDateTime();
            Attributes = (FileAttributes)byteBlock.ReadInt32();
            FullName = byteBlock.ReadString();
            Name = byteBlock.ReadString();
        }
    }
}