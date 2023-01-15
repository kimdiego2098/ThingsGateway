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
using System.Threading
.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// ITargetRpcActor
    /// </summary>
    public partial interface ITargetRpcActor : ITargetRpcClient
    {
        /// <summary>
        /// 创建一个和其他客户端的通道
        /// </summary>
        /// <param name="targetId">目标客户端ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Channel CreateChannel(string targetId);

        /// <summary>
        /// 创建一个和其他客户端的通道
        /// </summary>
        /// <param name="targetId">目标客户端ID</param>
        /// <param name="id">通道Id</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Channel CreateChannel(string targetId, int id);

        /// <summary>
        /// 向指定的Id执行ping。
        /// </summary>
        /// <param name="targetId">目标客户端ID</param>
        /// <param name="timeout">超时配置</param>
        /// <returns>如果返回True，则表示一定在线。如果返回false，则不一定代表不在线。</returns>
        bool Ping(string targetId, int timeout = 5000);

        #region 文件传输

        /// <summary>
        /// 从对点拉取文件
        /// </summary>
        /// <param name="targetId">目标客户端ID</param>
        /// <param name="fileOperator">文件传输操作器</param>
        /// <returns></returns>
        Result PullFile(string targetId, FileOperator fileOperator);

        /// <summary>
        /// 异步从对点拉取文件
        /// </summary>
        /// <param name="targetId">目标客户端ID</param>
        /// <param name="fileOperator">文件传输操作器</param>
        /// <returns></returns>
        Task<Result> PullFileAsync(string targetId, FileOperator fileOperator);

        /// <summary>
        /// 向对点推送文件
        /// </summary>
        /// <param name="targetId">目标客户端ID</param>
        /// <param name="fileOperator">文件传输操作器</param>
        /// <returns></returns>
        Result PushFile(string targetId, FileOperator fileOperator);

        /// <summary>
        /// 异步向对点推送文件
        /// </summary>
        /// <param name="targetId">目标客户端ID</param>
        /// <param name="fileOperator">文件传输操作器</param>
        /// <returns></returns>
        Task<Result> PushFileAsync(string targetId, FileOperator fileOperator);

        #endregion 文件传输

        #region 小文件

        /// <summary>
        /// 拉取小文件。默认设置1024*1024字节大小。
        /// </summary>
        /// <param name="targetId">目标客户端ID</param>
        /// <param name="path">请求路径</param>
        /// <param name="metadata">元数据</param>
        /// <param name="timeout">超时设置</param>
        /// <param name="token">可取消令箭</param>
        /// <returns></returns>
        PullSmallFileResult PullSmallFile(string targetId, string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default);

        /// <summary>
        /// 拉取小文件。默认设置1024*1024字节大小。
        /// </summary>
        /// <param name="targetId">目标客户端Id</param>
        /// <param name="path">请求路径</param>
        /// <param name="metadata">元数据</param>
        /// <param name="timeout">超时设置</param>
        /// <param name="token">可取消令箭</param>
        /// <returns></returns>
        Task<PullSmallFileResult> PullSmallFileAsync(string targetId, string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default);

        /// <summary>
        /// 推送小文件到特定的Id。默认设置1024*1024字节大小。
        /// </summary>
        /// <param name="targetId">目标客户端Id</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="fileInfo">推送的文件信息</param>
        /// <param name="metadata">元数据</param>
        /// <param name="timeout">超时设置</param>
        /// <param name="token">可取消令箭</param>
        /// <returns></returns>
        Result PushSmallFile(string targetId, string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default);

        /// <summary>
        /// 推送小文件。默认设置1024*1024字节大小。
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="savePath">保存路径</param>
        /// <param name="fileInfo">推送的文件信息</param>
        /// <param name="metadata">元数据</param>
        /// <param name="timeout">超时设置</param>
        /// <param name="token">可取消令箭</param>
        /// <returns></returns>
        Task<Result> PushSmallFileAsync(string targetId, string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default);

        #endregion 小文件
    }
}