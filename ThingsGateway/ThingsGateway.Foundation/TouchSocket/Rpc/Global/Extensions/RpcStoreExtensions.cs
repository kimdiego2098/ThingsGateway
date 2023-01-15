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
using System.Linq;

namespace ThingsGateway.Foundation.Rpc
{
    /// <summary>
    /// RpcStoreExtensions
    /// </summary>
    public static class RpcStoreExtensions
    {
        /// <summary>
        /// 注册所有服务
        /// </summary>
        /// <returns>返回搜索到的服务数</returns>
        public static int RegisterAllServer(this RpcStore rpcStore)
        {
            List<Type> types = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    Type[] t1 = assembly.GetTypes().Where(p => typeof(IRpcServer).IsAssignableFrom(p) && !p.IsAbstract && p.IsClass).ToArray();
                    types.AddRange(t1);
                }
                catch
                {
                }
            }

            foreach (Type type in types)
            {
                rpcStore.RegisterServer(type);
            }
            return types.Count;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void RegisterServer<T>(this RpcStore rpcStore) where T : IRpcServer
        {
            rpcStore.RegisterServer(typeof(T));
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="rpcStore"></param>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static void RegisterServer(this RpcStore rpcStore, Type providerType)
        {
            rpcStore.RegisterServer(providerType, providerType);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns></returns>
        public static void RegisterServer<TFrom, TTo>(this RpcStore rpcStore) where TFrom : class, IRpcServer where TTo : TFrom
        {
            rpcStore.RegisterServer(typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// 注册为单例服务
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <returns></returns>
        public static void RegisterServer<TFrom>(this RpcStore rpcStore, TFrom rpcServer) where TFrom : class, IRpcServer
        {
            rpcStore.RegisterServer(typeof(TFrom), rpcServer);
        }
    }
}