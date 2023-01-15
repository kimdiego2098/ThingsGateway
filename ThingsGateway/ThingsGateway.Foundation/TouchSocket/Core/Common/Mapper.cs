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
using System.Reflection;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 映射数据
    /// </summary>
    public static class Mapper
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, Property>> m_typeToProperty = new ConcurrentDictionary<Type, Dictionary<string, Property>>();

        /// <summary>
        /// 简单映射
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget Map<TTarget>(this object source) where TTarget : class, new()
        {
            return (TTarget)Map(source, typeof(TTarget));
        }

        /// <summary>
        /// 简单映射
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget Map<TSource, TTarget>(this TSource source) where TTarget : class, new()
        {
            return (TTarget)Map(source, typeof(TTarget));
        }

        /// <summary>
        /// 简单对象映射
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object Map(this object source, Type targetType)
        {
            return Map(source, Activator.CreateInstance(targetType));
        }

        /// <summary>
        /// 简单对象映射
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object Map(this object source, object target)
        {
            if (source is null)
            {
                return default;
            }
            var sourceType = source.GetType();
            if (sourceType.IsPrimitive || sourceType.IsEnum || sourceType == ThingsGateway.Foundation.Core.TouchSocketCoreUtility.stringType)
            {
                return source;
            }
            var sourcePairs = m_typeToProperty.GetOrAdd(sourceType, (k) =>
               {
                   Dictionary<string, Property> pairs = new Dictionary<string, Property>();
                   var ps = k.GetProperties(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                   foreach (var item in ps)
                   {
                       pairs.Add(item.Name, new Property(item));
                   }
                   return pairs;
               });

            var targetPairs = m_typeToProperty.GetOrAdd(target.GetType(), (k) =>
            {
                Dictionary<string, Property> pairs = new Dictionary<string, Property>();
                var ps = k.GetProperties(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var item in ps)
                {
                    pairs.Add(item.Name, new Property(item));
                }
                return pairs;
            });
            foreach (var item in targetPairs)
            {
                if (item.Value.CanWrite)
                {
                    if (sourcePairs.TryGetValue(item.Key, out Property property))
                    {
                        if (property.CanRead)
                        {
                            item.Value.SetValue(target, property.GetValue(source));
                        }
                    }
                }
            }
            return target;
        }

        /// <summary>
        /// 映射List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T1> MapList<T, T1>(this IEnumerable<T> list) where T : class where T1 : class, new()
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            List<T1> result = new List<T1>();
            foreach (var item in list)
            {
                result.Add(Map<T, T1>(item));
            }
            return result;
        }

        /// <summary>
        /// 映射List
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T1> MapList<T1>(this IEnumerable<object> list) where T1 : class, new()
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            List<T1> result = new List<T1>();
            foreach (var item in list)
            {
                result.Add(Map<T1>(item));
            }
            return result;
        }
    }
}