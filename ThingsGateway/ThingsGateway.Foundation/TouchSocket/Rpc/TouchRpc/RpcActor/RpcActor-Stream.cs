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
    public partial class RpcActor
    {
        /// <summary>
        /// 发送流数据
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="streamOperator"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public Result SendStream(Stream stream, StreamOperator streamOperator, Metadata metadata = default)
        {
            WaitStream waitStream = new WaitStream();
            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(waitStream);
            waitStream.Metadata = metadata;
            long size = stream.Length - stream.Position;
            streamOperator.SetLength(size);
            waitStream.Size = size;
            waitStream.StreamType = stream.GetType().FullName;
            ByteBlock byteBlock = BytePool.GetByteBlock(TouchRpcUtility.TransferPackage)
                .WriteObject(waitStream);
            try
            {
                Send(TouchRpcUtility.P_400_SendStreamToSocketClient_Request, byteBlock);

                waitData.SetCancellationToken(streamOperator.Token);

                waitData.Wait(streamOperator.Timeout);

                switch (waitData.Status)
                {
                    case WaitDataStatus.SetRunning:
                        {
                            WaitStream waitStreamResult = (WaitStream)waitData.WaitResult;
                            if (waitStreamResult.Status == 1)
                            {
                                if (TrySubscribeChannel(waitStreamResult.ChannelID, out Channel channel))
                                {
                                    while (true)
                                    {
                                        if (streamOperator.Token.IsCancellationRequested)
                                        {
                                            channel.Cancel();
                                            return streamOperator.SetResult(new Result(ResultCode.Canceled));
                                        }
                                        int r = stream.Read(byteBlock.Buffer, 0,
                                            (int)Math.Min(TouchRpcUtility.TransferPackage, streamOperator.MaxSpeed / 10.0));
                                        if (r <= 0)
                                        {
                                            channel.Complete();
                                            return streamOperator.SetResult(new Result(ResultCode.Success));
                                        }

                                        channel.Write(byteBlock.Buffer, 0, r);
                                        streamOperator.AddFlow(r);
                                    }
                                }
                                else
                                {
                                    return streamOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.SetChannelFail.GetDescription()));
                                }
                            }
                            else if (waitStreamResult.Status == 2)
                            {
                                return streamOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.RemoteRefuse.GetDescription(waitStreamResult.Message)));
                            }
                            else if (waitStreamResult.Status == 3)
                            {
                                return streamOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.Exception.GetDescription(waitStreamResult.Message)));
                            }
                            else
                            {
                                return streamOperator.SetResult(new Result(ResultCode.Error));
                            }
                        }
                    case WaitDataStatus.Overtime:
                        {
                            return streamOperator.SetResult(new Result(ResultCode.Overtime));
                        }
                    case WaitDataStatus.Canceled:
                        {
                            return streamOperator.SetResult(new Result(ResultCode.Canceled));
                        }
                    case WaitDataStatus.Disposed:
                    default:
                        {
                            return streamOperator.SetResult(new Result(ResultCode.Error));
                        }
                }
            }
            catch (Exception ex)
            {
                return streamOperator.SetResult(new Result(ResultCode.Error, ex.Message));
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        /// <summary>
        /// 发送流数据
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="streamOperator"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public Task<Result> SendStreamAsync(Stream stream, StreamOperator streamOperator, Metadata metadata = default)
        {
            return EasyTask.Run(() =>
            {
                return SendStream(stream, streamOperator, metadata);
            });
        }

        private void RequestStreamToClient(object obj)
        {
            try
            {
                WaitStream waitStream = (WaitStream)obj;
                StreamOperator streamOperator = new StreamOperator();
                StreamInfo streamInfo = new StreamInfo(waitStream.Size, waitStream.StreamType);
                StreamOperationEventArgs args = new StreamOperationEventArgs(streamOperator, waitStream.Metadata, streamInfo);

                try
                {
                    OnStreamTransfering?.Invoke(this, args);

                    if (args.IsPermitOperation)
                    {
                        if (args.Bucket != null)
                        {
                            waitStream.Status = 1;
                            Channel channel = CreateChannel();
                            waitStream.ChannelID = channel.ID;
                            TaskTransferingStream(streamOperator, args, channel, waitStream.Size);
                        }
                        else
                        {
                            streamOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.StreamBucketNull.GetDescription()));
                            waitStream.Status = 3;
                            waitStream.Message = "未设置流容器";

                            OnStreamTransfered?.Invoke(this, new StreamStatusEventArgs(new Result(ResultCode.Error, waitStream.Message), waitStream.Metadata, streamInfo)
                            { Message = args.Message });
                        }
                    }
                    else
                    {
                        streamOperator.SetResult(new Result(ResultCode.Error, args.Message));
                        waitStream.Status = 2;
                        waitStream.Message = args.Message;

                        OnStreamTransfered?.Invoke(this, new StreamStatusEventArgs(new Result(ResultCode.Error, TouchSocketStatus.RemoteRefuse.GetDescription(waitStream.Message)), waitStream.Metadata, streamInfo)
                        { Message = args.Message });
                    }
                }
                catch (Exception ex)
                {
                    streamOperator.SetResult(new Result(ResultCode.Error, ex.Message));
                    waitStream.Status = 3;
                    waitStream.Message = ex.Message;

                    Logger.Log(LogType.Error, this, $"在{nameof(RequestStreamToClient)}中发生错误。", ex);
                }

                waitStream.Metadata = null;
                using (ByteBlock byteBlock = new ByteBlock().WriteObject(waitStream))
                {
                    Send(TouchRpcUtility.P_401_SendStreamToClient, byteBlock);
                }
            }
            catch
            {
            }
        }

        private void RequestStreamToSocketClient(object obj)
        {
            try
            {
                WaitStream waitStream = (WaitStream)obj;
                StreamOperator streamOperator = new StreamOperator();
                StreamInfo streamInfo = new StreamInfo(waitStream.Size, waitStream.StreamType);
                StreamOperationEventArgs args = new StreamOperationEventArgs(streamOperator, waitStream.Metadata, streamInfo);

                try
                {
                    OnStreamTransfering?.Invoke(this, args);
                    if (args.IsPermitOperation)
                    {
                        if (args.Bucket != null)
                        {
                            waitStream.Status = 1;
                            Channel channel = CreateChannel();
                            waitStream.ChannelID = channel.ID;
                            TaskTransferingStream(streamOperator, args, channel, waitStream.Size);
                        }
                        else
                        {
                            streamOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.StreamBucketNull.GetDescription()));
                            waitStream.Status = 3;
                            waitStream.Message = "未设置流容器";

                            OnStreamTransfered?.Invoke(this, new StreamStatusEventArgs(new Result(ResultCode.Error, waitStream.Message), waitStream.Metadata, streamInfo)
                            { Message = args.Message });
                        }
                    }
                    else
                    {
                        streamOperator.SetResult(new Result(ResultCode.Error, args.Message));
                        waitStream.Status = 2;
                        waitStream.Message = args.Message;

                        OnStreamTransfered?.Invoke(this, new StreamStatusEventArgs(new Result(ResultCode.Error, TouchSocketStatus.RemoteRefuse.GetDescription(waitStream.Message)), waitStream.Metadata, streamInfo)
                        { Message = args.Message });
                    }
                }
                catch (Exception ex)
                {
                    streamOperator.SetResult(new Result(ResultCode.Error, ex.Message));
                    waitStream.Status = 3;
                    waitStream.Message = ex.Message;

                    Logger.Log(LogType.Error, this, $"在{nameof(RequestStreamToSocketClient)}中发生错误。", ex);
                }

                waitStream.Metadata = null;
                using (ByteBlock byteBlock = new ByteBlock().WriteObject(waitStream))
                {
                    Send(TouchRpcUtility.P_1400_SendStreamToSocketClient_Response, byteBlock);
                }
            }
            catch
            {
            }
        }

        private void TaskTransferingStream(StreamOperator streamOperator, StreamOperationEventArgs args, Channel channel, long size)
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                try
                {
                    streamOperator.SetLength(size);
                    Stream stream = args.Bucket;
                    while (channel.MoveNext())
                    {
                        if (streamOperator.Token.IsCancellationRequested)
                        {
                            channel.Cancel();
                            streamOperator.SetResult(new Result(ResultCode.Canceled));
                            break;
                        }
                        var data = channel.GetCurrent();
                        if (data != null)
                        {
                            stream.Write(data, 0, data.Length);
                            streamOperator.AddFlow(data.Length);
                        }
                    }
                    OnStreamTransfered?.Invoke(this, new StreamStatusEventArgs(streamOperator.SetResult(new Result(channel.Status.ToResultCode())),
                        args.Metadata, args.StreamInfo)
                    { Bucket = stream, Message = args.Message });
                }
                catch
                {
                }
            });
        }
    }
}