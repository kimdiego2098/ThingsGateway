using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ThingsGateway.Foundation.Extension
{
    public static class ConcurrentQueueExtensions
    {
        /// <summary>
        /// 批量出队
        /// </summary>
        public static List<T> ToListWithDequeue<T>(this IntelligentConcurrentQueue<T> values, int conut)
        {
            List<T> newlist = new();
            for (int i = 0; i < conut; i++)
            {
                if (values.TryDequeue(out T result))
                {
                    newlist.Add(result);
                }
                else
                {
                    break;
                }
            }
            return newlist;

        }

#if !NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// 清除所有成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            while (queue.TryDequeue(out _))
            {
            }
        }

#endif

        /// <summary>
        /// 清除所有成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="action"></param>
        public static void Clear<T>(this ConcurrentQueue<T> queue, Action<T> action)
        {
            while (queue.TryDequeue(out T t))
            {
                action?.Invoke(t);
            }
        }
    }
}