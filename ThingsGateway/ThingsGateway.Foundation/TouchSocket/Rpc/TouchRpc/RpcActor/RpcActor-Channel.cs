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

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    public partial class RpcActor
    {
        private readonly ConcurrentDictionary<int, InternalChannel> m_userChannels;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ChannelExisted(int id)
        {
            return m_userChannels.ContainsKey(id);
        }

        /// <inheritdoc/>
        public Channel CreateChannel()
        {
            return this.PrivateCreateChannel(default, true);
        }

        /// <inheritdoc/>
        public Channel CreateChannel(int id)
        {
            return this.PrivateCreateChannel(default, false, id);
        }

        /// <inheritdoc/>
        public Channel CreateChannel(string targetId, int id)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                throw new ArgumentException($"“{nameof(targetId)}”不能为 null 或空。", nameof(targetId));
            }
            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor actor))
                {
                    return actor.CreateChannel(id);
                }
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription());
            }
            return this.PrivateCreateChannel(targetId, false, id);
        }

        private Channel PrivateCreateChannel(string targetId, bool random, int id = 0)
        {
            lock (SyncRoot)
            {
                if (random)
                {
                    id = new object().GetHashCode();
                }
                else
                {
                    if (ChannelExisted(id))
                    {
                        throw new Exception(TouchSocketStatus.ChannelExisted.GetDescription(id));
                    }
                }

                ByteBlock byteBlock = new ByteBlock();
                WaitCreateChannelPackage waitCreateChannel = new WaitCreateChannelPackage()
                {
                    Random = random,
                    ChannelID = id,
                    SourceId = this.ID,
                    TargetId = targetId,
                    Route = targetId.HasValue()
                };
                WaitData<IWaitResult> waitData = WaitHandlePool.GetWaitData(waitCreateChannel);

                try
                {
                    waitCreateChannel.Package(byteBlock);
                    Send(TouchRpcUtility.P_100_CreateChannel_Request, byteBlock);
                    switch (waitData.Wait(10 * 1000))
                    {
                        case WaitDataStatus.SetRunning:
                            {
                                WaitCreateChannelPackage result = (WaitCreateChannelPackage)waitData.WaitResult;
                                switch (result.Status.ToStatus())
                                {
                                    case TouchSocketStatus.Success:
                                        {
                                            InternalChannel channel = new InternalChannel(this, targetId);
                                            channel.SetID(result.ChannelID);
                                            channel.SetUsing();
                                            if (m_userChannels.TryAdd(result.ChannelID, channel))
                                            {
                                                return channel;
                                            }
                                            throw new Exception(TouchSocketStatus.UnknownError.GetDescription());
                                        }
                                    case TouchSocketStatus.ClientNotFind:
                                        {
                                            throw new Exception(TouchSocketStatus.ClientNotFind.GetDescription(targetId));
                                        }
                                    case TouchSocketStatus.RoutingNotAllowed:
                                    default:
                                        {
                                            throw new Exception(result.Status.ToStatus().GetDescription(result.Message));
                                        }
                                }
                            }
                        case WaitDataStatus.Overtime:
                            {
                                throw new TimeoutException(TouchSocketStatus.Overtime.GetDescription());
                            }
                        default:
                            {
                                throw new Exception(TouchSocketStatus.UnknownError.GetDescription());
                            }
                    }
                }
                finally
                {
                    WaitHandlePool.Destroy(waitData);
                    byteBlock.Dispose();
                }
            }
        }

        internal void SendChannelPackage(ChannelPackage channelPackage)
        {
            using (ByteBlock byteBlock = new ByteBlock(channelPackage.GetLen()))
            {
                channelPackage.Package(byteBlock);
                this.Send(TouchRpcUtility.P_101_ChannelPackage, byteBlock);
            }
        }

        /// <inheritdoc/>
        public Channel CreateChannel(string targetId)
        {
            if (string.IsNullOrEmpty(targetId))
            {
                throw new ArgumentException($"“{nameof(targetId)}”不能为 null 或空。", nameof(targetId));
            }

            if (IsService)
            {
                if (this.TryFindRpcActor(targetId, out RpcActor actor))
                {
                    return actor.CreateChannel();
                }
                throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription());
            }
            return this.PrivateCreateChannel(targetId, true);
        }

        internal void RemoveChannel(int id)
        {
            m_userChannels.TryRemove(id, out _);
        }

        /// <summary>
        /// 订阅通道
        /// </summary>
        /// <param name="id"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool TrySubscribeChannel(int id, out Channel channel)
        {
            if (m_userChannels.TryGetValue(id, out InternalChannel channelOut))
            {
                if (channelOut.Using)
                {
                    channel = null;
                    return false;
                }
                channelOut.SetUsing();
                channel = channelOut;
                return true;
            }
            channel = null;
            return false;
        }

        private bool QueueChannelPackage(ChannelPackage channelPackage)
        {
            if (m_userChannels.TryGetValue(channelPackage.ChannelId, out InternalChannel channel))
            {
                channel.ReceivedData(channelPackage);
                return true;
            }

            return false;
        }

        private bool RequestCreateChannel(int id, string targetId)
        {
            lock (SyncRoot)
            {
                InternalChannel channel = new InternalChannel(this, targetId);
                channel.SetID(id);
                if (m_userChannels.TryAdd(id, channel))
                {
                    return true;
                }
                else
                {
                    channel.SafeDispose();
                    return false;
                }
            }
        }
    }
}