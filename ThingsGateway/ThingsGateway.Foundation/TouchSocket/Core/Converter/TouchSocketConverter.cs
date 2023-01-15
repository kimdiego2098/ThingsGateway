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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 转换器
    /// </summary>
    public class TouchSocketConverter<TSource>
    {
        private readonly List<IConverter<TSource>> m_converters = new List<IConverter<TSource>>();

        /// <summary>
        /// 添加插件
        /// </summary>
        /// <param name="converter">插件</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(IConverter<TSource> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException();
            }
            foreach (var item in m_converters)
            {
                if (item.GetType() == converter.GetType())
                {
                    return;
                }
            }

            m_converters.Add(converter);

            m_converters.Sort(delegate (IConverter<TSource> x, IConverter<TSource> y)
            {
                if (x.Order == y.Order) return 0;
                else if (x.Order > y.Order) return 1;
                else return -1;
            });
        }

        /// <summary>
        /// 清除所有转化器
        /// </summary>
        public void Clear()
        {
            m_converters.Clear();
        }

        /// <summary>
        /// 将源数据转换目标类型对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public object ConvertFrom(TSource source, Type targetType)
        {
            object result;
            foreach (var item in m_converters)
            {
                if (item.TryConvertFrom(source, targetType, out result))
                {
                    return result;
                }
            }

            throw new Exception($"{source}无法转换为{targetType}类型。");
        }

        /// <summary>
        /// 将目标类型对象转换源数据
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public TSource ConvertTo(object target)
        {
            foreach (var item in m_converters)
            {
                if (item.TryConvertTo(target, out TSource source))
                {
                    return source;
                }
            }

            throw new Exception($"{target}无法转换为{typeof(TSource)}类型。");
        }

        /// <summary>
        /// 移除插件
        /// </summary>
        /// <param name="converter"></param>
        public void Remove(IConverter<TSource> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException();
            }
            m_converters.Remove(converter);
        }

        /// <summary>
        /// 移除插件
        /// </summary>
        /// <param name="type"></param>
        public void Remove(Type type)
        {
            for (int i = m_converters.Count - 1; i >= 0; i--)
            {
                IConverter<TSource> plugin = m_converters[i];
                if (plugin.GetType() == type)
                {
                    m_converters.RemoveAt(i);
                }
            }
        }
    }
}