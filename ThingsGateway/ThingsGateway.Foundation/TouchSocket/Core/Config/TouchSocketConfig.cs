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
    /// 配置文件基类
    /// </summary>
    public class TouchSocketConfig : DependencyObject
    {
        //private bool built;
        private IContainer m_container;

        private IPluginsManager m_pluginsManager;

        //ConcurrentQueue<Tuple<Delegate, object[]>> actions = new ConcurrentQueue<Tuple<Delegate, object[]>>();

        ///// <summary>
        ///// 添加构建委托，该委托会在<see cref="Build"/>时调用。
        ///// </summary>
        ///// <param name="action"></param>
        ///// <param name="ps"></param>
        //public void AddBuildAction(Delegate action,params object[] ps)
        //{
        //    actions.Enqueue(Tuple.Create(action,ps)) ;
        //}

        ///// <summary>
        ///// 构建配置
        ///// </summary>
        //public void Build()
        //{
        //    if (!built)
        //    {
        //        built = true;
        //        while (actions.TryDequeue(out var action))
        //        {
        //            action.Item1.DynamicInvoke(action.Item2);
        //        }
        //    }
        //}

        /// <summary>
        /// 构造函数
        /// </summary>
        public TouchSocketConfig()
        {
            SetContainer(new Container());
        }

        /// <summary>
        /// IOC容器。
        /// </summary>
        public IContainer Container => m_container;

        /// <summary>
        /// 使用插件
        /// </summary>
        public bool IsUsePlugin { get; set; }

        /// <summary>
        /// 插件管理器
        /// </summary>
        public IPluginsManager PluginsManager => m_pluginsManager;

        /// <summary>
        /// 设置注入容器。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TouchSocketConfig SetContainer(IContainer value)
        {
            m_container = value;
            if (!value.IsRegistered(typeof(ILog)))
            {
                m_container.RegisterSingleton<ILog, LoggerGroup>();
            }
            SetPluginsManager(new PluginsManager(m_container));
            return this;
        }

        /// <summary>
        /// 设置PluginsManager
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TouchSocketConfig SetPluginsManager(IPluginsManager value)
        {
            m_pluginsManager = value;
            m_container.RegisterSingleton<IPluginsManager>(value);
            return this;
        }

        /// <summary>
        /// 启用插件
        /// </summary>
        /// <returns></returns>
        public TouchSocketConfig UsePlugin()
        {
            IsUsePlugin = true;
            return this;
        }
    }
}