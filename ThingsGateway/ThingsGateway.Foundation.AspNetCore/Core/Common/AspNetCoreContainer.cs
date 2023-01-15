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
using Microsoft.Extensions.DependencyInjection;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Foundation.AspNetCore
{
    /// <summary>
    /// AspNetCoreContainer
    /// </summary>
    public class AspNetCoreContainer : IContainer
    {
        private readonly IServiceCollection m_services;

        /// <summary>
        /// 初始化一个IServiceCollection的容器。
        /// </summary>
        /// <param name="services"></param>
        public AspNetCoreContainer(IServiceCollection services)
        {
            services.AddSingleton<IContainer>(this);
            m_services = services;
        }

        /// <summary>
        /// 返回迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DependencyDescriptor> GetEnumerator()
        {
            return m_services.ToList().Select(s =>
              {
                  DependencyDescriptor descriptor = new DependencyDescriptor(s.ServiceType, s.ImplementationType, (Lifetime)(int)s.Lifetime);
                  descriptor.ToInstance = s.ImplementationInstance;
                  return descriptor;
              }).GetEnumerator();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsRegistered(Type fromType, string key = "")
        {
            if (fromType.IsGenericType)
            {
                fromType = fromType.GetGenericTypeDefinition();
            }
            var array = m_services.ToArray();
            foreach (var item in array)
            {
                if (item.ServiceType == fromType)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="key"></param>
        public void Register(DependencyDescriptor descriptor, string key = "")
        {
            if (!key.IsNullOrEmpty())
            {
                throw new NotSupportedException($"{GetType().Name}不支持包含Key的多实现注入");
            }
            if (IsRegistered(descriptor.FromType))
            {
                this.Unregister(descriptor.FromType);
            }
            switch (descriptor.Lifetime)
            {
                case Lifetime.Singleton:
                    if (descriptor.ToInstance != null)
                    {
                        m_services.AddSingleton(descriptor.FromType, descriptor.ToInstance);
                    }
                    else
                    {
                        m_services.AddSingleton(descriptor.FromType, descriptor.ToType);
                    }
                    break;

                case Lifetime.Transient:
                default:
                    m_services.AddTransient(descriptor.FromType, descriptor.ToType);
                    break;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="ps"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Resolve(Type fromType, object[] ps = null, string key = "")
        {
            if (fromType == typeof(IContainer))
            {
                return this;
            }
            if (fromType == typeof(IServiceProvider))
            {
                return m_services.BuildServiceProvider();
            }
            ServiceProvider provider = m_services.BuildServiceProvider();

            return provider.GetService(fromType);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="key"></param>
        public void Unregister(DependencyDescriptor descriptor, string key = "")
        {
            var array = m_services.ToArray();
            foreach (var item in array)
            {
                if (item.ServiceType == descriptor.FromType)
                {
                    m_services.Remove(item);
                    return;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}