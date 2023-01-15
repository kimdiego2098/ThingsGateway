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
using System.IO;
using System.Threading
.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    public partial class RpcActor
    {
        private readonly ConcurrentDictionary<int, object> m_eventArgs;
        private string m_rootPath = string.Empty;

        /// <inheritdoc/>
        public string RootPath
        {
            get => m_rootPath;
            set
            {
                value ??= string.Empty;
                m_rootPath = value;
            }
        }

        #region Pull

        /// <inheritdoc/>
        public Result PullFile(FileOperator fileOperator)
        {
            return this.PrivatePullFile(default, fileOperator);
        }

        /// <inheritdoc/>
        public Result PullFile(string targetId, FileOperator fileOperator)
        {
            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor rpcActor))
                {
                    return rpcActor.PullFile(fileOperator);
                }
                return new Result(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription());
            }

            return this.PrivatePullFile(targetId, fileOperator);
        }

        /// <inheritdoc/>
        public Task<Result> PullFileAsync(string targetId, FileOperator fileOperator)
        {
            return EasyTask.Run(() =>
            {
                return PullFile(targetId, fileOperator);
            });
        }

        /// <inheritdoc/>
        public Task<Result> PullFileAsync(FileOperator fileOperator)
        {
            return EasyTask.Run(() =>
            {
                return PullFile(fileOperator);
            });
        }

        private Result PreviewPullFile(string targetId, FileOperator fileOperator, WaitFileInfoPackage waitFileInfoPackage)
        {
            TouchRpcFileInfo fileInfo = waitFileInfoPackage.FileInfo;
            fileOperator.SavePath = FileController.GetFullPath(m_rootPath, fileOperator.SavePath);
            TouchRpcFileStream stream;
            try
            {
                FileController.TryReadTempInfo(fileOperator.SavePath, fileOperator.Flags, ref fileInfo);
                stream = TouchRpcFileStream.Create(fileOperator.SavePath, ref fileInfo, fileOperator.Flags.HasFlag(TransferFlags.BreakpointResume));
            }
            catch (Exception ex)
            {
                return fileOperator.SetResult(new Result(ResultCode.Error, ex.Message));
            }

            WaitTransferPackage waitTransfer = new WaitTransferPackage()
            {
                EventHashCode = waitFileInfoPackage.EventHashCode,
                Path = fileInfo.FullName,
                Position = fileInfo.Position,
                SourceId = this.ID,
                TargetId = targetId,
                Route = targetId.HasValue()
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(waitTransfer);
            ByteBlock byteBlock = new ByteBlock();
            Channel channel = default;
            try
            {
                if (targetId == null)
                {
                    channel = CreateChannel();
                }
                else
                {
                    channel = CreateChannel(targetId);
                }
                waitTransfer.ChannelID = channel.ID;
                waitTransfer.Package(byteBlock);
                Send(TouchRpcUtility.P_501_BeginPullFile_Request, byteBlock);

                if (fileOperator.Token.IsCancellationRequested)
                {
                    waitData.SetCancellationToken(fileOperator.Token);
                }

                switch (waitData.Wait(fileOperator.Timeout))
                {
                    case WaitDataStatus.SetRunning:
                        {
                            WaitTransferPackage waitTransferResult = (WaitTransferPackage)waitData.WaitResult;
                            switch (waitTransferResult.Status.ToStatus())
                            {
                                case TouchSocketStatus.Success:
                                    {
                                        fileOperator.SetFileCompletedLength(fileInfo.Position);
                                        fileOperator.SetLength(fileInfo.Length);
                                        channel.Timeout = fileOperator.Timeout;
                                        while (channel.MoveNext())
                                        {
                                            if (fileOperator.Token.IsCancellationRequested)
                                            {
                                                channel.Cancel();
                                                fileOperator.SetResult(new Result(ResultCode.Canceled));
                                                break;
                                            }
                                            byte[] data = channel.GetCurrent();
                                            if (data != null)
                                            {
                                                stream.Write(data, 0, data.Length);
                                                fileInfo.Position = stream.FileWriter.Position;
                                                fileOperator.AddFlow(data.Length);
                                            }
                                        }

                                        if (channel.Status == ChannelStatus.Completed)
                                        {
                                            stream.FinishStream();
                                            return fileOperator.SetResult(new Result(ResultCode.Success));
                                        }
                                        else
                                        {
                                            return fileOperator.SetResult(new Result(channel.Status.ToResultCode()));
                                        }
                                    }
                                case TouchSocketStatus.LoadStreamFail:
                                    return fileOperator.SetResult(new Result(ResultCode.Fail, TouchSocketStatus.LoadStreamFail.GetDescription(fileInfo.FullName)));

                                default:
                                    return fileOperator.SetResult(new Result(ResultCode.Error, waitTransferResult.Status.ToStatus().GetDescription(waitTransferResult.Message)));
                            }
                        }
                    case WaitDataStatus.Overtime:
                        {
                            return fileOperator.SetResult(new Result(ResultCode.Overtime));
                        }
                    case WaitDataStatus.Canceled:
                        {
                            return fileOperator.SetResult(new Result(ResultCode.Canceled));
                        }
                    case WaitDataStatus.Disposed:
                    default:
                        {
                            return fileOperator.SetResult(new Result(ResultCode.Error));
                        }
                }
            }
            catch (Exception ex)
            {
                return fileOperator.SetResult(new Result(ResultCode.Error, ex.Message));
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                stream.SafeDispose();
                byteBlock.Dispose();
                channel.SafeDispose();
            }
        }

        private Result PrivatePullFile(string targetId, FileOperator fileOperator)
        {
            if (fileOperator is null)
            {
                return new Result(ResultCode.Error, TouchSocketStatus.ArgumentNull.GetDescription(nameof(fileOperator)));
            }

            WaitFileInfoPackage waitFileInfo = new WaitFileInfoPackage
            {
                SourceId = this.ID,
                TargetId = targetId,
                Route = targetId.HasValue(),
                Metadata = fileOperator.Metadata,
                SavePath = this.FileController.GetFullPath(this.m_rootPath, fileOperator.SavePath),
                Flags = fileOperator.Flags,
                ResourcePath = fileOperator.ResourcePath
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(waitFileInfo);

            ByteBlock byteBlock = new ByteBlock();
            try
            {
                waitFileInfo.Package(byteBlock);
                Send(TouchRpcUtility.P_500_PullFile_Request, byteBlock);

                if (fileOperator.Token.IsCancellationRequested)
                {
                    waitData.SetCancellationToken(fileOperator.Token);
                }

                waitData.Wait(fileOperator.Timeout);

                switch (waitData.Status)
                {
                    case WaitDataStatus.SetRunning:
                        {
                            WaitFileInfoPackage waitFileResult = (WaitFileInfoPackage)waitData.WaitResult;
                            switch (waitFileResult.Status.ToStatus())
                            {
                                case TouchSocketStatus.Success:
                                    return PreviewPullFile(targetId, fileOperator, waitFileResult);

                                case TouchSocketStatus.FileNotExists:
                                    return fileOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.FileNotExists.GetDescription(waitFileResult.ResourcePath)));

                                case TouchSocketStatus.RemoteRefuse:
                                default:
                                    return fileOperator.SetResult(new Result(ResultCode.Fail, waitFileResult.Status.ToStatus().GetDescription(waitFileResult.Message)));
                            }
                        }
                    case WaitDataStatus.Overtime:
                        {
                            return fileOperator.SetResult(Result.Overtime);
                        }
                    case WaitDataStatus.Canceled:
                        {
                            return fileOperator.SetResult(Result.Canceled);
                        }
                    case WaitDataStatus.Disposed:
                    default:
                        {
                            return fileOperator.SetResult(Result.UnknownFail);
                        }
                }
            }
            catch (Exception ex)
            {
                return fileOperator.SetResult(new Result(ex));
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        #endregion Pull

        #region Push

        /// <inheritdoc/>
        public Result PushFile(FileOperator fileOperator)
        {
            return PrivatePushFile(default, fileOperator);
        }

        /// <inheritdoc/>
        public Result PushFile(string targetId, FileOperator fileOperator)
        {
            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor rpcActor))
                {
                    return rpcActor.PushFile(fileOperator);
                }
                return new Result(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription());
            }
            return this.PrivatePushFile(default, fileOperator);
        }

        /// <inheritdoc/>
        public Task<Result> PushFileAsync(string targetId, FileOperator fileOperator)
        {
            return EasyTask.Run(() =>
            {
                return PushFile(targetId, fileOperator);
            });
        }

        /// <inheritdoc/>
        public Task<Result> PushFileAsync(FileOperator fileOperator)
        {
            return EasyTask.Run(() =>
            {
                return PushFile(fileOperator);
            });
        }

        private Result PreviewPushFile(FileOperator fileOperator, WaitTransferPackage waitTransfer)
        {
            try
            {
                using FileStorageReader reader = FilePool.GetReader(waitTransfer.Path);
                fileOperator.SetLength(reader.FileStorage.FileInfo.Length);
                if (TrySubscribeChannel(waitTransfer.ChannelID, out Channel channel))
                {
                    ByteBlock byteBlock = BytePool.GetByteBlock(TouchRpcUtility.TransferPackage);
                    try
                    {
                        long position = waitTransfer.Position;
                        reader.Position = position;
                        fileOperator.SetFileCompletedLength(waitTransfer.Position);
                        channel.Timeout = fileOperator.Timeout;
                        while (true)
                        {
                            if (fileOperator.Token.IsCancellationRequested)
                            {
                                channel.Cancel("主动取消");
                                fileOperator.SetResult(new Result(ResultCode.Canceled));
                                break;
                            }
                            int r = reader.Read(byteBlock.Buffer, 0, (int)Math.Min(TouchRpcUtility.TransferPackage, fileOperator.MaxSpeed / 10.0));
                            if (r == 0)
                            {
                                channel.Complete();
                                WaitPushFileAckPackage waitResult = null;
                                if (SpinWait.SpinUntil(() =>
                                {
                                    if (m_eventArgs.TryRemove(waitTransfer.EventHashCode, out object obj))
                                    {
                                        waitResult = (WaitPushFileAckPackage)obj;
                                        return true;
                                    }
                                    return false;
                                }, fileOperator.Timeout))
                                {
                                    if (waitResult.Status.ToStatus() == TouchSocketStatus.Success)
                                    {
                                        return fileOperator.SetResult(new Result(ResultCode.Success));
                                    }
                                    else
                                    {
                                        return fileOperator.SetResult(new Result(ResultCode.Error, waitResult.Message));
                                    }
                                }
                                else
                                {
                                    return fileOperator.SetResult(new Result(ResultCode.Overtime, "等待最后状态确认超时。"));
                                }
                            }
                            else
                            {
                                position += r;
                                if (channel.TryWrite(byteBlock.Buffer, 0, r))
                                {
                                    fileOperator.AddFlow(r);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (channel.Status == ChannelStatus.Cancel && !string.IsNullOrEmpty(channel.LastOperationMes))
                        {
                            return fileOperator.SetResult(new Result(ResultCode.Canceled, channel.LastOperationMes));
                        }
                        else
                        {
                            return fileOperator.SetResult(new Result(channel.Status.ToResultCode()));
                        }
                    }
                    catch (Exception ex)
                    {
                        return fileOperator.SetResult(new Result(ResultCode.Error, ex.Message));
                    }
                    finally
                    {
                        byteBlock.Dispose();
                    }
                }
                else
                {
                    return fileOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.SetChannelFail.GetDescription()));
                }
            }
            catch (Exception ex)
            {
                return fileOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.LoadStreamFail.GetDescription(waitTransfer.Path, ex.Message)));
            }
        }

        private Result PrivatePushFile(string targetId, FileOperator fileOperator)
        {
            if (fileOperator is null)
            {
                return fileOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.ArgumentNull.GetDescription(nameof(fileOperator))));
            }

            string fullPath = FileController.GetFullPath(m_rootPath, fileOperator.ResourcePath);

            if (!File.Exists(fullPath))
            {
                return fileOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.FileNotExists.GetDescription(fullPath)));
            }

            TouchRpcFileInfo fileInfo = new TouchRpcFileInfo();
            try
            {
                FileController.GetFileInfo(fullPath, fileOperator.Flags.HasFlag(TransferFlags.MD5Verify), ref fileInfo);
            }
            catch (Exception ex)
            {
                return fileOperator.SetResult(new Result(ResultCode.Error, ex.Message));
            }

            WaitFileInfoPackage waitFileInfoPackage = new WaitFileInfoPackage()
            {
                FileInfo = fileInfo,
                Metadata = fileOperator.Metadata,
                ResourcePath = this.FileController.GetFullPath(this.m_rootPath, fileOperator.ResourcePath),
                SavePath = fileOperator.SavePath,
                Flags = fileOperator.Flags,
                TargetId = targetId,
                SourceId = this.ID,
                Route = targetId.HasValue()
            };
            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(waitFileInfoPackage);

            try
            {
                SendPackage(TouchRpcUtility.P_502_PushFile_Request, waitFileInfoPackage);
                waitData.SetCancellationToken(fileOperator.Token);

                waitData.Wait(fileOperator.Timeout);

                switch (waitData.Status)
                {
                    case WaitDataStatus.SetRunning:
                        {
                            WaitTransferPackage waitResult = (WaitTransferPackage)waitData.WaitResult;
                            switch (waitResult.Status.ToStatus())
                            {
                                case TouchSocketStatus.Success:
                                    return PreviewPushFile(fileOperator, waitResult);

                                case TouchSocketStatus.LoadStreamFail:
                                    {
                                        return fileOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.LoadStreamFail.GetDescription(waitResult.Path, waitResult.Message)));
                                    }
                                case TouchSocketStatus.ClientNotFind:
                                    {
                                        return fileOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription(targetId)));
                                    }
                                case TouchSocketStatus.DirectoryNotExists:
                                    {
                                        return fileOperator.SetResult(new Result(ResultCode.Error, TouchSocketStatus.DirectoryNotExists.GetDescription(waitResult.Path)));
                                    }
                                case TouchSocketStatus.RemoteRefuse:
                                default:
                                    return fileOperator.SetResult(new Result(ResultCode.Error, waitResult.Status.ToStatus().GetDescription(waitResult.Message)));
                            }
                        }
                    case WaitDataStatus.Overtime:
                        {
                            return fileOperator.SetResult(new Result(ResultCode.Overtime));
                        }
                    case WaitDataStatus.Canceled:
                        {
                            return fileOperator.SetResult(new Result(ResultCode.Canceled));
                        }
                    case WaitDataStatus.Disposed:
                    default:
                        {
                            return fileOperator.SetResult(new Result(ResultCode.Error));
                        }
                }
            }
            catch (Exception ex)
            {
                return fileOperator.SetResult(new Result(ex));
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
            }
        }

        #endregion Push

        #region 小文件传输

        /// <summary>
        /// 允许传输的小文件的最大长度。默认1024*1024字节。
        /// <para>注意，当调整该值时，应该和对端保持一致。</para>
        /// </summary>
        public int MaxSmallFileLength { get; set; } = 1024 * 1024;

        /// <inheritdoc/>
        public PullSmallFileResult PullSmallFile(string targetId, string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor actor))
                {
                    return actor.PullSmallFile(path, metadata, timeout, token);
                }
                return new PullSmallFileResult(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription());
            }

            return PrivatePullSmallFile(targetId, path, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public PullSmallFileResult PullSmallFile(string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return PrivatePullSmallFile(default, path, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public Task<PullSmallFileResult> PullSmallFileAsync(string targetId, string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                return PullSmallFile(targetId, path, metadata, timeout, token);
            });
        }

        /// <inheritdoc/>
        public Task<PullSmallFileResult> PullSmallFileAsync(string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                return PullSmallFile(path, metadata, timeout, token);
            });
        }

        /// <inheritdoc/>
        public Result PushSmallFile(string targetId, string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor rpcActor))
                {
                    return rpcActor.PushSmallFile(savePath, fileInfo, metadata, timeout, token);
                }
                return new Result(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription());
            }
            return PrivatePushSmallFile(targetId, savePath, fileInfo, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public Result PushSmallFile(string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return PrivatePushSmallFile(default, savePath, fileInfo, metadata, timeout, token);
        }

        /// <inheritdoc/>
        public Task<Result> PushSmallFileAsync(string targetId, string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                return PushSmallFile(targetId, savePath, fileInfo, metadata, timeout, token);
            });
        }

        /// <inheritdoc/>
        public Task<Result> PushSmallFileAsync(string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                return PushSmallFile(savePath, fileInfo, metadata, timeout, token);
            });
        }

        private PullSmallFileResult PrivatePullSmallFile(string targetId, string path, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            WaitSmallFilePackage waitSmallFilePackage = new WaitSmallFilePackage()
            {
                Path = path,
                SourceId = ID,
                TargetId = targetId,
                Route = targetId.HasValue(),
                Metadata = metadata
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(waitSmallFilePackage);

            ByteBlock byteBlock = new ByteBlock();
            try
            {
                waitSmallFilePackage.Package(byteBlock);
                Send(TouchRpcUtility.P_517_PullSmallFile_Request, byteBlock);
                waitData.SetCancellationToken(token);

                waitData.Wait(timeout);

                switch (waitData.Status)
                {
                    case WaitDataStatus.SetRunning:
                        {
                            WaitSmallFilePackage waitFile = (WaitSmallFilePackage)waitData.WaitResult;
                            switch (waitFile.Status.ToStatus())
                            {
                                case TouchSocketStatus.Success:
                                    {
                                        return new PullSmallFileResult(waitFile.Data);
                                    }
                                case TouchSocketStatus.FileNotExists:
                                    {
                                        return new PullSmallFileResult(ResultCode.Error, TouchSocketStatus.FileNotExists.GetDescription(waitFile.Path));
                                    }
                                case TouchSocketStatus.RemoteRefuse:
                                case TouchSocketStatus.LengthErrorWhenRead:
                                case TouchSocketStatus.FileLengthTooLong:
                                case TouchSocketStatus.ClientNotFind:
                                default:
                                    {
                                        return new PullSmallFileResult(ResultCode.Error, TouchSocketStatus.RemoteRefuse.GetDescription(waitFile.Message));
                                    }
                            }
                        }
                    case WaitDataStatus.Overtime:
                        {
                            return new PullSmallFileResult(ResultCode.Overtime);
                        }
                    case WaitDataStatus.Canceled:
                        {
                            return new PullSmallFileResult(ResultCode.Canceled);
                        }
                    case WaitDataStatus.Disposed:
                    default:
                        {
                            return new PullSmallFileResult(ResultCode.Error, TouchSocketStatus.UnknownError.GetDescription());
                        }
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        private Result PrivatePushSmallFile(string targetId, string savePath, FileInfo fileInfo, Metadata metadata = null, int timeout = 5000, CancellationToken token = default)
        {
            if (!File.Exists(fileInfo.FullName))
            {
                return new Result(ResultCode.Error, TouchSocketStatus.FileNotExists.GetDescription(fileInfo.FullName));
            }
            if (fileInfo.Length > MaxSmallFileLength)
            {
                return new Result(ResultCode.Error, TouchSocketStatus.FileLengthTooLong.GetDescription());
            }
            WaitSmallFilePackage waitSmallFilePackage = new WaitSmallFilePackage()
            {
                Path = savePath,
                SourceId = ID,
                Metadata = metadata,
                Route = targetId.HasValue(),
                TargetId = targetId,
                FileInfo = new RemoteFileInfo(fileInfo)
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(waitSmallFilePackage);

            ByteBlock byteBlock = new ByteBlock();
            byte[] buffer = BytePool.GetByteCore((int)fileInfo.Length);
            try
            {
                int r = FileController.ReadAllBytes(fileInfo, buffer);
                if (r <= 0)
                {
                    return new Result(ResultCode.Error, TouchSocketStatus.LengthErrorWhenRead.GetDescription());
                }
                waitSmallFilePackage.Data = buffer;
                waitSmallFilePackage.Len = r;

                waitSmallFilePackage.Package(byteBlock);
                Send(TouchRpcUtility.P_518_PushSmallFile_Request, byteBlock);
                waitData.SetCancellationToken(token);

                waitData.Wait(timeout);

                switch (waitData.Status)
                {
                    case WaitDataStatus.SetRunning:
                        {
                            WaitSmallFilePackage waitFile = (WaitSmallFilePackage)waitData.WaitResult;
                            switch (waitFile.Status.ToStatus())
                            {
                                case TouchSocketStatus.Success:
                                    {
                                        return Result.Success;
                                    }
                                case TouchSocketStatus.RemoteRefuse:
                                    {
                                        return new Result(ResultCode.Error, TouchSocketStatus.RemoteRefuse.GetDescription(waitFile.Message));
                                    }
                                case TouchSocketStatus.ClientNotFind:
                                    {
                                        return new Result(ResultCode.Error, TouchSocketStatus.ClientNotFind.GetDescription());
                                    }
                                default:
                                    return new Result(ResultCode.Exception, waitFile.Message);
                            }
                        }
                    case WaitDataStatus.Overtime:
                        {
                            return Result.Overtime;
                        }
                    case WaitDataStatus.Canceled:
                        {
                            return Result.Canceled;
                        }
                    case WaitDataStatus.Disposed:
                    default:
                        {
                            return Result.UnknownFail;
                        }
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
                BytePool.Recycle(buffer);
            }
        }

        private void RequestPullSmallFile(object o)
        {
            //2.不响应
            //3.不允许
            //4.不存在
            //5.读取文件长度异常

            byte[] buffer = BytePool.GetByteCore(MaxSmallFileLength);
            try
            {
                WaitSmallFilePackage waitSmallFilePackage = (WaitSmallFilePackage)o;
                Result resultThis = Result.Success;
                try
                {
                    FileOperationEventArgs args = new FileOperationEventArgs(TransferType.SmallPull,
                        waitSmallFilePackage.Metadata, default)
                    {
                        ResourcePath = waitSmallFilePackage.Path
                    };

                    OnFileTransfering?.Invoke(this, args);

                    string fullPath = FileController.GetFullPath(RootPath, args.ResourcePath);
                    waitSmallFilePackage.Path = fullPath;
                    if (args.IsPermitOperation)
                    {
                        if (File.Exists(fullPath))
                        {
                            FileInfo fileInfo = new FileInfo(fullPath);
                            if (fileInfo.Length > MaxSmallFileLength)
                            {
                                waitSmallFilePackage.Status = TouchSocketStatus.FileLengthTooLong.ToValue();
                                resultThis = new Result(ResultCode.Error, TouchSocketStatus.FileLengthTooLong.GetDescription());
                            }
                            else
                            {
                                int r = FileController.ReadAllBytes(fileInfo, buffer);
                                if (r > 0)
                                {
                                    waitSmallFilePackage.Data = buffer;
                                    waitSmallFilePackage.Len = r;
                                    waitSmallFilePackage.FileInfo = new RemoteFileInfo(fileInfo);
                                    waitSmallFilePackage.Status = TouchSocketStatus.Success.ToValue();
                                }
                                else
                                {
                                    waitSmallFilePackage.Status = TouchSocketStatus.LengthErrorWhenRead.ToValue();
                                    resultThis = new Result(ResultCode.Error, TouchSocketStatus.LengthErrorWhenRead.GetDescription());
                                }
                            }
                        }
                        else
                        {
                            waitSmallFilePackage.Status = TouchSocketStatus.FileNotExists.ToValue();
                            resultThis = new Result(ResultCode.Error, TouchSocketStatus.FileNotExists.GetDescription(fullPath));
                        }
                    }
                    else
                    {
                        waitSmallFilePackage.Status = TouchSocketStatus.RemoteRefuse.ToValue();
                        waitSmallFilePackage.Message = args.Message;
                        resultThis = new Result(ResultCode.Error, TouchSocketStatus.RemoteRefuse.GetDescription(args.Message));
                    }
                }
                catch (Exception ex)
                {
                    waitSmallFilePackage.Status = TouchSocketStatus.Exception.ToValue();
                    waitSmallFilePackage.Message = ex.Message;
                }

                using (ByteBlock byteBlock = new ByteBlock())
                {
                    waitSmallFilePackage.SwitchId();
                    waitSmallFilePackage.Package(byteBlock);
                    Send(TouchRpcUtility.P_1517_PullSmallFile_Response, byteBlock);
                }

                FileTransferStatusEventArgs resultArgs = new FileTransferStatusEventArgs(
                    TransferType.SmallPull, waitSmallFilePackage.Metadata, waitSmallFilePackage.FileInfo, resultThis)
                {
                    ResourcePath = waitSmallFilePackage.Path
                };
                OnFileTransfered?.Invoke(this, resultArgs);
            }
            catch
            {
            }
            finally
            {
                BytePool.Recycle(buffer);
            }
        }

        private void RequestPushSmallFile(object o)
        {
            //2.不响应
            //3.不允许

            try
            {
                WaitSmallFilePackage waitSmallFilePackage = (WaitSmallFilePackage)o;
                Result resultThis = Result.Success;
                try
                {
                    FileOperationEventArgs args = new FileOperationEventArgs(TransferType.SmallPush,
                        waitSmallFilePackage.Metadata, waitSmallFilePackage.FileInfo)
                    {
                        SavePath = waitSmallFilePackage.Path
                    };

                    OnFileTransfering?.Invoke(this, args);

                    string fullPath = FileController.GetFullPath(RootPath, args.SavePath);
                    waitSmallFilePackage.Path = fullPath;
                    if (args.IsPermitOperation)
                    {
                        FileInfo fileInfo = new FileInfo(fullPath);
                        FileController.WriteAllBytes(fileInfo, waitSmallFilePackage.Data, 0, waitSmallFilePackage.Data.Length);
                        waitSmallFilePackage.Status = TouchSocketStatus.Success.ToValue();
                    }
                    else
                    {
                        waitSmallFilePackage.Status = TouchSocketStatus.RemoteRefuse.ToValue();
                        waitSmallFilePackage.Message = args.Message;
                        resultThis = new Result(ResultCode.Error, TouchSocketStatus.RemoteRefuse.GetDescription(args.Message));
                    }
                }
                catch (Exception ex)
                {
                    waitSmallFilePackage.Status = TouchSocketStatus.Exception.ToValue();
                    waitSmallFilePackage.Message = ex.Message;
                }
                waitSmallFilePackage.FileInfo = default;
                waitSmallFilePackage.Data = default;
                waitSmallFilePackage.SwitchId();
                using (ByteBlock byteBlock = new ByteBlock())
                {
                    waitSmallFilePackage.Package(byteBlock);
                    Send(TouchRpcUtility.P_1518_PushSmallFile_Response, byteBlock);
                }

                FileTransferStatusEventArgs resultArgs = new FileTransferStatusEventArgs(
                    TransferType.SmallPush, waitSmallFilePackage.Metadata, waitSmallFilePackage.FileInfo, resultThis)
                {
                    SavePath = waitSmallFilePackage.Path
                };
                OnFileTransfered?.Invoke(this, resultArgs);
            }
            catch
            {
            }
        }

        #endregion 小文件传输

        private void BeginPullFile(object o)
        {
            try
            {
                WaitTransferPackage waitTransferPackage = (WaitTransferPackage)o;
                FileTransferStatusEventArgs e;
                if (m_eventArgs.TryRemove(waitTransferPackage.EventHashCode, out object obj) && obj is FileOperationEventArgs args)
                {
                    try
                    {
                        using (FileStorageReader reader = FilePool.GetReader(waitTransferPackage.Path))
                        {
                            if (TrySubscribeChannel(waitTransferPackage.ChannelID, out Channel channel))
                            {
                                ByteBlock byteBlock = BytePool.GetByteBlock(TouchRpcUtility.TransferPackage);
                                FileOperator fileOperator = args.FileOperator;
                                fileOperator.SetLength(reader.FileStorage.FileInfo.Length);
                                try
                                {
                                    waitTransferPackage.Status = TouchSocketStatus.Success.ToValue();
                                    waitTransferPackage.SwitchId();
                                    SendPackage(TouchRpcUtility.P_1501_BeginPullFile_Response, waitTransferPackage);
                                    long position = waitTransferPackage.Position;
                                    reader.Position = position;
                                    fileOperator.SetFileCompletedLength(position);
                                    while (true)
                                    {
                                        if (fileOperator.Token.IsCancellationRequested)
                                        {
                                            channel.Cancel("主动取消");
                                            fileOperator.SetResult(new Result(ResultCode.Canceled));
                                            break;
                                        }
                                        int r = reader.Read(byteBlock.Buffer, 0,
                                            (int)Math.Min(TouchRpcUtility.TransferPackage, fileOperator.MaxSpeed / 10.0));
                                        if (r == 0)
                                        {
                                            channel.Complete();
                                            break;
                                        }
                                        else
                                        {
                                            position += r;
                                            if (channel.TryWrite(byteBlock.Buffer, 0, r))
                                            {
                                                fileOperator.AddFlow(r);
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    byteBlock.Dispose();

                                    e = new FileTransferStatusEventArgs(
                                       fileOperator.SetResult(new Result(channel.Status.ToResultCode())), args);
                                    OnFileTransfered?.Invoke(this, e);
                                }

                                return;
                            }
                            else
                            {
                                waitTransferPackage.Status = TouchSocketStatus.SetChannelFail.ToValue();
                                e = new FileTransferStatusEventArgs(new Result(ResultCode.Error, StringResStore.GetDescription(TouchSocketStatus.SetChannelFail)), args);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        waitTransferPackage.Status = TouchSocketStatus.Exception.ToValue();
                        waitTransferPackage.Message = ex.Message;
                        e = new FileTransferStatusEventArgs(new Result(ResultCode.Error, StringResStore.GetDescription(TouchSocketStatus.LoadStreamFail)), args);
                    }
                }
                else
                {
                    e = new FileTransferStatusEventArgs(TransferType.Pull, default, default, new Result(ResultCode.Overtime, StringResStore.GetDescription(TouchSocketStatus.GetEventArgsFail)));
                    waitTransferPackage.Status = TouchSocketStatus.GetEventArgsFail.ToValue();
                }

                waitTransferPackage.SwitchId();
                SendPackage(TouchRpcUtility.P_1501_BeginPullFile_Response, waitTransferPackage);

                OnFileTransfered?.Invoke(this, e);
            }
            catch
            {
            }
        }

        private void RequestPullFile(object o)
        {
            try
            {
                WaitFileInfoPackage waitFileInfoPackage = (WaitFileInfoPackage)o;
                try
                {
                    FileOperator fileOperator = new FileOperator()
                    {
                        Flags = waitFileInfoPackage.Flags,
                        ResourcePath = waitFileInfoPackage.ResourcePath,
                        SavePath = waitFileInfoPackage.SavePath,
                        Metadata = waitFileInfoPackage.Metadata
                    };

                    FileOperationEventArgs args = new FileOperationEventArgs(TransferType.Pull,
                        fileOperator, null);

                    OnFileTransfering?.Invoke(this, args);

                    args.ResourcePath = FileController.GetFullPath(m_rootPath, args.ResourcePath);

                    if (args.IsPermitOperation)
                    {
                        if (File.Exists(args.ResourcePath))
                        {
                            //合法传输
                            TouchRpcFileInfo touchRpcFileInfo = new TouchRpcFileInfo();
                            FileController.GetFileInfo(args.ResourcePath, args.Flags.HasFlag(TransferFlags.MD5Verify), ref touchRpcFileInfo);
                            waitFileInfoPackage.FileInfo = touchRpcFileInfo;
                            waitFileInfoPackage.Status = TouchSocketStatus.Success.ToValue();
                            waitFileInfoPackage.EventHashCode = args.GetHashCode();
                            if (m_eventArgs.TryAdd(args.GetHashCode(), args))
                            {
                                EasyTask.DelayRun(1000 * 60, args, (a) =>
                                {
                                    if (m_eventArgs.TryRemove(a.GetHashCode(), out object obj) && obj is FileOperationEventArgs eventArgs)
                                    {
                                        FileTransferStatusEventArgs e = new FileTransferStatusEventArgs(
                                             new Result(ResultCode.Overtime, StringResStore.GetDescription(TouchSocketStatus.Overtime)), eventArgs);
                                        OnFileTransfered?.Invoke(this, e);
                                    }
                                });
                            }
                            else
                            {
                                waitFileInfoPackage.Status = TouchSocketStatus.UnknownError.ToValue();
                            }
                        }
                        else
                        {
                            waitFileInfoPackage.Status = TouchSocketStatus.FileNotExists.ToValue();
                        }
                    }
                    else
                    {
                        waitFileInfoPackage.Message = args.Message;
                        waitFileInfoPackage.Status = TouchSocketStatus.RemoteRefuse.ToValue();
                    }
                }
                catch (Exception ex)
                {
                    waitFileInfoPackage.Status = TouchSocketStatus.Exception.ToValue();
                    waitFileInfoPackage.Message = ex.Message;
                }
                waitFileInfoPackage.SwitchId();
                this.SendPackage(TouchRpcUtility.P_1500_PullFile_Response, waitFileInfoPackage);
            }
            catch
            {
            }
        }

        private void RequestPushFile(object o)
        {
            try
            {
                WaitFileInfoPackage waitFileInfoPackage = (WaitFileInfoPackage)o;
                TouchRpcFileInfo fileInfo = waitFileInfoPackage.FileInfo;
                FileOperator fileOperator = new FileOperator()
                {
                    ResourcePath = waitFileInfoPackage.ResourcePath,
                    SavePath = waitFileInfoPackage.SavePath,
                    Flags = waitFileInfoPackage.Flags,
                    Metadata = waitFileInfoPackage.Metadata,
                };
                WaitTransferPackage waitTransferPackage = new WaitTransferPackage()
                {
                    Sign = waitFileInfoPackage.Sign,
                    TargetId = waitFileInfoPackage.SourceId,
                    SourceId = waitFileInfoPackage.TargetId,
                    Route = waitFileInfoPackage.Route,
                };

                FileOperationEventArgs args = new FileOperationEventArgs(TransferType.Push, fileOperator, fileInfo);
                OnFileTransfering?.Invoke(this, args);

                waitTransferPackage.Path = args.ResourcePath;
                waitTransferPackage.EventHashCode = args.GetHashCode();

                if (!args.IsPermitOperation)
                {
                    fileOperator.SetResult(new Result(ResultCode.Fail, TouchSocketStatus.RemoteRefuse.GetDescription(args.Message)));
                    waitTransferPackage.Message = args.Message;
                    waitTransferPackage.Status = TouchSocketStatus.RemoteRefuse.ToValue();
                    OnFileTransfered?.Invoke(this, new FileTransferStatusEventArgs(fileOperator.Result, args));
                    SendPackage(TouchRpcUtility.P_1502_PushFile_Response, waitTransferPackage);
                    return;
                }

                string saveFullPath = FileController.GetFullPath(m_rootPath, args.SavePath);

                if (!Directory.Exists(Path.GetDirectoryName(saveFullPath)))
                {
                    fileOperator.SetResult(new Result(ResultCode.Fail, TouchSocketStatus.DirectoryNotExists.GetDescription(Path.GetDirectoryName(saveFullPath))));
                    waitTransferPackage.Message = args.Message;
                    waitTransferPackage.Status = TouchSocketStatus.DirectoryNotExists.ToValue();
                    waitTransferPackage.Path = Path.GetDirectoryName(saveFullPath);
                    OnFileTransfered?.Invoke(this, new FileTransferStatusEventArgs(fileOperator.Result, args));
                    SendPackage(TouchRpcUtility.P_1502_PushFile_Response, waitTransferPackage);
                    return;
                }
                Channel channel = null;
                try
                {
                    FileController.TryReadTempInfo(saveFullPath, args.Flags, ref fileInfo);
                    using (TouchRpcFileStream stream = TouchRpcFileStream.Create(saveFullPath, ref fileInfo, args.Flags.HasFlag(TransferFlags.MD5Verify)))
                    {
                        if (waitFileInfoPackage.Route)
                        {
                            channel = CreateChannel(waitTransferPackage.SourceId);
                        }
                        else
                        {
                            channel = CreateChannel();
                        }

                        waitTransferPackage.ChannelID = channel.ID;
                        waitTransferPackage.Position = fileInfo.Position;
                        waitTransferPackage.Status = TouchSocketStatus.Success.ToValue();
                        SendPackage(TouchRpcUtility.P_1502_PushFile_Response, waitTransferPackage);

                        fileOperator.SetFileCompletedLength(fileInfo.Position);
                        fileOperator.SetLength(fileInfo.Length);
                        while (channel.MoveNext())
                        {
                            if (fileOperator.Token.IsCancellationRequested)
                            {
                                channel.Cancel();
                                fileOperator.SetResult(new Result(ResultCode.Canceled));
                                break;
                            }

                            byte[] data = channel.GetCurrent();
                            if (data != null)
                            {
                                stream.Write(data, 0, data.Length);
                                fileInfo.Position = stream.FileWriter.Position;
                                fileOperator.AddFlow(data.Length);
                            }
                        }

                        if (channel.Status == ChannelStatus.Completed)
                        {
                            stream.FinishStream();
                            fileOperator.SetResult(new Result(ResultCode.Success));
                        }
                        else
                        {
                            fileOperator.SetResult(new Result(channel.Status.ToResultCode()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    fileOperator.SetResult(new Result(ResultCode.Error, ex.Message));
                    channel?.Cancel(ex.Message);
                }
                finally
                {
                    channel.SafeDispose();
                }

                OnFileTransfered?.Invoke(this, new FileTransferStatusEventArgs(fileOperator.Result, args));
                if (channel?.Status == ChannelStatus.Completed && fileOperator.Result.IsSuccess())
                {
                    SendPackage(TouchRpcUtility.P_509_PushFileAck_Request, new WaitPushFileAckPackage()
                    {
                        TargetId = waitFileInfoPackage.SourceId,
                        SourceId = waitFileInfoPackage.TargetId,
                        Route = waitFileInfoPackage.Route,
                        Sign = args.GetHashCode(),
                        Status = TouchSocketStatus.Success.ToValue(),
                        Message = fileOperator.Result.Message
                    });
                }
                else
                {
                    SendPackage(TouchRpcUtility.P_509_PushFileAck_Request, new WaitPushFileAckPackage()
                    {
                        TargetId = waitFileInfoPackage.SourceId,
                        SourceId = waitFileInfoPackage.TargetId,
                        Route = waitFileInfoPackage.Route,
                        Sign = args.GetHashCode(),
                        Status = TouchSocketStatus.Exception.ToValue(),
                        Message = fileOperator.Result.Message
                    });
                }
            }
            catch
            {
            }
        }
    }
}