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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 插件管理器接口
    /// </summary>
    public interface IPluginsManager : IEnumerable<IPlugin>
    {
        /// <summary>
        /// 标识该插件是否可用。当不可用时，仅可以添加和删除插件，但不会触发插件
        /// </summary>
        bool Enable { get; set; }

        /// <summary>
        /// 内置IOC容器
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        /// 添加插件
        /// </summary>
        /// <param name="plugin">插件</param>
        /// <exception cref="ArgumentNullException"></exception>
        void Add(IPlugin plugin);

        /// <summary>
        /// 移除插件
        /// </summary>
        /// <param name="plugin"></param>
        void Remove(IPlugin plugin);

        /// <summary>
        /// 移除插件
        /// </summary>
        /// <param name="type"></param>
        void Remove(Type type);

        /// <summary>
        /// 清除所有插件
        /// </summary>
        void Clear();

        /// <summary>
        /// 触发对应方法
        /// </summary>
        /// <typeparam name="TPlugin">接口类型</typeparam>
        /// <param name="name">触发名称</param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        bool Raise<TPlugin>(string name, object sender, TouchSocketEventArgs e) where TPlugin : IPlugin;
    }
}