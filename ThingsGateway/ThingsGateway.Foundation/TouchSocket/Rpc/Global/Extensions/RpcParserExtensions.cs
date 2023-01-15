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

namespace ThingsGateway.Foundation.Rpc
{
    /// <summary>
    /// RpcParserExtensions
    /// </summary>
    public static class RpcParserExtensions
    {
        /// <summary>
        /// 获取本地代理
        /// </summary>
        /// <param name="rpcParser"></param>
        /// <param name="namespace"></param>
        /// <param name="attrbuteTypes"></param>
        /// <returns></returns>
        public static string GetProxyCodes(this IRpcParser rpcParser, string @namespace, Type[] attrbuteTypes)
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            return rpcParser.RpcStore.GetProxyCodes(@namespace, attrbuteTypes);
        }

        /// <summary>
        ///  获取本地代理
        /// </summary>
        /// <param name="rpcParser"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static string GetProxyCodes(this IRpcParser rpcParser, string @namespace)
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            return rpcParser.RpcStore.GetProxyCodes(@namespace, null);
        }

        /// <summary>
        /// 注册所有服务
        /// </summary>
        /// <returns>返回搜索到的服务数</returns>
        public static int RegisterAllServer(this IRpcParser rpcParser)
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            return rpcParser.RpcStore.RegisterAllServer();
        }

        /// <summary>
        /// 注册所有服务
        /// </summary>
        /// <returns>返回注册实例</returns>
        public static void RegisterServer<TFrom, TTo>(this IRpcParser rpcParser) where TFrom : class, IRpcServer where TTo : TFrom
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            rpcParser.RpcStore.RegisterServer<TFrom, TTo>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="rpcParser"></param>
        /// <param name="providerInterfaceType"></param>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static void RegisterServer(this IRpcParser rpcParser, Type providerInterfaceType, Type providerType)
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            rpcParser.RpcStore.RegisterServer(providerInterfaceType, providerType);
        }

        /// <summary>
        /// 注册为单例服务
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="rpcParser"></param>
        /// <param name="serverProvider"></param>
        public static void RegisterServer<TFrom>(this IRpcParser rpcParser, TFrom serverProvider) where TFrom : class, IRpcServer
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            rpcParser.RpcStore.RegisterServer<TFrom>(serverProvider);
        }

        /// <summary>
        /// 注册为单例服务
        /// </summary>
        /// <param name="rpcParser"></param>
        /// <param name="providerInterfaceType"></param>
        /// <param name="serverProvider"></param>
        public static void RegisterServer(this IRpcParser rpcParser, Type providerInterfaceType, IRpcServer serverProvider)
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            rpcParser.RpcStore.RegisterServer(providerInterfaceType, serverProvider);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void RegisterServer<T>(this IRpcParser rpcParser) where T : IRpcServer
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            rpcParser.RpcStore.RegisterServer<T>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="rpcParser"></param>
        /// <param name="fromType"></param>
        /// <returns></returns>
        public static void RegisterServer(this IRpcParser rpcParser, Type fromType)
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            rpcParser.RpcStore.RegisterServer(fromType);
        }

        /// <summary>
        /// 移除注册服务
        /// </summary>
        /// <param name="rpcParser"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static int UnregisterServer(this IRpcParser rpcParser, IRpcServer provider)
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            return rpcParser.RpcStore.UnregisterServer(provider);
        }

        /// <summary>
        /// 移除注册服务
        /// </summary>
        /// <param name="rpcParser"></param>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static int UnregisterServer(this IRpcParser rpcParser, Type providerType)
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            return rpcParser.RpcStore.UnregisterServer(providerType);
        }

        /// <summary>
        /// 移除注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int UnregisterServer<T>(this IRpcParser rpcParser) where T : RpcServer
        {
            if (rpcParser.RpcStore == null)
            {
                throw new ArgumentNullException(nameof(rpcParser.RpcStore), $"RpcStore为空，这一般是该解析器没有完成初始化配置所导致的。");
            }
            return rpcParser.RpcStore.UnregisterServer<T>();
        }
    }
}