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
    /// 缓存实体
    /// </summary>
    public class CacheEntry<TKey, TValue> : ICacheEntry<TKey, TValue>
    {
        /// <summary>
        /// 缓存实体
        /// </summary>
        /// <param name="key"></param>
        public CacheEntry(TKey key) : this(key, default)
        {
        }

        /// <summary>
        /// 缓存实体
        /// </summary>
        public CacheEntry(TKey key, TValue value)
        {
            UpdateTime = DateTime.Now;
            Duration = TimeSpan.FromSeconds(60);
            Key = key;
            Value = value;
        }

        /// <summary>
        /// 有效区间。如果想长期有效，请使用<see cref="TimeSpan.Zero"/>
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// 键
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public TValue Value { get; set; }
    }
}