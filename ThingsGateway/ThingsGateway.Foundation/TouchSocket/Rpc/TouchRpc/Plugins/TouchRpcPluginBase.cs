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
    /// TouchRpc插件基类
    /// </summary>
    public class TouchRpcPluginBase : TouchRpcPluginBase<ITouchRpc>
    {
    }

    /// <summary>
    /// TouchRpc插件基类
    /// </summary>
    public partial class TouchRpcPluginBase<TClient> : TcpPluginBase<TClient>, ITouchRpcPlugin
    {
        #region 虚函数

        /// <summary>
        /// 当文件传输结束之后。并不意味着完成传输，请通过<see cref="FileTransferStatusEventArgs.Result"/>属性值进行判断。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnFileTransfered(TClient client, FileTransferStatusEventArgs e)
        {
        }

        /// <summary>
        /// 当文件传输结束之后。并不意味着完成传输，请通过<see cref="FileTransferStatusEventArgs.Result"/>属性值进行判断。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnFileTransferedAsync(TClient client, FileTransferStatusEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// 在文件传输即将进行时触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnFileTransfering(TClient client, FileOperationEventArgs e)
        {
        }

        /// <summary>
        /// 在文件传输即将进行时触发。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnFileTransferingAsync(TClient client, FileOperationEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// 在完成握手连接时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnHandshaked(TClient client, VerifyOptionEventArgs e)
        {
        }

        /// <summary>
        /// 在完成握手连接时。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnHandshakedAsync(TClient client, VerifyOptionEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// 在验证Token时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnHandshaking(TClient client, VerifyOptionEventArgs e)
        {
        }

        /// <summary>
        /// 在验证Token时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnHandshakingAsync(TClient client, VerifyOptionEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// 收到协议数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnReceivedProtocolData(TClient client, ProtocolDataEventArgs e)
        {
        }

        /// <summary>
        /// 收到协议数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnReceivedProtocolDataAsync(TClient client, ProtocolDataEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// 当需要转发路由包时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnRouting(TClient client, PackageRouterEventArgs e)
        {
        }

        /// <summary>
        /// 当需要转发路由包时
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnRoutingAsync(TClient client, PackageRouterEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// 流数据处理，用户需要在此事件中对e.Bucket手动释放。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnStreamTransfered(TClient client, StreamStatusEventArgs e)
        {
        }

        /// <summary>
        /// 流数据处理，用户需要在此事件中对e.Bucket手动释放。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnStreamTransferedAsync(TClient client, StreamStatusEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        /// <summary>
        /// 即将接收流数据，用户需要在此事件中对e.Bucket初始化。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        protected virtual void OnStreamTransfering(TClient client, StreamOperationEventArgs e)
        {
        }

        /// <summary>
        /// 即将接收流数据，用户需要在此事件中对e.Bucket初始化。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnStreamTransferingAsync(TClient client, StreamOperationEventArgs e)
        {
            return EasyTask.CompletedTask;
        }

        #endregion 虚函数

        void ITouchRpcPlugin.OnFileTransfered(ITouchRpc client, FileTransferStatusEventArgs e)
        {
            OnFileTransfered((TClient)client, e);
        }

        Task ITouchRpcPlugin.OnFileTransferedAsync(ITouchRpc client, FileTransferStatusEventArgs e)
        {
            return OnFileTransferedAsync((TClient)client, e);
        }

        void ITouchRpcPlugin.OnFileTransfering(ITouchRpc client, FileOperationEventArgs e)
        {
            OnFileTransfering((TClient)client, e);
        }

        Task ITouchRpcPlugin.OnFileTransferingAsync(ITouchRpc client, FileOperationEventArgs e)
        {
            return OnFileTransferingAsync((TClient)client, e);
        }

        void ITouchRpcPlugin.OnHandshaked(ITouchRpc client, VerifyOptionEventArgs e)
        {
            OnHandshaked((TClient)client, e);
        }

        Task ITouchRpcPlugin.OnHandshakedAsync(ITouchRpc client, VerifyOptionEventArgs e)
        {
            return OnHandshakedAsync((TClient)client, e);
        }

        void ITouchRpcPlugin.OnHandshaking(ITouchRpc client, VerifyOptionEventArgs e)
        {
            OnHandshaking((TClient)client, e);
        }

        Task ITouchRpcPlugin.OnHandshakingAsync(ITouchRpc client, VerifyOptionEventArgs e)
        {
            return OnHandshakingAsync((TClient)client, e);
        }

        void ITouchRpcPlugin.OnReceivedProtocolData(ITouchRpc client, ProtocolDataEventArgs e)
        {
            OnReceivedProtocolData((TClient)client, e);
        }

        Task ITouchRpcPlugin.OnReceivedProtocolDataAsync(ITouchRpc client, ProtocolDataEventArgs e)
        {
            return OnReceivedProtocolDataAsync((TClient)client, e);
        }

        void ITouchRpcPlugin.OnRouting(ITouchRpc client, PackageRouterEventArgs e)
        {
            OnRouting((TClient)client, e);
        }

        Task ITouchRpcPlugin.OnRoutingAsync(ITouchRpc client, PackageRouterEventArgs e)
        {
            return OnRoutingAsync((TClient)client, e);
        }

        void ITouchRpcPlugin.OnStreamTransfered(ITouchRpc client, StreamStatusEventArgs e)
        {
            OnStreamTransfered((TClient)client, e);
        }

        Task ITouchRpcPlugin.OnStreamTransferedAsync(ITouchRpc client, StreamStatusEventArgs e)
        {
            return OnStreamTransferedAsync((TClient)client, e);
        }

        void ITouchRpcPlugin.OnStreamTransfering(ITouchRpc client, StreamOperationEventArgs e)
        {
            OnStreamTransfering((TClient)client, e);
        }

        Task ITouchRpcPlugin.OnStreamTransferingAsync(ITouchRpc client, StreamOperationEventArgs e)
        {
            return OnStreamTransferingAsync((TClient)client, e);
        }
    }
}