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
using System.Diagnostics;
using System.Threading
.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// 通道
    /// </summary>
    [DebuggerDisplay("ID={ID},Status={Status}")]
    public abstract class Channel : DisposableObject
    {
        /// <summary>
        /// 是否具有数据可读
        /// </summary>
        public abstract int Available { get; }

        /// <summary>
        /// 缓存容量
        /// </summary>
        public abstract int CacheCapacity { get; set; }

        /// <summary>
        /// 判断当前通道能否调用<see cref="MoveNext()"/>
        /// </summary>
        public abstract bool CanMoveNext { get; }

        /// <summary>
        /// 能否写入
        /// </summary>
        public abstract bool CanWrite { get; }

        /// <summary>
        /// 通道ID
        /// </summary>
        public abstract int ID { get; }

        /// <summary>
        /// 最后一次操作时显示消息
        /// </summary>
        public abstract string LastOperationMes { get; }

        /// <summary>
        /// 状态
        /// </summary>
        public abstract ChannelStatus Status { get; }

        /// <summary>
        /// 目的ID地址。仅当该通道由两个客户端打通时有效。
        /// </summary>
        public abstract string TargetId { get; }

        /// <summary>
        /// 超时时间，默认1000*10ms。
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// 是否被使用
        /// </summary>
        public abstract bool Using { get; }

        /// <summary>
        /// 取消
        /// </summary>
        public abstract void Cancel(string operationMes = null);

        /// <summary>
        /// 异步取消
        /// </summary>
        /// <returns></returns>
        public Task CancelAsync(string operationMes = null)
        {
            return EasyTask.Run(() =>
            {
                Cancel(operationMes);
            });
        }

        /// <summary>
        /// 完成操作
        /// </summary>
        public abstract void Complete(string operationMes = null);

        /// <summary>
        /// 异步完成操作
        /// </summary>
        /// <returns></returns>
        public Task CompleteAsync(string operationMes = null)
        {
            return EasyTask.Run(() =>
             {
                 Complete(operationMes);
             });
        }

        /// <summary>
        /// 获取当前的数据
        /// </summary>
        public abstract byte[] GetCurrent();

        /// <summary>
        /// 继续。
        /// <para>调用该指令时，接收方会跳出接收，但是通道依然可用，所以接收方需要重新调用<see cref="MoveNext()"/></para>
        /// </summary>
        /// <param name="operationMes"></param>
        public abstract void HoldOn(string operationMes = null);

        /// <summary>
        /// 异步调用继续
        /// </summary>
        /// <param name="operationMes"></param>
        /// <returns></returns>
        public Task HoldOnAsync(string operationMes = null)
        {
            return EasyTask.Run(() =>
            {
                HoldOn(operationMes);
            });
        }

        /// <summary>
        /// 转向下个元素
        /// </summary>
        /// <returns></returns>
        public abstract bool MoveNext();

        /// <summary>
        /// 转向下个元素
        /// </summary>
        /// <returns></returns>
        public Task<bool> MoveNextAsync()
        {
            return EasyTask.Run(() =>
            {
                return MoveNext();
            });
        }

        /// <summary>
        /// 阻塞读取数据，直到有数据，或者超时。
        /// </summary>
        /// <returns></returns>
        public Task<byte[]> ReadAsync()
        {
            return EasyTask.Run(() =>
             {
                 if (MoveNext())
                 {
                     return GetCurrent();
                 }
                 return null;
             });
        }

        /// <summary>
        /// 尝试写入。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool TryWrite(byte[] data, int offset, int length)
        {
            if (CanWrite)
            {
                try
                {
                    Write(data, offset, length);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试写入
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryWrite(byte[] data)
        {
            return TryWrite(data, 0, data.Length);
        }

        /// <summary>
        /// 异步尝试写入
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task<bool> TryWriteAsync(byte[] data, int offset, int length)
        {
            if (CanWrite)
            {
                try
                {
                    await WriteAsync(data, offset, length);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        /// 异步尝试写入
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<bool> TryWriteAsync(byte[] data)
        {
            return TryWriteAsync(data, 0, data.Length);
        }

        /// <summary>
        /// 写入通道
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public abstract void Write(byte[] data, int offset, int length);

        /// <summary>
        /// 写入通道
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data)
        {
            Write(data, 0, data.Length);
        }

        /// <summary>
        /// 写入通道
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public Task WriteAsync(byte[] data, int offset, int length)
        {
            return Task.Run(() =>
             {
                 this.Write(data, offset, length);
             });
        }

        /// <summary>
        /// 写入通道
        /// </summary>
        /// <param name="data"></param>
        public void WriteAsync(byte[] data)
        {
            WriteAsync(data, 0, data.Length);
        }
    }

    internal class InternalChannel : Channel
    {
        private readonly RpcActor m_actor;
        private readonly IntelligentDataQueue<ChannelPackage> m_dataQueue;
        private readonly AutoResetEvent m_moveWaitHandle;
        private readonly string m_targetId;
        private readonly Timer m_timer;
        private int m_cacheCapacity;
        private volatile bool m_canFree;
        private byte[] m_currentData;
        private int m_id;
        private string m_lastOperationMes;
        private DateTime m_lastOperationTime;
        private ChannelStatus m_status;
        private bool m_using;

        public InternalChannel(RpcActor client, string targetId)
        {
            m_actor = client;
            m_lastOperationTime = DateTime.Now;
            m_targetId = targetId;
            m_status = ChannelStatus.Default;
            m_cacheCapacity = 1024 * 1024 * 5;
            m_dataQueue = new IntelligentDataQueue<ChannelPackage>(m_cacheCapacity)
            {
                OverflowWait = false,
                OnQueueChanged = OnQueueChanged
            };
            m_moveWaitHandle = new AutoResetEvent(false);
            m_canFree = true;
            m_timer = new Timer((o) =>
            {
                if (DateTime.Now - m_lastOperationTime > TimeSpan.FromTicks(Timeout.Ticks * 3))
                {
                    this.SafeDispose();
                }
            }, null, 1000, 1000);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~InternalChannel()
        {
            Dispose(false);
        }

        /// <summary>
        /// 是否具有数据可读
        /// </summary>
        public override int Available => m_dataQueue.Count;

        /// <summary>
        /// 缓存容量
        /// </summary>
        public override int CacheCapacity
        {
            get => m_cacheCapacity;
            set
            {
                if (value < 0)
                {
                    value = 1024;
                }
                m_cacheCapacity = value;
                m_dataQueue.MaxSize = value;
            }
        }

        /// <summary>
        /// 判断当前通道能否调用<see cref="MoveNext()"/>
        /// </summary>
        public override bool CanMoveNext
        {
            get
            {
                if (Available > 0)
                {
                    return true;
                }
                if ((byte)m_status >= 4)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 能否写入
        /// </summary>
        public override bool CanWrite => (byte)m_status <= 3;

        /// <summary>
        /// ID
        /// </summary>
        public override int ID => m_id;

        /// <summary>
        /// 最后一次操作时显示消息
        /// </summary>
        public override string LastOperationMes
        {
            get => m_lastOperationMes;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public override ChannelStatus Status => m_status;

        /// <summary>
        /// 目的ID地址。
        /// </summary>
        public override string TargetId => m_targetId;

        /// <summary>
        /// 是否被使用
        /// </summary>
        public override bool Using => m_using;

        #region 操作

        /// <summary>
        /// 取消
        /// </summary>
        public override void Cancel(string operationMes = null)
        {
            if ((byte)m_status > 3)
            {
                return;
            }
            try
            {
                this.RequestCancel(true);
                ChannelPackage channelPackage = new ChannelPackage()
                {
                    ChannelId = this.m_id,
                    RunNow = true,
                    DataType = ChannelDataType.CancelOrder,
                    Message = operationMes,
                    SourceId = this.m_actor.ID,
                    TargetId = this.m_targetId,
                    Route = this.m_targetId.HasValue()
                };
                this.m_actor.SendChannelPackage(channelPackage);
                m_lastOperationTime = DateTime.Now;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 完成操作
        /// </summary>
        public override void Complete(string operationMes = null)
        {
            if ((byte)m_status > 3)
            {
                return;
            }

            this.RequestComplete(true);
            ChannelPackage channelPackage = new ChannelPackage()
            {
                ChannelId = this.m_id,
                RunNow = true,
                DataType = ChannelDataType.CompleteOrder,
                Message = operationMes,
                SourceId = this.m_actor.ID,
                TargetId = this.m_targetId,
                Route = this.m_targetId.HasValue()
            };
            this.m_actor.SendChannelPackage(channelPackage);
            m_lastOperationTime = DateTime.Now;
        }

        /// <summary>
        /// 继续。
        /// <para>调用该指令时，接收方会跳出接收，但是通道依然可用，所以接收方需要重新调用<see cref="MoveNext()"/></para>
        /// </summary>
        /// <param name="operationMes"></param>
        public override void HoldOn(string operationMes = null)
        {
            if ((byte)m_status > 3)
            {
                return;
            }
            ChannelPackage channelPackage = new ChannelPackage()
            {
                ChannelId = this.m_id,
                RunNow = true,
                DataType = ChannelDataType.HoldOnOrder,
                Message = operationMes,
                SourceId = this.m_actor.ID,
                TargetId = this.m_targetId,
                Route = this.m_targetId.HasValue()
            };
            this.m_actor.SendChannelPackage(channelPackage);
            m_lastOperationTime = DateTime.Now;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if ((byte)m_status > 3)
            {
                return;
            }
            m_timer.SafeDispose();

            try
            {
                this.RequestDispose(true);
                ChannelPackage channelPackage = new ChannelPackage()
                {
                    ChannelId = this.m_id,
                    RunNow = true,
                    DataType = ChannelDataType.HoldOnOrder,
                    SourceId = this.m_actor.ID,
                    TargetId = this.m_targetId,
                    Route = this.m_targetId.HasValue()
                };
                this.m_actor.SendChannelPackage(channelPackage);
                m_lastOperationTime = DateTime.Now;
            }
            catch
            {
            }
            base.Dispose(disposing);
        }

        #endregion 操作

        /// <summary>
        /// 获取当前的数据
        /// </summary>
        public override byte[] GetCurrent()
        {
            return this.m_currentData;
        }

        /// <summary>
        /// 转向下个元素
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            if (!this.CanMoveNext)
            {
                return false;
            }
            if (m_dataQueue.TryDequeue(out ChannelPackage channelPackage))
            {
                switch (channelPackage.DataType)
                {
                    case ChannelDataType.DataOrder:
                        {
                            m_currentData = channelPackage.Data.Array;
                            return true;
                        }
                    case ChannelDataType.CompleteOrder:
                        RequestComplete(true);
                        return false;

                    case ChannelDataType.CancelOrder:
                        RequestCancel(true);
                        return false;

                    case ChannelDataType.DisposeOrder:
                        RequestDispose(true);
                        return false;

                    case ChannelDataType.HoldOnOrder:
                        m_status = ChannelStatus.HoldOn;
                        return false;

                    case ChannelDataType.QueueRun:
                        this.m_canFree = true;
                        return false;

                    case ChannelDataType.QueuePause:
                        this.m_canFree = false;
                        return false;

                    default:
                        return false;
                }
            }

            m_moveWaitHandle.Reset();
            if (m_moveWaitHandle.WaitOne(Timeout))
            {
                return MoveNext();
            }
            else
            {
                m_status = ChannelStatus.Overtime;
                return false;
            }
        }

        /// <summary>
        /// 写入通道
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public override void Write(byte[] data, int offset, int length)
        {
            if ((byte)m_status > 3)
            {
                throw new Exception($"通道已{m_status}");
            }

            if (!SpinWait.SpinUntil(() => { return m_canFree; }, Timeout))
            {
                throw new TimeoutException();
            }
            ChannelPackage channelPackage = new ChannelPackage()
            {
                ChannelId = this.m_id,
                DataType = ChannelDataType.DataOrder,
                Data = new ArraySegment<byte>(data, offset, length),
                SourceId = this.m_actor.ID,
                TargetId = this.m_targetId,
                Route = this.m_targetId.HasValue()
            };
            this.m_actor.SendChannelPackage(channelPackage);
            m_lastOperationTime = DateTime.Now;
        }

        internal void ReceivedData(ChannelPackage channelPackage)
        {
            m_lastOperationTime = DateTime.Now;
            if (channelPackage.RunNow)
            {
                switch (channelPackage.DataType)
                {
                    case ChannelDataType.CompleteOrder:
                        this.m_lastOperationMes = channelPackage.Message;
                        RequestComplete(false);
                        break;

                    case ChannelDataType.CancelOrder:
                        this.m_lastOperationMes = channelPackage.Message;
                        RequestCancel(false);
                        break;

                    case ChannelDataType.DisposeOrder:
                        RequestDispose(false);
                        break;

                    case ChannelDataType.HoldOnOrder:
                        this.m_lastOperationMes = channelPackage.Message;
                        m_status = ChannelStatus.HoldOn;
                        break;

                    case ChannelDataType.QueueRun:
                        this.m_canFree = true;
                        return;

                    case ChannelDataType.QueuePause:
                        this.m_canFree = false;
                        return;

                    default:
                        return;
                }
            }
            m_dataQueue.Enqueue(channelPackage);
            m_moveWaitHandle.Set();
        }

        internal void SetID(int id)
        {
            m_id = id;
        }

        internal void SetUsing()
        {
            m_using = true;
        }

        private void Clear()
        {
            try
            {
                lock (this)
                {
                    m_moveWaitHandle.Set();
                    m_moveWaitHandle.SafeDispose();
                    m_actor.RemoveChannel(m_id);
                    m_dataQueue.Clear();
                }
            }
            catch
            {
            }
        }

        private void OnQueueChanged(bool free)
        {
            if ((byte)m_status > 3)
            {
                return;
            }

            try
            {
                ChannelPackage channelPackage = new ChannelPackage()
                {
                    ChannelId = this.m_id,
                    RunNow = true,
                    DataType = free ? ChannelDataType.QueueRun : ChannelDataType.QueuePause,
                    SourceId = this.m_actor.ID,
                    TargetId = this.m_targetId,
                    Route = this.m_targetId.HasValue()
                };
                this.m_actor.SendChannelPackage(channelPackage);
                m_lastOperationTime = DateTime.Now;
            }
            catch
            {
            }
        }

        private void RequestCancel(bool clear)
        {
            m_status = ChannelStatus.Cancel;
            if (clear)
            {
                Clear();
            }
        }

        private void RequestComplete(bool clear)
        {
            m_status = ChannelStatus.Completed;
            if (clear)
            {
                Clear();
            }
        }

        internal void RequestDispose(bool clear)
        {
            if ((byte)m_status > 3)
            {
                return;
            }
            m_status = ChannelStatus.Disposed;
            if (clear)
            {
                Clear();
            }
        }
    }
}