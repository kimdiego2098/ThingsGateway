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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// IDependencyProperty
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IDependencyProperty<out TValue>
    {
        /// <summary>
        /// 默认值
        /// </summary>
        TValue DefauleValue { get; }

        /// <summary>
        /// 属性名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 所属类型
        /// </summary>
        Type Owner { get; }
    }

    /// <summary>
    /// 依赖项属性
    /// </summary>
    [DebuggerDisplay("Name={Name},Type={ValueType}")]
    public class DependencyProperty<TValue> : IDependencyProperty<TValue>
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        protected string m_name;

        /// <summary>
        /// 所属类型
        /// </summary>
        protected Type m_owner;

        private TValue m_value;

        /// <summary>
        /// 依赖项属性
        /// </summary>
        protected DependencyProperty()
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TValue DefauleValue => m_value;

        /// <summary>
        ///<inheritdoc/>
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Type Owner => m_owner;

        /// <summary>
        /// 注册依赖项属性。
        /// <para>依赖属性的默认值，可能会应用于所有的<see cref="IDependencyObject"/></para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="owner"></param>
        /// <param name="value">依赖项属性值，一般该值应该是值类型，因为它可能会被用于多个依赖对象。</param>
        /// <returns></returns>
        public static DependencyProperty<TValue> Register(string propertyName, Type owner, TValue value)
        {
            DependencyProperty<TValue> dp = new DependencyProperty<TValue>
            {
                m_name = propertyName,
                m_owner = owner,
                m_value = value
            };
            return dp;
        }
    }
}