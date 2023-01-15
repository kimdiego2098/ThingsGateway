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

using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// ITouchRpcPlugin
    /// </summary>
    public partial interface ITouchRpcPlugin : IPlugin
    {
        /// <summary>
        /// 当文件传输结束之后。并不意味着完成传输，请通过<see cref="FileTransferStatusEventArgs.Result"/>属性值进行判断。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnFileTransfered(ITouchRpc client, FileTransferStatusEventArgs e);

        /// <summary>
        /// 当文件传输结束之后。并不意味着完成传输，请通过<see cref="FileTransferStatusEventArgs.Result"/>属性值进行判断。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnFileTransferedAsync(ITouchRpc client, FileTransferStatusEventArgs e);

        /// <summary>
        /// 在文件传输即将进行时触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnFileTransfering(ITouchRpc client, FileOperationEventArgs e);

        /// <summary>
        /// 在文件传输即将进行时触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnFileTransferingAsync(ITouchRpc client, FileOperationEventArgs e);

        /// <summary>
        /// 在完成握手连接时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnHandshaked(ITouchRpc client, VerifyOptionEventArgs e);

        /// <summary>
        /// 当需要转发路由包时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnRouting(ITouchRpc client, PackageRouterEventArgs e);

        /// <summary>
        /// 当需要转发路由包时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        Task OnRoutingAsync(ITouchRpc client, PackageRouterEventArgs e);

        /// <summary>
        /// 在完成握手连接时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnHandshakedAsync(ITouchRpc client, VerifyOptionEventArgs e);

        /// <summary>
        /// 在验证Token时
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        [AsyncRaiser]
        void OnHandshaking(ITouchRpc client, VerifyOptionEventArgs e);

        /// <summary>
        /// 在验证Token时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnHandshakingAsync(ITouchRpc client, VerifyOptionEventArgs e);

        /// <summary>
        /// 收到协议数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnReceivedProtocolData(ITouchRpc client, ProtocolDataEventArgs e);

        /// <summary>
        /// 收到协议数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnReceivedProtocolDataAsync(ITouchRpc client, ProtocolDataEventArgs e);

        /// <summary>
        /// 流数据处理，用户需要在此事件中对e.Bucket手动释放。
        /// 当流数据传输结束之后。并不意味着完成传输，请通过<see cref="StreamStatusEventArgs.Result"/>属性值进行判断。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnStreamTransfered(ITouchRpc client, StreamStatusEventArgs e);

        /// <summary>
        /// 流数据处理，用户需要在此事件中对e.Bucket手动释放。
        /// 当流数据传输结束之后。并不意味着完成传输，请通过<see cref="StreamStatusEventArgs.Result"/>属性值进行判断。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnStreamTransferedAsync(ITouchRpc client, StreamStatusEventArgs e);

        /// <summary>
        /// 即将接收流数据，用户需要在此事件中对e.Bucket初始化。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        [AsyncRaiser]
        void OnStreamTransfering(ITouchRpc client, StreamOperationEventArgs e);

        /// <summary>
        /// 即将接收流数据，用户需要在此事件中对e.Bucket初始化。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnStreamTransferingAsync(ITouchRpc client, StreamOperationEventArgs e);
    }
}