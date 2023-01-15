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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 文件写入器。
    /// </summary>
    public class FileStorageWriter : DisposableObject, IWrite
    {
        private readonly FileStorage m_fileStorage;
        private long m_position;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileStorage"></param>
        public FileStorageWriter(FileStorage fileStorage)
        {
            m_fileStorage = fileStorage ?? throw new System.ArgumentNullException(nameof(fileStorage));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~FileStorageWriter()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        /// <summary>
        /// 文件存储器
        /// </summary>
        public FileStorage FileStorage => m_fileStorage;

        /// <summary>
        /// 游标位置
        /// </summary>
        public int Pos
        {
            get => (int)m_position;
            set => m_position = value;
        }

        /// <summary>
        /// 游标位置
        /// </summary>
        public long Position
        {
            get => m_position;
            set => m_position = value;
        }

        /// <summary>
        /// 移动Pos到流末尾
        /// </summary>
        /// <returns></returns>
        public long SeekToEnd()
        {
            return Position = FileStorage.Length;
        }

        /// <summary>
        /// 读取数据到缓存区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public void Write(byte[] buffer, int offset, int length)
        {
            m_fileStorage.Write(m_position, buffer, offset, length);
            m_position += length;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            FilePool.TryReleaseFile(m_fileStorage.Path);
            base.Dispose(disposing);
        }
    }
}