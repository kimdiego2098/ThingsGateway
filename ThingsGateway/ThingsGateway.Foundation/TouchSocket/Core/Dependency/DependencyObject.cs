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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 依赖对象接口
    /// </summary>
    public interface IDependencyObject : IDisposable
    {
        /// <summary>
        /// 获取依赖注入的值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dp"></param>
        /// <returns></returns>
        public TValue GetValue<TValue>(IDependencyProperty<TValue> dp);

        /// <summary>
        /// 是否有值。
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public bool HasValue<TValue>(IDependencyProperty<TValue> dp);

        /// <summary>
        /// 重置属性值。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dp"></param>
        /// <returns></returns>
        public DependencyObject RemoveValue<TValue>(IDependencyProperty<TValue> dp);

        /// <summary>
        /// 设置依赖注入的值
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        public DependencyObject SetValue<TValue>(IDependencyProperty<TValue> dp, TValue value);
    }

    /// <summary>
    /// 依赖项对象.
    /// 线程安全。
    /// </summary>
    public class DependencyObject : DisposableObject, IDependencyObject, System.IDisposable
    {
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly ConcurrentDictionary<object, object> m_dp;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DependencyObject()
        {
            m_dp = new ConcurrentDictionary<object, object>();
        }

        /// <summary>
        /// 获取依赖注入的值
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public TValue GetValue<TValue>(IDependencyProperty<TValue> dp)
        {
            if (m_dp.TryGetValue(dp, out object value))
            {
                return (TValue)value;
            }
            else
            {
                return dp.DefauleValue;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public bool HasValue<TValue>(IDependencyProperty<TValue> dp)
        {
            return m_dp.ContainsKey(dp);
        }

        /// <summary>
        /// 移除设定值。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dp"></param>
        /// <returns></returns>
        public DependencyObject RemoveValue<TValue>(IDependencyProperty<TValue> dp)
        {
            m_dp.TryRemove(dp, out _);
            return this;
        }

        /// <summary>
        /// 设置依赖注入的值
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        public DependencyObject SetValue<TValue>(IDependencyProperty<TValue> dp, TValue value)
        {
            m_dp.AddOrUpdate(dp, value, (k, v) => v);
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            m_dp.Clear();
            base.Dispose(disposing);
        }
    }
}