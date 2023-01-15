using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ThingsGateway.Foundation.Extension
{
    /// <summary>
    /// DictionaryExtension
    /// </summary>
    public static class DictionaryExtension
    {
        #region 字典扩展
        /// <summary>
        /// 移除满足条件的项目。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="pairs"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static int Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> pairs, Func<KeyValuePair<TKey, TValue>, bool> func)
        {
            List<TKey> list = new List<TKey>();
            foreach (var item in pairs)
            {
                if (func?.Invoke(item) == true)
                {
                    list.Add(item.Key);
                }
            }

            int count = 0;
            foreach (var item in list)
            {
                if (pairs.TryRemove(item, out _))
                {
                    count++;
                }
            }
            return count;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER

        /// <summary>
        /// 尝试添加
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="tkey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryAdd<Tkey, TValue>(this Dictionary<Tkey, TValue> dictionary, Tkey tkey, TValue value)
        {
            if (dictionary.ContainsKey(tkey))
            {
                return false;
            }
            dictionary.Add(tkey, value);
            return true;
        }

#endif

        /// <summary>
        /// 尝试添加
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="tkey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void AddOrUpdate<Tkey, TValue>(this Dictionary<Tkey, TValue> dictionary, Tkey tkey, TValue value)
        {
            if (dictionary.ContainsKey(tkey))
            {
                dictionary[tkey] = value;
            }
            else
            {
                dictionary.Add(tkey, value);
            }
        }

        /// <summary>
        /// 获取值。如果键不存在，则返回默认值。
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="tkey"></param>
        /// <returns></returns>
        public static TValue GetValue<Tkey, TValue>(this Dictionary<Tkey, TValue> dictionary, Tkey tkey)
        {
            if (dictionary.TryGetValue(tkey, out TValue value))
            {
                return value;
            }
            else
            {
                return default;
            }
        }

        #endregion 字典扩展
    }
}