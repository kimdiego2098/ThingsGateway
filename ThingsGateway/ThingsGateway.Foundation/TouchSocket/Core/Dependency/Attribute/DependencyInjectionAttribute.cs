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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 指定依赖类型。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyTypeAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个依赖类型。当确定某个类型仅以某种特定方式注入时，可以过滤不必要的注入操作，以提高效率。
        /// </summary>
        /// <param name="type">可以叠加位域</param>
        public DependencyTypeAttribute(DependencyType type)
        {
            Type = type;
        }

        /// <summary>
        /// 支持类型。
        /// </summary>
        public DependencyType Type { get; }
    }

    /// <summary>
    /// 依赖注入类型。
    /// </summary>
    public enum DependencyType
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        Constructor = 1,

        /// <summary>
        /// 属性
        /// </summary>
        Property = 2,

        /// <summary>
        /// 方法
        /// </summary>
        Method = 4
    }

    /// <summary>
    /// 指定依赖类型，构造函数，可用于构造函数，属性，方法。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Method)]
    public class DependencyInjectAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个依赖注入对象。并且指定构造参数。
        /// <para>当创建时也指定参数时，会覆盖该设定。</para>
        /// </summary>
        /// <param name="ps"></param>
        public DependencyInjectAttribute(params object[] ps)
        {
            Ps = ps;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resolveNullIfNoRegistered"></param>
        public DependencyInjectAttribute(bool resolveNullIfNoRegistered)
        {
            ResolveNullIfNoRegistered = resolveNullIfNoRegistered;
        }

        /// <summary>
        /// 构造参数
        /// </summary>
        public object[] Ps { get; }

        /// <summary>
        /// 如果没有注册则返回为空
        /// </summary>
        public bool ResolveNullIfNoRegistered { get; set; }
    }

    /// <summary>
    /// 参数，属性指定性注入。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class DependencyParamterInjectAttribute : DependencyInjectAttribute
    {
        /// <summary>
        /// 参数，属性指定性注入。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ps"></param>
        public DependencyParamterInjectAttribute(string key, params object[] ps) : base(ps)
        {
            Key = key;
        }

        /// <summary>
        /// 类型，参数，属性指定性注入。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="ps"></param>
        public DependencyParamterInjectAttribute(Type type, string key, params object[] ps) : base(ps)
        {
            Key = key;
            Type = type;
        }

        /// <summary>
        /// 类型，参数，属性指定性注入。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ps"></param>
        public DependencyParamterInjectAttribute(Type type, params object[] ps) : base(ps)
        {
            Key = string.Empty;
            Type = type;
        }

        /// <summary>
        /// 注入类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resolveNullIfNoRegistered"></param>
        public DependencyParamterInjectAttribute(bool resolveNullIfNoRegistered)
        {
            ResolveNullIfNoRegistered = resolveNullIfNoRegistered;
        }

        /// <summary>
        /// 指定键。
        /// </summary>
        public string Key { get; }
    }
}