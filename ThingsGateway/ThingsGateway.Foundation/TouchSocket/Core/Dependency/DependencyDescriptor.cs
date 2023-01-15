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
    /// 注入依赖对象
    /// </summary>
    public class DependencyDescriptor
    {
        /// <summary>
        /// 初始化一个单例实例。
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="instance"></param>
        public DependencyDescriptor(Type fromType, object instance)
        {
            FromType = fromType;
            ToInstance = instance;
            Lifetime = Lifetime.Singleton;
            ToType = instance.GetType();
        }

        /// <summary>
        /// 初始化一个完整的服务注册
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <param name="lifetime"></param>
        public DependencyDescriptor(Type fromType, Type toType, Lifetime lifetime)
        {
            FromType = fromType;
            Lifetime = lifetime;
            ToType = toType;
        }

        /// <summary>
        /// 初始化一个简单的的服务描述
        /// </summary>
        /// <param name="fromType"></param>
        public DependencyDescriptor(Type fromType)
        {
            FromType = fromType;
        }

        /// <summary>
        /// 实例化工厂委托
        /// </summary>
        public Func<IContainer, object> ImplementationFactory { get; set; }

        /// <summary>
        /// 实例类型
        /// </summary>
        public Type ToType { get; }

        /// <summary>
        /// 实例
        /// </summary>
        public object ToInstance { get; set; }

        /// <summary>
        /// 生命周期
        /// </summary>
        public Lifetime Lifetime { get; }

        /// <summary>
        /// 注册类型
        /// </summary>
        public Type FromType { get; }
    }
}