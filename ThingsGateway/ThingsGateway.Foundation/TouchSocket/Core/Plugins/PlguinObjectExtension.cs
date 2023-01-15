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
    /// PlguinObjectExtension
    /// </summary>
    public static class PlguinObjectExtension
    {
        /// <summary>
        /// 添加插件
        /// </summary>
        /// <typeparam name="TPlugin">插件类型</typeparam>
        /// <returns>插件类型实例</returns>
        public static TPlugin AddPlugin<TPlugin>(this IPluginObject plguinObject) where TPlugin : class, IPlugin
        {
            plguinObject.Container.RegisterSingleton<TPlugin>();
            var obj = plguinObject.Container.Resolve<TPlugin>();
            AddPlugin(plguinObject, obj);
            return obj;
        }

        /// <summary>
        /// 添加插件
        /// </summary>
        /// <param name="plguinObject"></param>
        /// <param name="plugin">插件</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddPlugin(this IPluginObject plguinObject, IPlugin plugin)
        {
            plguinObject.Container.RegisterSingleton(plugin);
            plguinObject.PluginsManager.Add(plugin);
        }

        /// <summary>
        /// 清空插件
        /// </summary>
        public static void ClearPlugins(this IPluginObject plguinObject)
        {
            plguinObject.PluginsManager.Clear();
        }

        /// <summary>
        /// 移除插件
        /// </summary>
        /// <param name="plguinObject"></param>
        /// <param name="plugin"></param>
        public static void RemovePlugin(this IPluginObject plguinObject, IPlugin plugin)
        {
            plguinObject.PluginsManager.Remove(plugin);
        }

        /// <summary>
        /// 移除插件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="plguinObject"></param>
        public static void RemovePlugin<T>(this IPluginObject plguinObject) where T : IPlugin
        {
            plguinObject.PluginsManager.Remove(typeof(T));
        }
    }
}