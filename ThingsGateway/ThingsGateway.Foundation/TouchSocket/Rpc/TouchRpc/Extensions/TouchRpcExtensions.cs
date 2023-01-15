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
using System.Collections.Generic;

using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// RPC辅助扩展
    /// </summary>
    public static partial class TouchRpcExtensions
    {
        /// <summary>
        /// 创建一个直接向目标地址请求的Rpc客户端。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="targetId"></param>
        public static IRpcClient CreateIDRpcClient<TRpcActor>(this TRpcActor client, string targetId) where TRpcActor : IRpcActor
        {
            return new IDRpcActor(targetId, client);
        }

        #region 批量传输（弃用）

        /// <summary>
        /// 批量拉取文件
        /// </summary>
        /// <param name="client">终端</param>
        /// <param name="multipleCount">并行数量</param>
        /// <param name="fileOperators">批量操作器</param>
        [Obsolete("此方法因为特殊原因已被弃用。")]
        public static void PullFiles(this IRpcActor client, int multipleCount, FileOperator[] fileOperators)
        {
            if (multipleCount < 1)
            {
                throw new Exception("并行数量不能小于1。");
            }

            if (fileOperators is null)
            {
                throw new ArgumentNullException(nameof(fileOperators));
            }

            int index = 0;
            int t = 0;
            int complatedLen = 0;
            List<Task<Result>> results = new List<Task<Result>>();

            LoopAction loopAction = LoopAction.CreateLoopAction(-1, 100, (loop) =>
            {
                int st = multipleCount - t;
                for (int i = 0; i < st; i++)
                {
                    if (index == fileOperators.Length)
                    {
                        break;
                    }
                    Task<Result> result = client.PullFileAsync(fileOperators[index]);
                    results.Add(result);
                    index++;
                    t++;
                }

                List<Task<Result>> cr = new List<Task<Result>>();

                foreach (var item in results)
                {
                    if (item.IsCompleted)
                    {
                        cr.Add(item);
                        t--;
                        complatedLen++;
                        if (complatedLen == fileOperators.Length)
                        {
                            loop.Dispose();
                        }
                    }
                }

                foreach (var item in cr)
                {
                    results.Remove(item);
                }
            });

            loopAction.Run();
        }

        /// <summary>
        /// 异步批量拉取文件
        /// </summary>
        /// <param name="client">终端</param>
        /// <param name="multipleCount">并行数量</param>
        /// <param name="fileOperators">批量操作器</param>
        [Obsolete("此方法因为特殊原因已被弃用。")]
        public static void PullFilesAsync(this IRpcActor client, int multipleCount, FileOperator[] fileOperators)
        {
            if (multipleCount < 1)
            {
                throw new Exception("并行数量不能小于1。");
            }

            if (fileOperators is null)
            {
                throw new ArgumentNullException(nameof(fileOperators));
            }

            int index = 0;
            int t = 0;
            int complatedLen = 0;
            List<Task<Result>> results = new List<Task<Result>>();

            LoopAction loopAction = LoopAction.CreateLoopAction(-1, 100, (loop) =>
            {
                int st = multipleCount - t;
                for (int i = 0; i < st; i++)
                {
                    if (index == fileOperators.Length)
                    {
                        break;
                    }
                    Task<Result> result = client.PullFileAsync(fileOperators[index]);
                    results.Add(result);
                    index++;
                    t++;
                }

                List<Task<Result>> cr = new List<Task<Result>>();

                foreach (var item in results)
                {
                    if (item.IsCompleted)
                    {
                        cr.Add(item);
                        t--;
                        complatedLen++;
                        if (complatedLen == fileOperators.Length)
                        {
                            loop.Dispose();
                        }
                    }
                }

                foreach (var item in cr)
                {
                    results.Remove(item);
                }
            });

            loopAction.RunAsync();
        }

        /// <summary>
        /// 批量推送文件
        /// </summary>
        /// <param name="client">终端</param>
        /// <param name="multipleCount">并行数量</param>
        /// <param name="fileOperators">批量操作器</param>
        [Obsolete("此方法因为特殊原因已被弃用。")]
        public static void PushFiles(this IRpcActor client, int multipleCount, FileOperator[] fileOperators)
        {
            if (multipleCount < 1)
            {
                throw new Exception("并行数量不能小于1。");
            }

            if (fileOperators is null)
            {
                throw new ArgumentNullException(nameof(fileOperators));
            }

            int index = 0;
            int t = 0;
            int complatedLen = 0;
            List<Task<Result>> results = new List<Task<Result>>();

            LoopAction loopAction = LoopAction.CreateLoopAction(-1, 100, (loop) =>
            {
                int st = multipleCount - t;
                for (int i = 0; i < st; i++)
                {
                    if (index == fileOperators.Length)
                    {
                        break;
                    }
                    Task<Result> result = client.PushFileAsync(fileOperators[index]);
                    results.Add(result);
                    index++;
                    t++;
                }

                List<Task<Result>> cr = new List<Task<Result>>();

                foreach (var item in results)
                {
                    if (item.IsCompleted)
                    {
                        cr.Add(item);
                        t--;
                        complatedLen++;
                        if (complatedLen == fileOperators.Length)
                        {
                            loop.Dispose();
                        }
                    }
                }

                foreach (var item in cr)
                {
                    results.Remove(item);
                }
            });

            loopAction.Run();
        }

        /// <summary>
        /// 异步批量推送文件
        /// </summary>
        /// <param name="client">终端</param>
        /// <param name="multipleCount">并行数量</param>
        /// <param name="fileOperators">批量操作器</param>
        [Obsolete("此方法因为特殊原因已被弃用。")]
        public static void PushFilesAsync(this IRpcActor client, int multipleCount, FileOperator[] fileOperators)
        {
            if (multipleCount < 1)
            {
                throw new Exception("并行数量不能小于1。");
            }

            if (fileOperators is null)
            {
                throw new ArgumentNullException(nameof(fileOperators));
            }

            int index = 0;
            int t = 0;
            int complatedLen = 0;
            List<Task<Result>> results = new List<Task<Result>>();

            LoopAction loopAction = LoopAction.CreateLoopAction(-1, 100, (loop) =>
            {
                int st = multipleCount - t;
                for (int i = 0; i < st; i++)
                {
                    if (index == fileOperators.Length)
                    {
                        break;
                    }
                    Task<Result> result = client.PushFileAsync(fileOperators[index]);
                    results.Add(result);
                    index++;
                    t++;
                }

                List<Task<Result>> cr = new List<Task<Result>>();

                foreach (var item in results)
                {
                    if (item.IsCompleted)
                    {
                        cr.Add(item);
                        t--;
                        complatedLen++;
                        if (complatedLen == fileOperators.Length)
                        {
                            loop.Dispose();
                        }
                    }
                }

                foreach (var item in cr)
                {
                    results.Remove(item);
                }
            });

            loopAction.RunAsync();
        }

        #endregion 批量传输（弃用）

        /// <summary>
        /// 转化Protocol协议标识为TouchRpc
        /// </summary>
        /// <param name="client"></param>
        public static void SwitchProtocolToTouchRpc(this ITcpClientBase client)
        {
            client.SetDataHandlingAdapter(new FixedHeaderPackageAdapter());
            client.Protocol = TouchRpcUtility.TouchRpcProtocol;
        }

        /// <summary>
        /// 转为ResultCode
        /// </summary>
        /// <param name="channelStatus"></param>
        /// <returns></returns>
        public static ResultCode ToResultCode(this ChannelStatus channelStatus)
        {
            switch (channelStatus)
            {
                case ChannelStatus.Default:
                    return ResultCode.Default;

                case ChannelStatus.Overtime:
                    return ResultCode.Overtime;

                case ChannelStatus.Cancel:
                    return ResultCode.Canceled;

                case ChannelStatus.Completed:
                    return ResultCode.Success;

                case ChannelStatus.Moving:
                case ChannelStatus.Disposed:
                default:
                    return ResultCode.Error;
            }
        }
    }
}