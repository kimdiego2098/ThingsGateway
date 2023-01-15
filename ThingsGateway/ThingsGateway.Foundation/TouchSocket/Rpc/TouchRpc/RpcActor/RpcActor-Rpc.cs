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
using System.Collections.Generic;

using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    public partial class RpcActor
    {
        private readonly ConcurrentDictionary<long, TouchRpcCallContext> m_contextDic;

        /// <inheritdoc/>
        public object Caller { get; set; }

        /// <inheritdoc/>
        public RpcStore RpcStore { get; set; }

        /// <inheritdoc/>
        public SerializationSelector SerializationSelector { get; set; }

        #region Rpc

        /// <inheritdoc/>
        public T Invoke<T>(string invokeKey, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            TouchRpcPackage rpcPackage = new TouchRpcPackage
            {
                MethodName = invokeKey,
                SourceId = ID
            };
            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(rpcPackage);
            ByteBlock byteBlock = new ByteBlock();
            if (invokeOption == default)
            {
                invokeOption = InvokeOption.WaitInvoke;
            }

            if (invokeOption.Token.CanBeCanceled)
            {
                waitData.SetCancellationToken(invokeOption.Token);
                invokeOption.Token.Register(CanceledInvoke, new CanceledPackage() { SourceId = ID, Sign = rpcPackage.Sign });
            }

            try
            {
                rpcPackage.LoadInvokeOption(invokeOption);
                List<byte[]> datas = new List<byte[]>();
                foreach (object parameter in parameters)
                {
                    datas.Add(SerializationSelector.SerializeParameter(rpcPackage.SerializationType, parameter));
                }
                rpcPackage.ParametersBytes = datas;
                rpcPackage.Package(byteBlock);

                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            return default;
                        }
                    case FeedbackType.WaitSend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    {
                                        break;
                                    }
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            return default;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    {
                                        TouchRpcPackage resultContext = (TouchRpcPackage)waitData.WaitResult;
                                        resultContext.ThrowStatus();
                                        if (resultContext.IsByRef)
                                        {
                                            try
                                            {
                                                for (int i = 0; i < parameters.Length; i++)
                                                {
                                                    parameters[i] = SerializationSelector.DeserializeParameter(resultContext.SerializationType, resultContext.ParametersBytes[i], types[i]);
                                                }
                                            }
                                            catch (System.Exception e)
                                            {
                                                throw new Exception(e.Message);
                                            }
                                        }
                                        else
                                        {
                                            parameters = null;
                                        }
                                        return (T)SerializationSelector.DeserializeParameter(resultContext.SerializationType, resultContext.ReturnParameterBytes, typeof(T));
                                    }
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            return default;
                        }
                    default:
                        return default;
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Invoke(string invokeKey, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            TouchRpcPackage rpcPackage = new TouchRpcPackage
            {
                MethodName = invokeKey,
                SourceId = ID
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(rpcPackage);
            ByteBlock byteBlock = new ByteBlock();
            if (invokeOption == default)
            {
                invokeOption = InvokeOption.WaitInvoke;
            }

            if (invokeOption.Token.CanBeCanceled)
            {
                waitData.SetCancellationToken(invokeOption.Token);
                invokeOption.Token.Register(CanceledInvoke, new CanceledPackage() { SourceId = ID, Sign = rpcPackage.Sign });
            }

            try
            {
                rpcPackage.LoadInvokeOption(invokeOption);
                List<byte[]> datas = new List<byte[]>();
                foreach (object parameter in parameters)
                {
                    datas.Add(SerializationSelector.SerializeParameter(rpcPackage.SerializationType, parameter));
                }
                rpcPackage.ParametersBytes = datas;
                rpcPackage.Package(byteBlock);

                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                        }
                        break;

                    case FeedbackType.WaitSend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    {
                                        break;
                                    }
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            break;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    {
                                        TouchRpcPackage resultContext = (TouchRpcPackage)waitData.WaitResult;
                                        resultContext.ThrowStatus();
                                        if (resultContext.IsByRef)
                                        {
                                            for (int i = 0; i < parameters.Length; i++)
                                            {
                                                parameters[i] = SerializationSelector.DeserializeParameter(resultContext.SerializationType, resultContext.ParametersBytes[i], types[i]);
                                            }
                                        }
                                        else
                                        {
                                            parameters = null;
                                        }
                                        break;
                                    }
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Invoke(string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            TouchRpcPackage rpcPackage = new TouchRpcPackage
            {
                MethodName = invokeKey,
                SourceId = ID
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(rpcPackage);
            ByteBlock byteBlock = new ByteBlock();
            if (invokeOption == default)
            {
                invokeOption = InvokeOption.WaitInvoke;
            }

            if (invokeOption.Token.CanBeCanceled)
            {
                waitData.SetCancellationToken(invokeOption.Token);
                invokeOption.Token.Register(CanceledInvoke, new CanceledPackage() { SourceId = ID, Sign = rpcPackage.Sign });
            }

            try
            {
                rpcPackage.LoadInvokeOption(invokeOption);
                if (parameters != null)
                {
                    List<byte[]> datas = new List<byte[]>();
                    foreach (object parameter in parameters)
                    {
                        datas.Add(SerializationSelector.SerializeParameter(rpcPackage.SerializationType, parameter));
                    }
                    rpcPackage.ParametersBytes = datas;
                }

                rpcPackage.Package(byteBlock);

                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                        }
                        break;

                    case FeedbackType.WaitSend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    break;

                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            break;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    {
                                        TouchRpcPackage resultContext = (TouchRpcPackage)waitData.WaitResult;
                                        resultContext.ThrowStatus();
                                        break;
                                    }
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        /// <inheritdoc/>
        public T Invoke<T>(string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            TouchRpcPackage rpcPackage = new TouchRpcPackage
            {
                MethodName = invokeKey,
                SourceId = ID
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(rpcPackage);
            ByteBlock byteBlock = new ByteBlock();
            if (invokeOption == default)
            {
                invokeOption = InvokeOption.WaitInvoke;
            }

            if (invokeOption.Token.CanBeCanceled)
            {
                waitData.SetCancellationToken(invokeOption.Token);
                invokeOption.Token.Register(CanceledInvoke, new CanceledPackage() { SourceId = ID, Sign = rpcPackage.Sign });
            }

            try
            {
                rpcPackage.LoadInvokeOption(invokeOption);
                if (parameters != null)
                {
                    List<byte[]> datas = new List<byte[]>();
                    foreach (object parameter in parameters)
                    {
                        datas.Add(SerializationSelector.SerializeParameter(rpcPackage.SerializationType, parameter));
                    }
                    rpcPackage.ParametersBytes = datas;
                }

                rpcPackage.Package(byteBlock);

                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            return default;
                        }
                    case FeedbackType.WaitSend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            return default;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    {
                                        TouchRpcPackage resultContext = (TouchRpcPackage)waitData.WaitResult;
                                        resultContext.ThrowStatus();
                                        return (T)SerializationSelector.DeserializeParameter(resultContext.SerializationType, resultContext.ReturnParameterBytes, typeof(T));
                                    }
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            return default;
                        }

                    default:
                        return default;
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        /// <inheritdoc/>
        public Task InvokeAsync(string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            return EasyTask.Run(() =>
            {
                Invoke(invokeKey, invokeOption, parameters);
            });
        }

        /// <inheritdoc/>
        public Task<T> InvokeAsync<T>(string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            return EasyTask.Run(() =>
            {
                return Invoke<T>(invokeKey, invokeOption, parameters);
            });
        }

        #endregion Rpc

        #region IdRpc

        /// <inheritdoc/>
        public void Invoke(string targetId, string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                throw new ArgumentException($"“{nameof(targetId)}”不能为 null 或空。", nameof(targetId));
            }

            if (string.IsNullOrEmpty(invokeKey))
            {
                throw new ArgumentException($"“{nameof(invokeKey)}”不能为 null 或空。", nameof(invokeKey));
            }

            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor actor))
                {
                    actor.Invoke(invokeKey, invokeOption, parameters);
                    return;
                }
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription());
            }

            TouchRpcPackage rpcPackage = new TouchRpcPackage
            {
                MethodName = invokeKey,
                Route = true,
                TargetId = targetId,
                SourceId = ID,
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetReverseWaitData(rpcPackage);
            ByteBlock byteBlock = new ByteBlock();
            if (invokeOption == default)
            {
                invokeOption = InvokeOption.WaitInvoke;
            }

            if (invokeOption.Token.CanBeCanceled)
            {
                waitData.SetCancellationToken(invokeOption.Token);
                invokeOption.Token.Register(CanceledInvoke, new CanceledPackage() { SourceId = ID, TargetId = targetId, Sign = rpcPackage.Sign, Route = true });
            }

            try
            {
                rpcPackage.LoadInvokeOption(invokeOption);
                if (parameters != null)
                {
                    List<byte[]> datas = new List<byte[]>();
                    foreach (object parameter in parameters)
                    {
                        datas.Add(SerializationSelector.SerializeParameter(rpcPackage.SerializationType, parameter));
                    }
                    rpcPackage.ParametersBytes = datas;
                }

                rpcPackage.Package(byteBlock);

                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                        }
                        break;

                    case FeedbackType.WaitSend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    break;

                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            break;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    {
                                        TouchRpcPackage resultContext = (TouchRpcPackage)waitData.WaitResult;
                                        resultContext.ThrowStatus();
                                        break;
                                    }
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        /// <inheritdoc/>
        public T Invoke<T>(string targetId, string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                throw new ArgumentException($"“{nameof(targetId)}”不能为 null 或空。", nameof(targetId));
            }

            if (string.IsNullOrEmpty(invokeKey))
            {
                throw new ArgumentException($"“{nameof(invokeKey)}”不能为 null 或空。", nameof(invokeKey));
            }

            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor rpcActor))
                {
                    return rpcActor.Invoke<T>(invokeKey, invokeOption, parameters);
                }
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription());
            }

            TouchRpcPackage rpcPackage = new TouchRpcPackage
            {
                MethodName = invokeKey,
                TargetId = targetId,
                SourceId = ID,
                Route = true
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetReverseWaitData(rpcPackage);
            ByteBlock byteBlock = new ByteBlock();
            if (invokeOption == default)
            {
                invokeOption = InvokeOption.WaitInvoke;
            }

            if (invokeOption.Token.CanBeCanceled)
            {
                waitData.SetCancellationToken(invokeOption.Token);
                invokeOption.Token.Register(CanceledInvoke, new CanceledPackage() { SourceId = ID, TargetId = targetId, Sign = rpcPackage.Sign, Route = true });
            }

            try
            {
                rpcPackage.LoadInvokeOption(invokeOption);
                if (parameters != null)
                {
                    List<byte[]> datas = new List<byte[]>();
                    foreach (object parameter in parameters)
                    {
                        datas.Add(SerializationSelector.SerializeParameter(rpcPackage.SerializationType, parameter));
                    }
                    rpcPackage.ParametersBytes = datas;
                }

                rpcPackage.Package(byteBlock);

                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            return default;
                        }
                    case FeedbackType.WaitSend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            return default;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    {
                                        TouchRpcPackage resultContext = (TouchRpcPackage)waitData.WaitResult;
                                        resultContext.ThrowStatus();
                                        return (T)SerializationSelector.DeserializeParameter(resultContext.SerializationType, resultContext.ReturnParameterBytes, typeof(T));
                                    }
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            return default;
                        }

                    default:
                        return default;
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Invoke(string targetId, string invokeKey, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                throw new ArgumentException($"“{nameof(targetId)}”不能为 null 或空。", nameof(targetId));
            }

            if (string.IsNullOrEmpty(invokeKey))
            {
                throw new ArgumentException($"“{nameof(invokeKey)}”不能为 null 或空。", nameof(invokeKey));
            }

            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor rpcActor))
                {
                    rpcActor.Invoke(invokeKey, invokeOption, ref parameters, types);
                    return;
                }
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription());
            }

            TouchRpcPackage rpcPackage = new TouchRpcPackage
            {
                MethodName = invokeKey,
                TargetId = targetId,
                SourceId = ID,
                Route = true
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetReverseWaitData(rpcPackage);
            ByteBlock byteBlock = new ByteBlock();
            if (invokeOption == default)
            {
                invokeOption = InvokeOption.WaitInvoke;
            }

            if (invokeOption.Token.CanBeCanceled)
            {
                waitData.SetCancellationToken(invokeOption.Token);
                invokeOption.Token.Register(CanceledInvoke, new CanceledPackage() { SourceId = ID, TargetId = targetId, Sign = rpcPackage.Sign, Route = true });
            }

            try
            {
                rpcPackage.LoadInvokeOption(invokeOption);
                List<byte[]> datas = new List<byte[]>();
                foreach (object parameter in parameters)
                {
                    datas.Add(SerializationSelector.SerializeParameter(rpcPackage.SerializationType, parameter));
                }
                rpcPackage.ParametersBytes = datas;
                rpcPackage.Package(byteBlock);

                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            return;
                        }
                    case FeedbackType.WaitSend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            return;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    {
                                        TouchRpcPackage resultContext = (TouchRpcPackage)waitData.WaitResult;
                                        resultContext.ThrowStatus();
                                        if (resultContext.IsByRef)
                                        {
                                            for (int i = 0; i < parameters.Length; i++)
                                            {
                                                parameters[i] = SerializationSelector.DeserializeParameter(resultContext.SerializationType, resultContext.ParametersBytes[i], types[i]);
                                            }
                                        }
                                        else
                                        {
                                            parameters = null;
                                        }
                                        break;
                                    }
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            return;
                        }

                    default:
                        return;
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        /// <inheritdoc/>
        public T Invoke<T>(string targetId, string invokeKey, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                throw new ArgumentException($"“{nameof(targetId)}”不能为 null 或空。", nameof(targetId));
            }

            if (string.IsNullOrEmpty(invokeKey))
            {
                throw new ArgumentException($"“{nameof(invokeKey)}”不能为 null 或空。", nameof(invokeKey));
            }

            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor rpcActor))
                {
                    return rpcActor.Invoke<T>(invokeKey, invokeOption, ref parameters, types);
                }
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription());
            }

            TouchRpcPackage rpcPackage = new TouchRpcPackage
            {
                MethodName = invokeKey,
                TargetId = targetId,
                SourceId = ID,
                Route = true
            };

            WaitData<IWaitResult> waitData = WaitHandlePool.GetReverseWaitData(rpcPackage);
            ByteBlock byteBlock = new ByteBlock();
            if (invokeOption == default)
            {
                invokeOption = InvokeOption.WaitInvoke;
            }

            if (invokeOption.Token.CanBeCanceled)
            {
                waitData.SetCancellationToken(invokeOption.Token);
                invokeOption.Token.Register(CanceledInvoke, new CanceledPackage() { SourceId = ID, TargetId = targetId, Sign = rpcPackage.Sign, Route = true });
            }

            try
            {
                rpcPackage.LoadInvokeOption(invokeOption);
                List<byte[]> datas = new List<byte[]>();
                foreach (object parameter in parameters)
                {
                    datas.Add(SerializationSelector.SerializeParameter(rpcPackage.SerializationType, parameter));
                }
                rpcPackage.ParametersBytes = datas;
                rpcPackage.Package(byteBlock);

                switch (invokeOption.FeedbackType)
                {
                    case FeedbackType.OnlySend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            return default;
                        }
                    case FeedbackType.WaitSend:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            return default;
                        }
                    case FeedbackType.WaitInvoke:
                        {
                            Send(TouchRpcUtility.P_200_Invoke_Request, byteBlock.Buffer, 0, byteBlock.Len);
                            switch (waitData.Wait(invokeOption.Timeout))
                            {
                                case WaitDataStatus.SetRunning:
                                    {
                                        TouchRpcPackage resultContext = (TouchRpcPackage)waitData.WaitResult;
                                        resultContext.ThrowStatus();
                                        if (resultContext.IsByRef)
                                        {
                                            for (int i = 0; i < parameters.Length; i++)
                                            {
                                                parameters[i] = SerializationSelector.DeserializeParameter(resultContext.SerializationType, resultContext.ParametersBytes[i], types[i]);
                                            }
                                        }
                                        else
                                        {
                                            parameters = null;
                                        }
                                        return (T)SerializationSelector.DeserializeParameter(resultContext.SerializationType, resultContext.ReturnParameterBytes, typeof(T));
                                    }
                                case WaitDataStatus.Overtime:
                                    {
                                        throw new TimeoutException("等待结果超时");
                                    }
                            }
                            return default;
                        }

                    default:
                        return default;
                }
            }
            finally
            {
                WaitHandlePool.Destroy(waitData);
                byteBlock.Dispose();
            }
        }

        /// <inheritdoc/>
        public Task InvokeAsync(string targetId, string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            return EasyTask.Run(() =>
            {
                Invoke(targetId, invokeKey, invokeOption, parameters);
            });
        }

        /// <inheritdoc/>
        public Task<T> InvokeAsync<T>(string targetId, string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            return EasyTask.Run(() =>
            {
                return Invoke<T>(targetId, invokeKey, invokeOption, parameters);
            });
        }

        #endregion IdRpc

        private void CanceledInvoke(object obj)
        {
            if (obj is CanceledPackage canceled)
            {
                using (ByteBlock byteBlock = new ByteBlock())
                {
                    canceled.Package(byteBlock);
                    Send(TouchRpcUtility.P_204_CancelInvoke, byteBlock);
                }
            }
        }

        private void InvokeThis(object o)
        {
            try
            {
                TouchRpcPackage rpcPackage = (TouchRpcPackage)o;

                List<byte[]> psData = rpcPackage.ParametersBytes;
                if (rpcPackage.Feedback == FeedbackType.WaitSend)
                {
                    using (ByteBlock returnByteBlock = new ByteBlock())
                    {
                        rpcPackage.SwitchId();
                        rpcPackage.MethodName = default;
                        rpcPackage.ParametersBytes = default;
                        rpcPackage.Status = TouchSocketStatus.Success.ToValue();
                        rpcPackage.Package(returnByteBlock);
                        Send(TouchRpcUtility.P_1200_Invoke_Response, returnByteBlock);
                        rpcPackage.SwitchId();
                    }
                }

                InvokeResult invokeResult = new InvokeResult();
                object[] ps = null;
                MethodInstance methodInstance = GetInvokeMethod?.Invoke(rpcPackage.MethodName);
                TouchRpcCallContext callContext = null;
                if (methodInstance != null)
                {
                    try
                    {
                        if (methodInstance.IsEnable)
                        {
                            callContext = new TouchRpcCallContext()
                            {
                                Caller = Caller,
                                MethodInstance = methodInstance,
                                TouchRpcPackage = rpcPackage
                            };

                            m_contextDic.TryAdd(rpcPackage.Sign, callContext);

                            if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
                            {
                                ps = new object[methodInstance.ParameterTypes.Length];
                                ps[0] = callContext;
                                for (int i = 0; i < psData.Count; i++)
                                {
                                    ps[i + 1] = SerializationSelector.DeserializeParameter(rpcPackage.SerializationType, psData[i], methodInstance.ParameterTypes[i + 1]);
                                }
                            }
                            else
                            {
                                ps = new object[methodInstance.ParameterTypes.Length];
                                for (int i = 0; i < methodInstance.ParameterTypes.Length; i++)
                                {
                                    ps[i] = SerializationSelector.DeserializeParameter(rpcPackage.SerializationType, psData[i], methodInstance.ParameterTypes[i]);
                                }
                            }
                        }
                        else
                        {
                            invokeResult.Status = InvokeStatus.UnEnable;
                        }
                    }
                    catch (Exception ex)
                    {
                        invokeResult.Status = InvokeStatus.Exception;
                        invokeResult.Message = ex.Message;
                    }
                }
                else
                {
                    invokeResult.Status = InvokeStatus.UnFound;
                }

                if (invokeResult.Status == InvokeStatus.Ready)
                {
                    IRpcServer rpcServer = methodInstance.ServerFactory.Create(callContext, ps);
                    if (rpcServer is ITransientRpcServer transientRpcServer)
                    {
                        transientRpcServer.CallContext = callContext;
                    }
                    invokeResult = RpcStore.Execute(rpcServer, ps, callContext);
                }

                if (rpcPackage.Feedback == FeedbackType.OnlySend)
                {
                    return;
                }

                switch (invokeResult.Status)
                {
                    case InvokeStatus.UnFound:
                        {
                            rpcPackage.Status = TouchSocketStatus.RpcMethodNotFind.ToValue();
                            break;
                        }
                    case InvokeStatus.Success:
                        {
                            if (methodInstance.HasReturn)
                            {
                                rpcPackage.ReturnParameterBytes = SerializationSelector.SerializeParameter(rpcPackage.SerializationType, invokeResult.Result);
                            }
                            else
                            {
                                rpcPackage.ReturnParameterBytes = null;
                            }

                            if (methodInstance.IsByRef)
                            {
                                rpcPackage.IsByRef = true;
                                rpcPackage.ParametersBytes = new List<byte[]>();

                                int i = 0;
                                if (methodInstance.MethodFlags.HasFlag(MethodFlags.IncludeCallContext))
                                {
                                    i = 1;
                                }
                                for (; i < ps.Length; i++)
                                {
                                    rpcPackage.ParametersBytes.Add(SerializationSelector.SerializeParameter(rpcPackage.SerializationType, ps[i]));
                                }
                            }
                            else
                            {
                                rpcPackage.ParametersBytes = null;
                            }

                            rpcPackage.Status = TouchSocketStatus.Success.ToValue();
                            break;
                        }
                    case InvokeStatus.UnEnable:
                        {
                            rpcPackage.Status = TouchSocketStatus.RpcMethodDisable.ToValue();
                            break;
                        }
                    case InvokeStatus.InvocationException:
                        {
                            rpcPackage.Status = TouchSocketStatus.RpcInvokeException.ToValue();
                            rpcPackage.Message = invokeResult.Message;
                            break;
                        }
                    case InvokeStatus.Exception:
                        {
                            rpcPackage.Status = TouchSocketStatus.Exception.ToValue();
                            rpcPackage.Message = invokeResult.Message;
                            break;
                        }
                    default:
                        return;
                }

                m_contextDic.TryRemove(rpcPackage.Sign, out _);

                using (ByteBlock byteBlock = new ByteBlock())
                {
                    rpcPackage.MethodName = default;
                    rpcPackage.SwitchId();
                    rpcPackage.Package(byteBlock);
                    Send(TouchRpcUtility.P_1200_Invoke_Response, byteBlock);
                }
            }
            catch
            {
            }
        }
    }
}