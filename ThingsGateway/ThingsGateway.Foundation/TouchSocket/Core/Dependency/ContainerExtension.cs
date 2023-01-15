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
    /// IContainerExtensions
    /// </summary>
    public static class ContainerExtension
    {
        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="container"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IContainer RegisterSingleton<TFrom, TTo>(this IContainer container, TTo instance)
              where TFrom : class
              where TTo : class, TFrom
        {
            RegisterSingleton(container, typeof(TFrom), instance);
            return container;
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <param name="container"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IContainer RegisterSingleton(this IContainer container, object instance)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            RegisterSingleton(container, instance.GetType(), instance);
            return container;
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IContainer RegisterSingleton<TFrom, TTo>(this IContainer container, string key, TTo instance)
              where TFrom : class
              where TTo : class, TFrom
        {
            RegisterSingleton(container, typeof(TFrom), instance, key);
            return container;
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fromType"></param>
        /// <param name="instance"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer RegisterSingleton(this IContainer container, Type fromType, object instance, string key = "")
        {
            container.Register(new DependencyDescriptor(fromType, instance), key);
            return container;
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="container"></param>
        /// <param name="instance"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer RegisterSingleton<TFrom>(this IContainer container, object instance, string key = "")
        {
            container.Register(new DependencyDescriptor(typeof(TFrom), instance), key);
            return container;
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <param name="container"></param>
        /// <param name="instance"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer RegisterSingleton(this IContainer container, object instance, string key = "")
        {
            container.Register(new DependencyDescriptor(instance.GetType(), instance), key);
            return container;
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer RegisterSingleton<TFrom>(this IContainer container, string key = "")
        {
            container.Register(new DependencyDescriptor(typeof(TFrom), typeof(TFrom), Lifetime.Singleton), key);
            return container;
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer RegisterSingleton(this IContainer container, Type fromType, Type toType, string key = "")
        {
            container.Register(new DependencyDescriptor(fromType, toType, Lifetime.Singleton), key);
            return container;
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTO"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static IContainer RegisterSingleton<TFrom, TTO>(this IContainer container)
             where TFrom : class
             where TTO : class, TFrom
        {
            RegisterSingleton(container, typeof(TFrom), typeof(TTO));
            return container;
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTO"></typeparam>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer RegisterSingleton<TFrom, TTO>(this IContainer container, string key)
             where TFrom : class
             where TTO : class, TFrom
        {
            RegisterSingleton(container, typeof(TFrom), typeof(TTO), key);
            return container;
        }

        #region Transient

        /// <summary>
        /// 注册临时映射
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTO"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static IContainer RegisterTransient<TFrom, TTO>(this IContainer container)
             where TFrom : class
             where TTO : class, TFrom
        {
            RegisterTransient(container, typeof(TFrom), typeof(TTO));
            return container;
        }

        /// <summary>
        /// 注册临时映射
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static IContainer RegisterTransient<TFrom>(this IContainer container)
             where TFrom : class
        {
            RegisterTransient(container, typeof(TFrom), typeof(TFrom));
            return container;
        }

        /// <summary>
        /// 注册临时映射
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTO"></typeparam>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer RegisterTransient<TFrom, TTO>(this IContainer container, string key)
            where TFrom : class
            where TTO : class, TFrom
        {
            RegisterTransient(container, typeof(TFrom), typeof(TTO), key);
            return container;
        }

        /// <summary>
        /// 注册临时映射
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer RegisterTransient(this IContainer container, Type fromType, Type toType, string key = "")
        {
            container.Register(new DependencyDescriptor(fromType, toType, Lifetime.Transient), key);
            return container;
        }

        #endregion Transient

        #region Scoped

        /// <summary>
        /// 注册区域映射
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTO"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static IContainer RegisterScoped<TFrom, TTO>(this IContainer container)
             where TFrom : class
             where TTO : class, TFrom
        {
            RegisterScoped(container, typeof(TFrom), typeof(TTO));
            return container;
        }

        /// <summary>
        /// 注册区域映射
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static IContainer RegisterScoped<TFrom>(this IContainer container)
             where TFrom : class
        {
            RegisterScoped(container, typeof(TFrom), typeof(TFrom));
            return container;
        }

        /// <summary>
        /// 注册区域映射
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTO"></typeparam>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer RegisterScoped<TFrom, TTO>(this IContainer container, string key)
             where TFrom : class
             where TTO : class, TFrom
        {
            RegisterScoped(container, typeof(TFrom), typeof(TTO), key);
            return container;
        }

        /// <summary>
        /// 注册区域映射
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer RegisterScoped(this IContainer container, Type fromType, Type toType, string key = "")
        {
            container.Register(new DependencyDescriptor(fromType, toType, Lifetime.Scoped), key);
            return container;
        }

        #endregion Scoped

        /// <summary>
        /// 创建类型对应的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="ps"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Resolve<T>(this IContainerProvider container, object[] ps, string key = "")
        {
            return (T)container.Resolve(typeof(T), ps, key);
        }

        /// <summary>
        /// 创建类型对应的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static T Resolve<T>(this IContainerProvider container)
        {
            return Resolve<T>(container, null);
        }

        /// <summary>
        /// 创建类型对应的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Resolve<T>(this IContainerProvider container, string key)
        {
            return Resolve<T>(container, null, key);
        }

        #region Unregister

        /// <summary>
        /// 移除注册信息
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fromType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer Unregister(this IContainer container, Type fromType, string key = "")
        {
            container.Unregister(new DependencyDescriptor(fromType), key);
            return container;
        }

        /// <summary>
        /// 移除注册信息
        /// </summary>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer Unregister<TFrom>(this IContainer container, string key = "")
        {
            container.Unregister(new DependencyDescriptor(typeof(TFrom)), key);
            return container;
        }

        #endregion Unregister
    }
}