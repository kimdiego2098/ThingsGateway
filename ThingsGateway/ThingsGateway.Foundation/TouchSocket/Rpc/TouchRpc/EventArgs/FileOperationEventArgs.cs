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
    /// 操作文件事件类
    /// </summary>
    public class FileOperationEventArgs : MsgEventArgs
    {
        private string m_savePath;
        private string m_resourcePath;
        private readonly Metadata m_metadata;

        /// <summary>
        /// FileOperationEventArgs
        /// </summary>
        /// <param name="transferType"></param>
        /// <param name="fileOperator"></param>
        /// <param name="fileInfo"></param>
        public FileOperationEventArgs(TransferType transferType, FileOperator fileOperator, RemoteFileInfo fileInfo)
        {
            FileOperator = fileOperator;
            TransferType = transferType;
            FileInfo = fileInfo;
        }

        /// <summary>
        /// FileOperationEventArgs
        /// </summary>
        /// <param name="transferType"></param>
        /// <param name="metadata"></param>
        /// <param name="fileInfo"></param>
        public FileOperationEventArgs(TransferType transferType, Metadata metadata, RemoteFileInfo fileInfo)
        {
            FileOperator = default;
            TransferType = transferType;
            FileInfo = fileInfo;
            m_metadata = metadata;
        }

        /// <summary>
        /// 存放路径，
        /// 可输入绝对路径，也可以输入相对路径。
        /// 但是必须包含文件名及扩展名。
        /// </summary>
        public string SavePath
        {
            get => FileOperator == null ? m_savePath : FileOperator.SavePath;
            set
            {
                if (FileOperator == null)
                {
                    m_savePath = value;
                }
                else
                {
                    FileOperator.SavePath = value;
                }
            }
        }

        /// <summary>
        /// 请求文件路径，
        /// 可输入绝对路径，也可以输入相对路径。
        /// </summary>
        public string ResourcePath
        {
            get => FileOperator == null ? m_resourcePath : FileOperator.ResourcePath;
            set
            {
                if (FileOperator == null)
                {
                    m_resourcePath = value;
                }
                else
                {
                    FileOperator.ResourcePath = value;
                }
            }
        }

        /// <summary>
        /// 元数据
        /// </summary>
        public Metadata Metadata => FileOperator == null ? m_metadata : FileOperator.Metadata;

        /// <summary>
        /// 文件操作器
        /// </summary>
        public FileOperator FileOperator { get; private set; }

        /// <summary>
        /// 文件信息
        /// </summary>
        public RemoteFileInfo FileInfo { get; private set; }

        /// <summary>
        /// 传输标识
        /// </summary>
        public TransferFlags Flags { get => FileOperator == null ? TransferFlags.None : FileOperator.Flags; }

        /// <summary>
        /// 传输类型
        /// </summary>
        public TransferType TransferType { get; private set; }
    }
}