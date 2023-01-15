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
using System.Collections.Concurrent;
using System.Linq;
using System.Threading
.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// RpcActor
    /// </summary>
    public partial class RpcActor : DisposableObject, IRpcActor
    {
        /// <summary>
        /// 结束标识编码。
        /// </summary>
        public static readonly byte[] EndCodes = new byte[] { 20, 17, 11, 25 };

        #region 委托

        /// <summary>
        /// 获取调用函数的委托
        /// </summary>
        public Func<string, MethodInstance> GetInvokeMethod { get; set; }

        /// <summary>
        /// 请求关闭
        /// </summary>
        public Action<RpcActor, string> OnClose { get; set; }

        /// <summary>
        /// 当文件传输结束之后。并不意味着完成传输，请通过<see cref="FileTransferStatusEventArgs.Result"/>属性值进行判断。
        /// </summary>
        public Action<RpcActor, FileTransferStatusEventArgs> OnFileTransfered { get; set; }

        /// <summary>
        /// 当需要路由的时候
        /// </summary>
        public Action<RpcActor, PackageRouterEventArgs> OnRouting { get; set; }

        /// <summary>
        /// 在文件传输即将进行时触发。
        /// </summary>
        public Action<RpcActor, FileOperationEventArgs> OnFileTransfering { get; set; }

        /// <summary>
        /// 查找其他RpcActor
        /// </summary>
        public Func<string, RpcActor> OnFindRpcActor { get; set; }

        /// <summary>
        /// 在完成握手连接时
        /// </summary>
        public Action<RpcActor, VerifyOptionEventArgs> OnHandshaked { get; set; }

        /// <summary>
        /// 握手
        /// </summary>
        public Action<RpcActor, VerifyOptionEventArgs> OnHandshaking { get; set; }


        /// <summary>
        /// 接收到数据
        /// </summary>
        public Action<RpcActor, short, ByteBlock> OnReceived { get; set; }

        /// <summary>
        /// 重设ID
        /// </summary>
        public Action<bool, RpcActor, WaitSetID> OnResetID { get; set; }

        /// <summary>
        /// 流数据处理，用户需要在此事件中对e.Bucket手动释放。
        /// </summary>
        public Action<RpcActor, StreamStatusEventArgs> OnStreamTransfered { get; set; }

        /// <summary>
        /// 即将接收流数据，用户需要在此事件中对e.Bucket初始化。
        /// </summary>
        public Action<RpcActor, StreamOperationEventArgs> OnStreamTransfering { get; set; }

        /// <summary>
        /// 发送数据接口
        /// </summary>
        public Action<RpcActor, ArraySegment<byte>[]> OutputSend { get; set; }

        #endregion 委托

        #region 属性

        /// <summary>
        /// 文件资源访问接口。
        /// </summary>
        public IFileResourceController FileController { get; set; }

        /// <summary>
        /// 本节点ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsHandshaked { get; private set; }

        /// <summary>
        /// 是否为服务器组件
        /// </summary>
        public bool IsService { get; private set; }

        /// <summary>
        /// 日志
        /// </summary>
        public ILog Logger { get; set; }

        /// <summary>
        ///  获取可用于同步对<see cref="RpcActor"/>的访问的对象。
        /// </summary>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Func<IRpcClient, bool> TryCanInvoke { get; set; }

        /// <summary>
        /// 等待返回池
        /// </summary>
        public WaitHandlePool<IWaitResult> WaitHandlePool { get; private set; }

        #endregion 属性

        /// <summary>
        /// 构造函数
        /// </summary>
        public RpcActor(bool isService)
        {
            m_eventArgs = new ConcurrentDictionary<int, object>();
            WaitHandlePool = new WaitHandlePool<IWaitResult>();
            m_userChannels = new ConcurrentDictionary<int, InternalChannel>();
            m_contextDic = new ConcurrentDictionary<long, TouchRpcCallContext>();
            IsService = isService;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="message"></param>
        public void Close(string message)
        {
            if (IsHandshaked)
            {
                IsHandshaked = false;
                foreach (var item in m_userChannels.Values)
                {
                    item.RequestDispose(false);
                }
                var keys = m_contextDic.Keys.ToArray();
                foreach (var item in keys)
                {
                    if (m_contextDic.TryRemove(item, out TouchRpcCallContext rpcCallContext))
                    {
                        rpcCallContext.TryCancel();
                    }
                }
                WaitHandlePool.CancelAll();
                OnClose?.Invoke(this, message);
            }
        }

        /// <summary>
        /// 建立对点
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="TokenVerifyException"></exception>
        /// <exception cref="TimeoutException"></exception>
        public void Handshake(string verifyToken, string id, CancellationToken token = default, int timeout = 5000, Metadata metadata = null)
        {
            if (IsHandshaked)
            {
                return;
            }
            WaitVerify waitVerify = new WaitVerify()
            {
                Token = verifyToken,
                ID = id,
                Metadata = metadata
            };
            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(waitVerify);
            waitData.SetCancellationToken(token);

            try
            {
                SendFastObject(TouchRpcUtility.P_0_Handshake_Request, waitVerify);
                switch (waitData.Wait(timeout))
                {
                    case WaitDataStatus.SetRunning:
                        {
                            WaitVerify verifyResult = (WaitVerify)waitData.WaitResult;
                            if (verifyResult.Status == 1)
                            {
                                ID = verifyResult.ID;
                                IsHandshaked = true;
                                OnHandshaked?.Invoke(this, new VerifyOptionEventArgs(verifyToken, verifyResult.Metadata) { Message = "成功建立" });
                                verifyResult.Handle = true;
                                return;
                            }
                            else if (verifyResult.Status == 3)
                            {
                                verifyResult.Handle = true;
                                Close("连接数量已达到服务器设定最大值");
                                throw new Exception("连接数量已达到服务器设定最大值");
                            }
                            else if (verifyResult.Status == 4)
                            {
                                verifyResult.Handle = true;
                                Close("服务器拒绝连接");
                                throw new Exception("服务器拒绝连接");
                            }
                            else
                            {
                                verifyResult.Handle = true;
                                Close(verifyResult.Message);
                                throw new TokenVerifyException(verifyResult.Message);
                            }
                        }
                    case WaitDataStatus.Overtime:
                        Close("连接超时");
                        throw new TimeoutException("连接超时");
                    case WaitDataStatus.Canceled:
                    case WaitDataStatus.Disposed:
                    default:
                        Close(null);
                        return;
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
            }
        }

        /// <summary>
        /// 处理接收数据
        /// </summary>
        /// <param name="byteBlock"></param>
        public void InputReceivedData(ByteBlock byteBlock)
        {
            short protocol = ThingsGatewayBitConverter.Default.ToInt16(byteBlock.Buffer, 0);
            switch (protocol)
            {
                #region 0-99

                case TouchRpcUtility.P_0_Handshake_Request:
                    {
                        try
                        {
                            WaitVerify waitVerify = byteBlock.Seek(2).ReadObject<WaitVerify>();
                            VerifyOptionEventArgs args = new VerifyOptionEventArgs(waitVerify.Token, waitVerify.Metadata);
                            if (!waitVerify.ID.IsNullOrEmpty())
                            {
                                OnResetID?.Invoke(true, this, new WaitSetID(ID, waitVerify.ID));
                                ID = waitVerify.ID;
                            }
                            OnHandshaking?.Invoke(this, args);

                            if (args.IsPermitOperation)
                            {
                                waitVerify.ID = ID;
                                waitVerify.Status = 1;
                                byteBlock.Reset();
                                byteBlock.WriteObject(waitVerify);
                                Send(TouchRpcUtility.P_1000_Handshake_Response, byteBlock);
                                IsHandshaked = true;
                                args.Message = "Success";
                                OnHandshaked?.Invoke(this, args);
                            }
                            else
                            {
                                waitVerify.Status = 2;
                                waitVerify.Message = args.Message;
                                byteBlock.Reset();
                                byteBlock.WriteObject(waitVerify);
                                Send(TouchRpcUtility.P_1000_Handshake_Response, byteBlock);
                                Close(args.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            Close(ex.Message);
                        }
                        return;
                    }
                case TouchRpcUtility.P_1000_Handshake_Response:
                    {
                        WaitVerify waitVerify = byteBlock.Seek(2).ReadObject<WaitVerify>();
                        WaitHandlePool.SetRun(waitVerify);
                        SpinWait.SpinUntil(() =>
                        {
                            return waitVerify.Handle;
                        }, 3000);
                        return;
                    }
                case TouchRpcUtility.P_1_ResetID_Request:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitSetID waitSetID = byteBlock.ReadObject<WaitSetID>();
                            try
                            {
                                OnResetID?.Invoke(false, this, waitSetID);
                                ID = waitSetID.NewID;
                                waitSetID.Status = 1;
                            }
                            catch (System.Exception ex)
                            {
                                waitSetID.Status = 2;
                                waitSetID.Message = ex.Message;
                            }
                            byteBlock.Reset();
                            Send(TouchRpcUtility.P_1001_ResetID_Response, byteBlock.WriteObject(waitSetID));
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_1001_ResetID_Response:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitHandlePool.SetRun(byteBlock.ReadObject<WaitSetID>());
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_2_Ping_Request://心跳
                    {
                        byteBlock.Pos = 2;

                        try
                        {
                            WaitPingPackage waitPing = new WaitPingPackage();
                            waitPing.UnpackageRouter(byteBlock);
                            if (IsService && waitPing.Route)
                            {
                                if (TryRoute(RouteType.Ping, waitPing))
                                {
                                    if (TryFindRpcActor(waitPing.TargetId, out RpcActor actor))
                                    {
                                        actor.Send(TouchRpcUtility.P_2_Ping_Request, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                        return;
                                    }
                                    else
                                    {
                                        waitPing.UnpackageBody(byteBlock);
                                        waitPing.Status = TouchSocketStatus.ClientNotFind.ToValue();
                                    }
                                }
                                else
                                {
                                    waitPing.UnpackageBody(byteBlock);
                                    waitPing.Status = TouchSocketStatus.RoutingNotAllowed.ToValue();
                                }
                            }
                            else
                            {
                                waitPing.UnpackageBody(byteBlock);
                                waitPing.Status = TouchSocketStatus.Success.ToValue();
                            }
                            waitPing.SwitchId();
                            byteBlock.Reset();
                            waitPing.Package(byteBlock);
                            Send(TouchRpcUtility.P_1002_Ping_Response, byteBlock);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_1002_Ping_Response://心跳
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitPingPackage waitPing = new WaitPingPackage();
                            waitPing.UnpackageRouter(byteBlock);
                            if (IsService && waitPing.Route)
                            {
                                if (TryFindRpcActor(waitPing.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_1002_Ping_Response, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                }
                            }
                            else
                            {
                                waitPing.UnpackageBody(byteBlock);
                                WaitHandlePool.SetRun(waitPing);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }

                #endregion 0-99

                #region 100-199

                case TouchRpcUtility.P_100_CreateChannel_Request:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitCreateChannelPackage waitCreateChannel = new WaitCreateChannelPackage();
                            waitCreateChannel.UnpackageRouter(byteBlock);
                            if (this.IsService && waitCreateChannel.Route)
                            {
                                if (this.TryRoute(RouteType.CreateChannel, waitCreateChannel))
                                {
                                    if (TryFindRpcActor(waitCreateChannel.TargetId, out RpcActor actor))
                                    {
                                        actor.Send(TouchRpcUtility.P_100_CreateChannel_Request, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                        return;
                                    }
                                    else
                                    {
                                        waitCreateChannel.UnpackageBody(byteBlock);
                                        waitCreateChannel.Status = TouchSocketStatus.ClientNotFind.ToValue();
                                    }
                                }
                                else
                                {
                                    waitCreateChannel.UnpackageBody(byteBlock);
                                    waitCreateChannel.Status = TouchSocketStatus.RoutingNotAllowed.ToValue();
                                }
                            }
                            else
                            {
                                waitCreateChannel.UnpackageBody(byteBlock);

                                while (true)
                                {
                                    if (RequestCreateChannel(waitCreateChannel.ChannelID, waitCreateChannel.Route ? waitCreateChannel.SourceId : waitCreateChannel.TargetId))
                                    {
                                        waitCreateChannel.Status = TouchSocketStatus.Success.ToValue();
                                        break;
                                    }
                                    else
                                    {
                                        waitCreateChannel.Status = TouchSocketStatus.ChannelExisted.ToValue();
                                    }

                                    if (!waitCreateChannel.Random)
                                    {
                                        break;
                                    }
                                    waitCreateChannel.ChannelID = new object().GetHashCode();
                                }
                            }

                            waitCreateChannel.SwitchId();
                            byteBlock.Reset();
                            waitCreateChannel.Package(byteBlock);
                            Send(TouchRpcUtility.P_1100_CreateChannel_Response, byteBlock);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_1100_CreateChannel_Response:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitCreateChannelPackage waitCreateChannel = new WaitCreateChannelPackage();
                            waitCreateChannel.UnpackageRouter(byteBlock);
                            if (this.IsService && waitCreateChannel.Route)
                            {
                                if (TryFindRpcActor(waitCreateChannel.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_1100_CreateChannel_Response, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                    return;
                                }
                            }
                            else
                            {
                                waitCreateChannel.UnpackageBody(byteBlock);
                                WaitHandlePool.SetRun(waitCreateChannel);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_101_ChannelPackage:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            ChannelPackage channelPackage = new ChannelPackage();
                            channelPackage.UnpackageRouter(byteBlock);
                            if (this.IsService && channelPackage.Route)
                            {
                                if (TryFindRpcActor(channelPackage.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_101_ChannelPackage, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                }
                                else
                                {
                                    channelPackage.UnpackageBody(byteBlock);
                                    channelPackage.Message = TouchSocketStatus.ClientNotFind.GetDescription(channelPackage.TargetId);
                                    channelPackage.SwitchId();
                                    channelPackage.RunNow = true;
                                    channelPackage.DataType = ChannelDataType.DisposeOrder;
                                    byteBlock.Reset();
                                    channelPackage.Package(byteBlock);
                                    this.Send(TouchRpcUtility.P_101_ChannelPackage, byteBlock);
                                }
                            }
                            else
                            {
                                channelPackage.UnpackageBody(byteBlock);
                                QueueChannelPackage(channelPackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }

                #endregion 100-199

                #region 200-299

                case TouchRpcUtility.P_200_Invoke_Request:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitRouterPackage waitRouterPackage = new WaitRouterPackage();
                            waitRouterPackage.UnpackageRouter(byteBlock);
                            if (IsService && waitRouterPackage.Route)
                            {
                                if (this.TryRoute(RouteType.Rpc, waitRouterPackage))
                                {
                                    if (TryFindRpcActor(waitRouterPackage.TargetId, out RpcActor actor))
                                    {
                                        actor.Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                        return;
                                    }
                                    else
                                    {
                                        waitRouterPackage.UnpackageBody(byteBlock);
                                        waitRouterPackage.Status = TouchSocketStatus.ClientNotFind.ToValue();
                                    }
                                }
                                else
                                {
                                    waitRouterPackage.UnpackageBody(byteBlock);
                                    waitRouterPackage.Status = TouchSocketStatus.RoutingNotAllowed.ToValue();
                                }

                                byteBlock.Reset();
                                waitRouterPackage.SwitchId();

                                TouchRpcPackage rpcPackage = waitRouterPackage.Map<TouchRpcPackage>();
                                rpcPackage.Package(byteBlock);
                                this.Send(TouchRpcUtility.P_1200_Invoke_Response, byteBlock);
                            }
                            else
                            {
                                TouchRpcPackage rpcPackage = new TouchRpcPackage();
                                rpcPackage.Unpackage(byteBlock.Seek(2));
                                ThreadPool.QueueUserWorkItem(InvokeThis, rpcPackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_1200_Invoke_Response:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            TouchRpcPackage rpcPackage = new TouchRpcPackage();
                            rpcPackage.UnpackageRouter(byteBlock);
                            if (IsService && rpcPackage.Route)
                            {
                                if (this.TryFindRpcActor(rpcPackage.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_1200_Invoke_Response, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                }
                            }
                            else
                            {
                                rpcPackage.UnpackageBody(byteBlock);
                                WaitHandlePool.SetRun(rpcPackage);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_204_CancelInvoke:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            CanceledPackage canceledPackage = new CanceledPackage();
                            canceledPackage.UnpackageRouter(byteBlock);
                            if (IsService && canceledPackage.Route)
                            {
                                if (this.TryFindRpcActor(canceledPackage.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_204_CancelInvoke, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                }
                            }
                            else
                            {
                                canceledPackage.UnpackageBody(byteBlock);
                                if (m_contextDic.TryGetValue(canceledPackage.Sign, out TouchRpcCallContext context))
                                {
                                    context.TokenSource.Cancel();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }

                #endregion 200-299

                #region 400-499

                case TouchRpcUtility.P_400_SendStreamToSocketClient_Request://StreamStatusToThis
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            ThreadPool.QueueUserWorkItem(RequestStreamToSocketClient, byteBlock.ReadObject<WaitStream>());
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_1400_SendStreamToSocketClient_Response://StreamStatusToThis
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitStream waitStream = byteBlock.ReadObject<WaitStream>();
                            WaitHandlePool.SetRun(waitStream);
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_401_SendStreamToClient://StreamToThis
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            ThreadPool.QueueUserWorkItem(RequestStreamToClient, byteBlock.ReadObject<WaitStream>());
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }

                #endregion 400-499

                #region 500-599

                case TouchRpcUtility.P_500_PullFile_Request:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitFileInfoPackage waitFileInfoPackage = new WaitFileInfoPackage();
                            waitFileInfoPackage.UnpackageRouter(byteBlock);
                            if (IsService && waitFileInfoPackage.Route)
                            {
                                if (this.TryRoute(RouteType.PullFile, waitFileInfoPackage))
                                {
                                    if (TryFindRpcActor(waitFileInfoPackage.TargetId, out RpcActor actor))
                                    {
                                        actor.Send(TouchRpcUtility.P_500_PullFile_Request, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                        return;
                                    }
                                    else
                                    {
                                        waitFileInfoPackage.UnpackageBody(byteBlock);
                                        waitFileInfoPackage.Status = TouchSocketStatus.ClientNotFind.ToValue();
                                    }
                                }
                                else
                                {
                                    waitFileInfoPackage.UnpackageBody(byteBlock);
                                    waitFileInfoPackage.Status = TouchSocketStatus.RoutingNotAllowed.ToValue();
                                }

                                byteBlock.Reset();
                                waitFileInfoPackage.SwitchId();
                                waitFileInfoPackage.Package(byteBlock);
                                this.Send(TouchRpcUtility.P_1500_PullFile_Response, byteBlock);
                            }
                            else
                            {
                                waitFileInfoPackage.UnpackageBody(byteBlock);
                                ThreadPool.QueueUserWorkItem(RequestPullFile, waitFileInfoPackage);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_1500_PullFile_Response:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitFileInfoPackage waitFileInfoPackage = new WaitFileInfoPackage();
                            waitFileInfoPackage.UnpackageRouter(byteBlock);
                            if (IsService && waitFileInfoPackage.Route)
                            {
                                if (TryFindRpcActor(waitFileInfoPackage.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_1500_PullFile_Response, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                }
                            }
                            else
                            {
                                waitFileInfoPackage.UnpackageBody(byteBlock);
                                WaitHandlePool.SetRun(waitFileInfoPackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }

                        return;
                    }
                case TouchRpcUtility.P_501_BeginPullFile_Request:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitTransferPackage waitTransferPackage = new WaitTransferPackage();
                            waitTransferPackage.UnpackageRouter(byteBlock);
                            if (IsService && waitTransferPackage.Route)
                            {
                                if (TryFindRpcActor(waitTransferPackage.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_501_BeginPullFile_Request, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                    return;
                                }
                                else
                                {
                                    waitTransferPackage.UnpackageBody(byteBlock);
                                    waitTransferPackage.Status = TouchSocketStatus.ClientNotFind.ToValue();
                                }
                                byteBlock.Reset();
                                waitTransferPackage.SwitchId();
                                waitTransferPackage.Package(byteBlock);
                                this.Send(TouchRpcUtility.P_1501_BeginPullFile_Response, byteBlock);
                            }
                            else
                            {
                                waitTransferPackage.UnpackageBody(byteBlock);
                                ThreadPool.QueueUserWorkItem(BeginPullFile, waitTransferPackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_1501_BeginPullFile_Response:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitTransferPackage waitTransferPackage = new WaitTransferPackage();
                            waitTransferPackage.UnpackageRouter(byteBlock);
                            if (IsService && waitTransferPackage.Route)
                            {
                                if (TryFindRpcActor(waitTransferPackage.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_1501_BeginPullFile_Response, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                }
                            }
                            else
                            {
                                waitTransferPackage.UnpackageBody(byteBlock);
                                WaitHandlePool.SetRun(waitTransferPackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_502_PushFile_Request:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitFileInfoPackage waitFileInfoPackage = new WaitFileInfoPackage();
                            waitFileInfoPackage.UnpackageRouter(byteBlock);
                            if (IsService && waitFileInfoPackage.Route)
                            {
                                if (this.TryRoute(RouteType.PullFile, waitFileInfoPackage))
                                {
                                    if (TryFindRpcActor(waitFileInfoPackage.TargetId, out RpcActor actor))
                                    {
                                        actor.Send(TouchRpcUtility.P_502_PushFile_Request, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                        return;
                                    }
                                    else
                                    {
                                        waitFileInfoPackage.UnpackageBody(byteBlock);
                                        waitFileInfoPackage.Status = TouchSocketStatus.ClientNotFind.ToValue();
                                    }
                                }
                                else
                                {
                                    waitFileInfoPackage.UnpackageBody(byteBlock);
                                    waitFileInfoPackage.Status = TouchSocketStatus.RoutingNotAllowed.ToValue();
                                }

                                byteBlock.Reset();
                                waitFileInfoPackage.SwitchId();
                                waitFileInfoPackage.Package(byteBlock);
                                this.Send(TouchRpcUtility.P_1502_PushFile_Response, byteBlock);
                            }
                            else
                            {
                                waitFileInfoPackage.UnpackageBody(byteBlock);
                                ThreadPool.QueueUserWorkItem(RequestPushFile, waitFileInfoPackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_1502_PushFile_Response:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitTransferPackage waitTransferPackage = new WaitTransferPackage();
                            waitTransferPackage.UnpackageRouter(byteBlock);
                            if (IsService && waitTransferPackage.Route)
                            {
                                if (TryFindRpcActor(waitTransferPackage.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_1502_PushFile_Response, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                }
                            }
                            else
                            {
                                waitTransferPackage.UnpackageBody(byteBlock);
                                WaitHandlePool.SetRun(waitTransferPackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }

                        return;
                    }
                case TouchRpcUtility.P_509_PushFileAck_Request:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            WaitPushFileAckPackage waitPushFileAckPackage = new WaitPushFileAckPackage();
                            waitPushFileAckPackage.UnpackageRouter(byteBlock);
                            if (IsService && waitPushFileAckPackage.Route)
                            {
                                if (TryFindRpcActor(waitPushFileAckPackage.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_509_PushFileAck_Request, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                }
                            }
                            else
                            {
                                waitPushFileAckPackage.UnpackageBody(byteBlock);
                                m_eventArgs.TryAdd((int)waitPushFileAckPackage.Sign, waitPushFileAckPackage);

                                EasyTask.DelayRun(10000, waitPushFileAckPackage, (a) =>
                                {
                                    m_eventArgs.TryRemove((int)((WaitPushFileAckPackage)a).Sign, out _);
                                });
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_517_PullSmallFile_Request:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            var waitSmallFilePackage = new WaitSmallFilePackage();
                            waitSmallFilePackage.UnpackageRouter(byteBlock);
                            if (IsService && waitSmallFilePackage.Route)
                            {
                                if (this.TryRoute(RouteType.PullFile, waitSmallFilePackage))
                                {
                                    if (this.TryFindRpcActor(waitSmallFilePackage.TargetId, out RpcActor actor))
                                    {
                                        actor.Send(protocol, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                        return;
                                    }
                                    else
                                    {
                                        waitSmallFilePackage.UnpackageBody(byteBlock);
                                        waitSmallFilePackage.Status = TouchSocketStatus.ClientNotFind.ToValue();
                                    }
                                }
                                else
                                {
                                    waitSmallFilePackage.UnpackageBody(byteBlock);
                                    waitSmallFilePackage.Status = TouchSocketStatus.RoutingNotAllowed.ToValue();
                                }
                                byteBlock.Reset();
                                waitSmallFilePackage.SwitchId();
                                waitSmallFilePackage.Package(byteBlock);
                                Send(TouchRpcUtility.P_1517_PullSmallFile_Response, byteBlock);
                            }
                            else
                            {
                                waitSmallFilePackage.UnpackageBody(byteBlock);
                                ThreadPool.QueueUserWorkItem(RequestPullSmallFile, waitSmallFilePackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_1517_PullSmallFile_Response:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            var waitSmallFilePackage = new WaitSmallFilePackage();
                            waitSmallFilePackage.UnpackageRouter(byteBlock);
                            if (IsService && waitSmallFilePackage.Route)
                            {
                                if (this.TryFindRpcActor(waitSmallFilePackage.TargetId, out RpcActor actor))
                                {
                                    actor.Send(protocol, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                }
                            }
                            else
                            {
                                waitSmallFilePackage.UnpackageBody(byteBlock);
                                WaitHandlePool.SetRun(waitSmallFilePackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_518_PushSmallFile_Request:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            var waitSmallFilePackage = new WaitSmallFilePackage();
                            waitSmallFilePackage.UnpackageRouter(byteBlock);
                            if (IsService && waitSmallFilePackage.Route)
                            {
                                if (this.TryRoute(RouteType.PullFile, waitSmallFilePackage))
                                {
                                    if (this.TryFindRpcActor(waitSmallFilePackage.TargetId, out RpcActor actor))
                                    {
                                        actor.Send(protocol, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                        return;
                                    }
                                    else
                                    {
                                        waitSmallFilePackage.UnpackageBody(byteBlock);
                                        waitSmallFilePackage.Status = TouchSocketStatus.ClientNotFind.ToValue();
                                    }
                                }
                                else
                                {
                                    waitSmallFilePackage.UnpackageBody(byteBlock);
                                    waitSmallFilePackage.Status = TouchSocketStatus.RoutingNotAllowed.ToValue();
                                }
                                byteBlock.Reset();
                                waitSmallFilePackage.SwitchId();
                                waitSmallFilePackage.Package(byteBlock);
                                Send(TouchRpcUtility.P_1518_PushSmallFile_Response, byteBlock);
                            }
                            else
                            {
                                waitSmallFilePackage.UnpackageBody(byteBlock);
                                ThreadPool.QueueUserWorkItem(RequestPushSmallFile, waitSmallFilePackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }
                case TouchRpcUtility.P_1518_PushSmallFile_Response:
                    {
                        try
                        {
                            byteBlock.Pos = 2;
                            var waitSmallFilePackage = new WaitSmallFilePackage();
                            waitSmallFilePackage.UnpackageRouter(byteBlock);

                            if (IsService && waitSmallFilePackage.Route)
                            {
                                if (this.TryFindRpcActor(waitSmallFilePackage.TargetId, out RpcActor actor))
                                {
                                    actor.Send(TouchRpcUtility.P_1518_PushSmallFile_Response, byteBlock.Buffer, 2, byteBlock.Len - 2);
                                }
                            }
                            else
                            {
                                waitSmallFilePackage.UnpackageBody(byteBlock);
                                WaitHandlePool.SetRun(waitSmallFilePackage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        return;
                    }

                #endregion 500-599

                default:
                    {
                        try
                        {
                            OnReceived?.Invoke(this, protocol, byteBlock);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(this, $"在protocol={protocol}中发生错误。信息:{ex.Message}");
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 尝试获取指定Id的RpcActor。一般此方法仅在Service下有效。
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="rpcActor"></param>
        /// <returns></returns>
        public bool TryFindRpcActor(string targetId, out RpcActor rpcActor)
        {
            if (targetId == ID)
            {
                rpcActor = this;
                return true;
            }
            if (OnFindRpcActor?.Invoke(targetId) is RpcActor actor)
            {
                rpcActor = actor;
                return true;
            }

            rpcActor = default;
            return false;
        }

        /// <summary>
        /// 尝试请求路由，触发路由相关插件。
        /// </summary>
        /// <param name="routerType"></param>
        /// <param name="routerPackage"></param>
        /// <returns></returns>
        public bool TryRoute(RouteType routerType, RouterPackage routerPackage)
        {
            try
            {
                PackageRouterEventArgs args = new PackageRouterEventArgs(routerType, routerPackage);
                OnRouting?.Invoke(this, args);
                return args.IsPermitOperation;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试请求路由，触发路由相关插件。并在路由失败时向<see cref="MsgRouterPackage.Message"/>中传递消息。
        /// </summary>
        /// <param name="routerType"></param>
        /// <param name="routerPackage"></param>
        /// <returns></returns>
        public bool TryRoute(RouteType routerType, WaitRouterPackage routerPackage)
        {
            try
            {
                PackageRouterEventArgs args = new PackageRouterEventArgs(routerType, routerPackage);
                OnRouting?.Invoke(this, args);
                routerPackage.Message = args.Message;
                return args.IsPermitOperation;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Ping(int timeout = 5000)
        {
            return PrivatePing(default, timeout);
        }

        /// <inheritdoc/>
        public bool Ping(string targetId, int timeout = 5000)
        {
            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor rpcActor))
                {
                    return rpcActor.Ping(timeout);
                }
                return false;
            }
            return PrivatePing(targetId, timeout);
        }

        private bool PrivatePing(string targetId, int timeout)
        {
            try
            {
                WaitPingPackage waitPing = new WaitPingPackage
                {
                    TargetId = targetId,
                    SourceId = ID,
                    Route = targetId.HasValue()
                };
                var wait = WaitHandlePool.GetWaitData(waitPing);
                using (ByteBlock byteBlock = new ByteBlock())
                {
                    waitPing.Package(byteBlock);
                    Send(TouchRpcUtility.P_2_Ping_Request, byteBlock);
                }

                switch (wait.Wait(timeout))
                {
                    case WaitDataStatus.SetRunning:
                        {
                            switch (wait.WaitResult.Status.ToStatus())
                            {
                                case TouchSocketStatus.Success: return true;
                                default:
                                    return false;
                            }
                        }
                    case WaitDataStatus.Default:
                    case WaitDataStatus.Overtime:
                    case WaitDataStatus.Canceled:
                    case WaitDataStatus.Disposed:
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 重新设置ID,并且同步到对端
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="Exception"></exception>
        public void ResetID(string id)
        {
            WaitSetID waitSetID = new WaitSetID(ID, id);

            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(waitSetID);

            ByteBlock byteBlock = new ByteBlock();
            byteBlock.WriteObject(waitSetID);

            Send(TouchRpcUtility.P_1_ResetID_Request, byteBlock.Buffer, 0, byteBlock.Len);

            switch (waitData.Wait(5000))
            {
                case WaitDataStatus.SetRunning:
                    {
                        if (waitData.WaitResult.Status == 1)
                        {
                            ID = id;
                        }
                        else
                        {
                            throw new Exception(waitData.WaitResult.Message);
                        }
                        break;
                    }
                case WaitDataStatus.Overtime:
                    throw new TimeoutException(TouchSocketStatus.Overtime.GetDescription());
                case WaitDataStatus.Canceled:
                    break;

                case WaitDataStatus.Disposed:
                default:
                    throw new Exception(TouchSocketStatus.UnknownError.GetDescription());
            }
        }

        /// <summary>
        /// 以Fast序列化，发送小（64K）对象。接收方需要将Pos设为2，然后ReadObject即可。
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="obj"></param>
        public void SendFastObject(short protocol, object obj)
        {
            using ByteBlock byteBlock = new ByteBlock();
            byteBlock.WriteObject(obj, SerializationType.FastBinary);
            Send(protocol, byteBlock);
        }

        /// <summary>
        /// 以包发送小（64K）对象。接收方需要将Pos设为2，然后ReadPackage即可。
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="package"></param>
        public void SendPackage(short protocol, IPackage package)
        {
            using ByteBlock byteBlock = new ByteBlock();
            package.Package(byteBlock);
            Send(protocol, byteBlock);
        }

        #region 重写

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Caller = null;
            OnClose = null;
            OnFileTransfered = null;
            OnFileTransfering = null;
            OnRouting = null;
            OnFindRpcActor = null;
            OnHandshaked = null;
            OnHandshaking = null;
            OnReceived = null;
            OnResetID = null;
            OnStreamTransfered = null;
            OnStreamTransfering = null;
            OutputSend = null;
            WaitHandlePool.SafeDispose();
            Close(nameof(Dispose));
            base.Dispose(disposing);
        }

        #endregion 重写

        #region 协议同步发送

        /// <summary>
        /// 发送字节
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void Send(short protocol, byte[] buffer, int offset, int length)
        {
            ArraySegment<byte>[] transferBytes = new ArraySegment<byte>[]
           {
            new ArraySegment<byte>(ThingsGatewayBitConverter.Default.GetBytes(protocol)),
            new ArraySegment<byte>(buffer,offset,length)
           };
            OutputSend?.Invoke(this, transferBytes);
        }

        private void Send(short protocol, ByteBlock byteBlock)
        {
            Send(protocol, byteBlock.Buffer, 0, byteBlock.Len);
        }

        #endregion 协议同步发送

        #region 协议异步发送

        /// <summary>
        /// 异步发送字节
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public Task SendAsync(short protocol, byte[] buffer, int offset, int length)
        {
            return EasyTask.Run(() =>
             {
                 Send(protocol, buffer, offset, length);
             });
        }

        #endregion 协议异步发送
    }
}