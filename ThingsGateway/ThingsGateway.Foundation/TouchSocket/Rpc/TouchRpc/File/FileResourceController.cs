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

using System.IO;



namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// 文件资源控制器。
    /// </summary>
    public class FileResourceController : IFileResourceController
    {

        /// <inheritdoc/>
        public virtual void GetFileInfo<T>(string path, bool md5, ref T fileInfo) where T : RemoteFileInfo
        {
            FileInfo info = new FileInfo(path);
            fileInfo.FullName = info.FullName;
            fileInfo.Name = info.Name;
            fileInfo.Attributes = info.Attributes;
            fileInfo.CreationTime = info.CreationTime;
            fileInfo.Length = info.Length;
            fileInfo.LastAccessTime = info.LastAccessTime;
            fileInfo.LastWriteTime = info.LastWriteTime;

            if (md5)
            {
                using FileStream fileStream = File.OpenRead(path);
                fileInfo.MD5 = FileUtility.GetStreamMD5(fileStream);
            }
        }

        /// <inheritdoc/>
        public virtual string GetFullPath(string root, string path)
        {
            if (path.IsNullOrEmpty())
            {
                return string.Empty;
            }
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                return Path.GetFullPath(Path.Combine(root, path));
            }
        }


        /// <inheritdoc/>
        public virtual bool TryReadTempInfo(string path, TransferFlags flags, ref TouchRpcFileInfo info)
        {
            if (flags == TransferFlags.None)
            {
                return false;
            }
            string filePath = path + ".rrqm";
            string tempPath = path + ".temp";
            if (File.Exists(filePath) && File.Exists(tempPath))
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    TouchRpcFileInfo tempInfo = SerializeConvert.JsonDeserializeFromString<TouchRpcFileInfo>(File.ReadAllText(tempPath));
                    if (flags.HasFlag(TransferFlags.BreakpointResume))
                    {
                        if (tempInfo.Length != info.Length)
                        {
                            return false;
                        }
                        if (flags.HasFlag(TransferFlags.MD5Verify))
                        {
                            if (tempInfo.MD5 == info.MD5 && info.MD5.HasValue())
                            {
                                info.Position = tempInfo.Position;
                                return true;
                            }
                        }
                        else
                        {
                            info.Position = tempInfo.Position;
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public virtual int ReadAllBytes(FileInfo fileInfo, byte[] buffer)
        {
            using (FileStorageReader reader = FilePool.GetReader(fileInfo))
            {
                return reader.Read(buffer, 0, buffer.Length);
            }
        }

        /// <inheritdoc/>
        public virtual void WriteAllBytes(FileInfo fileInfo, byte[] buffer, int offset, int length)
        {
            FileUtility.Delete(fileInfo.FullName);
            using (var writer = FilePool.GetWriter(fileInfo))
            {
                writer.Write(buffer, offset, length);
            }
        }
    }
}