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
    /// PluginsManagerExtension
    /// </summary>
    public static class PluginsManagerExtension
    {
        /// <summary>
        /// 添加插件
        /// </summary>
        /// <typeparam name="TPlugin">插件类型</typeparam>
        /// <returns>插件类型实例</returns>
        public static TPlugin Add<TPlugin>(this IPluginsManager pluginsManager) where TPlugin : class, IPlugin
        {
            pluginsManager.Container.RegisterSingleton<TPlugin>();
            var obj = pluginsManager.Container.Resolve<TPlugin>();
            pluginsManager.Add(obj);
            return obj;
        }

        /// <summary>
        /// 添加插件
        /// </summary>
        /// <typeparam name="TPlugin">插件类型</typeparam>
        /// <param name="pluginsManager"></param>
        /// <param name="ps">创建插件相关构造函数插件</param>
        /// <returns>插件类型实例</returns>
        public static TPlugin Add<TPlugin>(this IPluginsManager pluginsManager, params object[] ps) where TPlugin : class, IPlugin
        {
            pluginsManager.Container.RegisterSingleton<TPlugin>();
            var obj = pluginsManager.Container.Resolve<TPlugin>(ps);
            pluginsManager.Add(obj);
            return obj;
        }

        /// <summary>
        /// 清空插件
        /// </summary>
        public static void Clear(this IPluginsManager pluginsManager)
        {
            pluginsManager.Clear();
        }

        /// <summary>
        /// 移除插件
        /// </summary>
        /// <param name="pluginsManager"></param>
        /// <param name="plugin"></param>
        public static void Remove(this IPluginsManager pluginsManager, IPlugin plugin)
        {
            pluginsManager.Remove(plugin);
        }

        /// <summary>
        /// 移除插件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pluginsManager"></param>
        public static void Remove<T>(this IPluginsManager pluginsManager) where T : IPlugin
        {
            pluginsManager.Remove(typeof(T));
        }
    }
}