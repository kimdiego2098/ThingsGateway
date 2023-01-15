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
    public interface IFileResourceController
    {
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="md5"></param>
        /// <param name="fileInfo"></param>
        void GetFileInfo<T>(string path, bool md5, ref T fileInfo) where T : RemoteFileInfo;

        /// <summary>
        /// 获取全路径
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetFullPath(string root, string path);

        /// <summary>
        /// 读取缓存文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="flags"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        bool TryReadTempInfo(string path, TransferFlags flags, ref TouchRpcFileInfo info);

        /// <summary>
        /// 读取文件的所有数据
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int ReadAllBytes(FileInfo fileInfo, byte[] buffer);

        /// <summary>
        /// 写入数据到文件
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        void WriteAllBytes(FileInfo fileInfo, byte[] buffer, int offset, int length);
    }
}