﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using NewLife.Reflection;
using NewLife.Threading;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

//#nullable enable
namespace NewLife.Caching;

/// <summary>默认字典缓存</summary>
public class MemoryCache : Cache
{
    #region 属性

    /// <summary>缓存核心</summary>
    protected ConcurrentDictionary<String, CacheItem> _cache = new();

    /// <summary>缓存键过期</summary>
    public event EventHandler<EventArgs<String>>? KeyExpired;

    /// <summary>容量。容量超标时，采用LRU机制删除，默认100_000</summary>
    public Int32 Capacity { get; set; } = 100_000;

    /// <summary>定时清理时间，默认60秒</summary>
    public Int32 Period { get; set; } = 60;

    #endregion 属性

    #region 静态默认实现

    /// <summary>默认缓存</summary>
    public static ICache Instance { get; set; } = new MemoryCache();

    #endregion 静态默认实现

    #region 构造

    /// <summary>实例化一个内存字典缓存</summary>
    public MemoryCache()
    {
        //_cache = new ConcurrentDictionary<String, CacheItem>();
        Name = "Memory";

        Init(null);
    }

    /// <summary>销毁</summary>
    /// <param name="disposing"></param>
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);

        _clearTimer.TryDispose();
        _clearTimer = null;
    }

    #endregion 构造

    #region 缓存属性

    private Int32 _count;

    /// <summary>缓存项。原子计数</summary>
    public override Int32 Count => _count;

    /// <summary>所有键。实际返回只读列表新实例，数据量较大时注意性能</summary>
    public override ICollection<String> Keys => _cache.Keys;

    #endregion 缓存属性

    #region 方法

    /// <summary>返回全部</summary>
    public IReadOnlyDictionary<string, CacheItem> GetAll() => _cache;

    /// <summary>获取或添加缓存项</summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="expire">过期时间，秒</param>
    /// <returns></returns>
    public virtual T? GetOrAdd<T>(String key, T value, Int32 expire = -1)
    {
        if (expire < 0) expire = Expire;

        CacheItem? ci = null;
        do
        {
            if (_cache.TryGetValue(key, out var item)) return (T?)item.Visit();

            ci ??= new CacheItem(value, expire);
        } while (!_cache.TryAdd(key, ci));

        Interlocked.Increment(ref _count);

        return (T?)ci.Visit();
    }

    /// <summary>初始化配置</summary>
    /// <param name="config"></param>
    public override void Init(String? config)
    {
        if (_clearTimer == null)
        {
            var period = Period;
            _clearTimer = new TimerX(RemoveNotAlive, null, 10 * 1000, period * 1000)
            {
                Async = true,
                //CanExecute = () => _cache.Any(),
            };
        }
    }

    #endregion 方法

    #region 基本操作

    /// <summary>清空所有缓存项</summary>
    public override void Clear()
    {
        _cache.Clear();
        _count = 0;
    }

    /// <summary>是否包含缓存项</summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public override Boolean ContainsKey(String key) => _cache.TryGetValue(key, out var item) && item != null && !item.Expired;

    /// <summary>获取缓存项，不存在时返回默认值</summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    [return: MaybeNull]
    public override T Get<T>(String key)
    {
        if (!_cache.TryGetValue(key, out var item) || item == null || item.Expired) return default;

        var rs = item.Visit();
        if (rs == null) return default;

        return rs.ChangeType<T>();
    }

    /// <summary>获取缓存项有效期，不存在时返回Zero</summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public override TimeSpan GetExpire(String key)
    {
        if (!_cache.TryGetValue(key, out var item) || item == null) return TimeSpan.Zero;

        return TimeSpan.FromMilliseconds(item.ExpiredTime - Runtime.TickCount64);
    }

    /// <summary>批量移除缓存项</summary>
    /// <param name="keys">键集合</param>
    /// <returns>实际移除个数</returns>
    public override Int32 Remove(params String[] keys)
    {
        var count = 0;
        foreach (var k in keys)
        {
            if (_cache.TryRemove(k, out _))
            {
                count++;

                Interlocked.Decrement(ref _count);
            }
        }
        return count;
    }

    /// <summary>添加缓存项，已存在时更新</summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="expire">过期时间，秒</param>
    /// <returns></returns>
    public override Boolean Set<T>(String key, T value, Int32 expire = -1)
    {
        if (expire < 0) expire = Expire;

        //_cache.AddOrUpdate(key,
        //    k => new CacheItem(value, expire),
        //    (k, item) =>
        //    {
        //        item.Value = value;
        //        item.ExpiredTime = DateTime.Now.AddSeconds(expire);

        //        return item;
        //    });

        // 不用AddOrUpdate，避免匿名委托带来的GC损耗
        CacheItem? ci = null;
        do
        {
            if (_cache.TryGetValue(key, out var item))
            {
                item.Set(value, expire);
                return true;
            }

            ci ??= new CacheItem(value, expire);
        } while (!_cache.TryAdd(key, ci));

        Interlocked.Increment(ref _count);

        return true;
    }

    /// <summary>设置缓存项有效期</summary>
    /// <param name="key">键</param>
    /// <param name="expire">过期时间</param>
    /// <returns>设置是否成功</returns>
    public override Boolean SetExpire(String key, TimeSpan expire)
    {
        if (!_cache.TryGetValue(key, out var item) || item == null) return false;

        item.ExpiredTime = Runtime.TickCount64 + (Int64)expire.TotalMilliseconds;

        return true;
    }

    #endregion 基本操作

    #region 高级操作

    /// <summary>添加，已存在时不更新，常用于锁争夺</summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="expire">过期时间，秒</param>
    /// <returns></returns>
    public override Boolean Add<T>(String key, T value, Int32 expire = -1)
    {
        if (expire < 0) expire = Expire;

        CacheItem? ci = null;
        do
        {
            if (_cache.TryGetValue(key, out _)) return false;

            ci ??= new CacheItem(value, expire);
        } while (!_cache.TryAdd(key, ci));

        Interlocked.Increment(ref _count);

        return true;
    }

    /// <summary>递减，原子操作</summary>
    /// <param name="key">键</param>
    /// <param name="value">变化量</param>
    /// <returns></returns>
    public override Int64 Decrement(String key, Int64 value)
    {
        var item = GetOrAddItem(key, k => 0L);
        return item.Dec(value);
    }

    /// <summary>递减，原子操作</summary>
    /// <param name="key">键</param>
    /// <param name="value">变化量</param>
    /// <returns></returns>
    public override Double Decrement(String key, Double value)
    {
        var item = GetOrAddItem(key, k => 0d);
        return item.Dec(value);
    }

    /// <summary>获取 或 添加 缓存数据，在数据不存在时执行委托请求数据</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间<seealso cref="Cache.Expire"/></param>
    /// <returns></returns>
    [return: MaybeNull]
    public override T GetOrAdd<T>(String key, Func<String, T> callback, Int32 expire = -1)
    {
        if (expire < 0) expire = Expire;

        CacheItem? ci = null;
        do
        {
            if (_cache.TryGetValue(key, out var item)) return (T?)item.Visit();

            ci ??= new CacheItem(callback(key), expire);
        } while (!_cache.TryAdd(key, ci));

        Interlocked.Increment(ref _count);

        return (T?)ci.Visit();
    }

    /// <summary>累加，原子操作</summary>
    /// <param name="key">键</param>
    /// <param name="value">变化量</param>
    /// <returns></returns>
    public override Int64 Increment(String key, Int64 value)
    {
        var item = GetOrAddItem(key, k => 0L);
        return item.Inc(value);
    }

    /// <summary>累加，原子操作</summary>
    /// <param name="key">键</param>
    /// <param name="value">变化量</param>
    /// <returns></returns>
    public override Double Increment(String key, Double value)
    {
        var item = GetOrAddItem(key, k => 0d);
        return item.Inc(value);
    }

    /// <summary>设置新值并获取旧值，原子操作</summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    [return: MaybeNull]
    public override T Replace<T>(String key, T value)
    {
        var expire = Expire;

        CacheItem? ci = null;
        do
        {
            if (_cache.TryGetValue(key, out var item))
            {
                var rs = item.Value;
                // 如果已经过期，不要返回旧值
                if (item.Expired) rs = default(T);
                item.Set(value, expire);
                return (T?)rs;
            }

            ci ??= new CacheItem(value, expire);
        } while (!_cache.TryAdd(key, ci));

        Interlocked.Increment(ref _count);

        return default;
    }

    /// <summary>尝试获取指定键，返回是否包含值。有可能缓存项刚好是默认值，或者只是反序列化失败</summary>
    /// <remarks>
    /// 在 MemoryCache 中，如果某个key过期，在清理之前仍然可以通过TryGet访问，并且更新访问时间，避免被清理。
    /// </remarks>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值。即使有值也不一定能够返回，可能缓存项刚好是默认值，或者只是反序列化失败</param>
    /// <returns>返回是否包含值，即使反序列化失败</returns>
    public override Boolean TryGetValue<T>(String key, [MaybeNullWhen(false)] out T value)
    {
        value = default;

        // 没有值，直接结束
        if (!_cache.TryGetValue(key, out var item) || item == null) return false;

        // 得到已有值
        value = item.Visit().ChangeType<T>();

        // 是否未过期的有效值
        return !item.Expired;
    }

    #endregion 高级操作

    #region 集合操作

    /// <summary>获取哈希</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public override IDictionary<String, T> GetDictionary<T>(String key)
    {
        var item = GetOrAddItem(key, k => new ConcurrentDictionary<String, T>());
        return item.Visit() as IDictionary<String, T> ??
         throw new InvalidCastException($"Unable to convert the value of [{key}] from {item.Value?.GetType()} to {typeof(IDictionary<String, T>)}");
    }

    /// <summary>获取列表</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public override IList<T> GetList<T>(String key)
    {
        var item = GetOrAddItem(key, k => new List<T>());
        return item.Visit() as IList<T> ??
         throw new InvalidCastException($"Unable to convert the value of [{key}] from {item.Value?.GetType()} to {typeof(IList<T>)}");
    }

    /// <summary>获取队列</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public override IProducerConsumer<T> GetQueue<T>(String key)
    {
        var item = GetOrAddItem(key, k => new MemoryQueue<T>());
        return item.Visit() as IProducerConsumer<T> ??
            throw new InvalidCastException($"Unable to convert the value of [{key}] from {item.Value?.GetType()} to {typeof(IProducerConsumer<T>)}");
    }

    /// <summary>获取Set</summary>
    /// <remarks>基于HashSet，非线程安全</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public override ICollection<T> GetSet<T>(String key)
    {
        var item = GetOrAddItem(key, k => new HashSet<T>());
        return item.Visit() as ICollection<T> ??
            throw new InvalidCastException($"Unable to convert the value of [{key}] from {item.Value?.GetType()} to {typeof(ICollection<T>)}");
    }

    /// <summary>获取栈</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public override IProducerConsumer<T> GetStack<T>(String key)
    {
        var item = GetOrAddItem(key, k => new MemoryQueue<T>(new ConcurrentStack<T>()));
        return item.Visit() as IProducerConsumer<T> ??
            throw new InvalidCastException($"Unable to convert the value of [{key}] from {item.Value?.GetType()} to {typeof(IProducerConsumer<T>)}");
    }

    /// <summary>获取 或 添加 缓存项</summary>
    /// <param name="key"></param>
    /// <param name="valueFactory"></param>
    /// <returns></returns>
    protected CacheItem GetOrAddItem(String key, Func<String, Object> valueFactory)
    {
        var expire = Expire;

        CacheItem? ci = null;
        do
        {
            if (_cache.TryGetValue(key, out var item)) return item;

            ci ??= new CacheItem(valueFactory(key), expire);
        } while (!_cache.TryAdd(key, ci));

        Interlocked.Increment(ref _count);

        return ci;
    }

    #endregion 集合操作

    #region 缓存项

    /// <summary>缓存项</summary>
    public class CacheItem
    {
        private Object? _Value;

        /// <summary>构造缓存项</summary>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        public CacheItem(Object? value, Int32 expire) => Set(value, expire);

        /// <summary>是否过期</summary>
        public Boolean Expired => ExpiredTime <= Runtime.TickCount64;

        /// <summary>过期时间</summary>
        public Int64 ExpiredTime { get; internal set; }

        /// <summary>数值</summary>
        public Object? Value { get => _Value; set => _Value = value; }

        /// <summary>访问时间</summary>
        public Int64 VisitTime { get; private set; }

        /// <summary>递减</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal Int64 Dec(Int64 value)
        {
            // 原子操作
            Int64 newValue;
            Object oldValue;
            do
            {
                oldValue = _Value ?? 0;
                newValue = oldValue.ToLong() - value.ToLong();
            } while (Interlocked.CompareExchange(ref _Value, newValue, oldValue) != oldValue);

            Visit();

            return newValue;
        }

        /// <summary>递减</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal Double Dec(Double value)
        {
            // 原子操作
            Double newValue;
            Object oldValue;
            do
            {
                oldValue = _Value ?? 0;
                newValue = oldValue.ToDouble() - value.ToDouble();
            } while (Interlocked.CompareExchange(ref _Value, newValue, oldValue) != oldValue);

            Visit();

            return newValue;
        }

        /// <summary>递增</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal Int64 Inc(Int64 value)
        {
            // 原子操作
            Int64 newValue;
            Object oldValue;
            do
            {
                oldValue = _Value ?? 0;
                newValue = oldValue.ToLong() + value.ToLong();
            } while (Interlocked.CompareExchange(ref _Value, newValue, oldValue) != oldValue);

            Visit();

            return newValue;
        }

        /// <summary>递增</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal Double Inc(Double value)
        {
            // 原子操作
            Double newValue;
            Object oldValue;
            do
            {
                oldValue = _Value ?? 0;
                newValue = oldValue.ToDouble() + value.ToDouble();
            } while (Interlocked.CompareExchange(ref _Value, newValue, oldValue) != oldValue);

            Visit();

            return newValue;
        }

        /// <summary>设置数值和过期时间</summary>
        /// <param name="value"></param>
        /// <param name="expire">过期时间，秒</param>
        internal void Set(Object? value, Int32 expire)
        {
            Value = value;

            var now = VisitTime = Runtime.TickCount64;
            if (expire <= 0)
                ExpiredTime = Int64.MaxValue;
            else
                ExpiredTime = now + expire * 1000L;
        }

        /// <summary>更新访问时间并返回数值</summary>
        /// <returns></returns>
        internal Object? Visit()
        {
            VisitTime = Runtime.TickCount64;
            return Value;
        }
    }

    #endregion 缓存项

    #region 清理过期缓存

    /// <summary>清理会话计时器</summary>
    private TimerX? _clearTimer;

    /// <summary>缓存过期</summary>
    /// <param name="key"></param>
    protected virtual void OnExpire(String key) => KeyExpired?.Invoke(this, new EventArgs<String>(key));

    /// <summary>移除过期的缓存项</summary>
    private void RemoveNotAlive(Object? state)
    {
        var tx = _clearTimer;
        if (tx != null /*&& tx.Period == 60_000*/) tx.Period = Period * 1000;

        var dic = _cache;
        if (_count == 0 && !dic.Any()) return;

        // 过期时间升序，用于缓存满以后删除
        var slist = new SortedList<Int64, IList<String>>();
        // 超出个数
        var flag = true;
        if (Capacity <= 0 || _count <= Capacity) flag = false;

        // 60分钟之内过期的数据，进入LRU淘汰
        var now = Runtime.TickCount64;
        var exp = now + 3600_000;
        var k = 0;

        // 这里先计算，性能很重要
        var list = new List<String>();
        foreach (var item in dic)
        {
            var ci = item.Value;
            if (ci.ExpiredTime <= now)
                list.Add(item.Key);
            else
            {
                k++;
                if (flag && ci.ExpiredTime < exp)
                {
                    if (!slist.TryGetValue(ci.VisitTime, out var ss))
                        slist.Add(ci.VisitTime, ss = new List<String>());

                    ss.Add(item.Key);
                }
            }
        }

        // 如果满了，删除前面
        if (flag && slist.Count > 0 && _count - list.Count > Capacity)
        {
            var over = _count - list.Count - Capacity;
            for (var i = 0; i < slist.Count && over > 0; i++)
            {
                var ss = slist.Values[i];
                if (ss != null && ss.Count > 0)
                {
                    foreach (var item in ss)
                    {
                        if (over <= 0) break;

                        list.Add(item);
                        over--;
                        k--;
                    }
                }
            }
        }

        foreach (var item in list)
        {
            OnExpire(item);
            _cache.Remove(item);
        }

        // 修正
        _count = k;
    }

    #endregion 清理过期缓存
}

/// <summary>生产者消费者</summary>
/// <typeparam name="T"></typeparam>
public class MemoryQueue<T> : DisposeBase, IProducerConsumer<T>
{
    private readonly IProducerConsumerCollection<T> _collection;
    private readonly SemaphoreSlim _occupiedNodes;

    /// <summary>实例化内存队列</summary>
    public MemoryQueue()
    {
        _collection = new ConcurrentQueue<T>();
        _occupiedNodes = new SemaphoreSlim(0);
    }

    /// <summary>实例化内存队列</summary>
    /// <param name="collection"></param>
    public MemoryQueue(IProducerConsumerCollection<T> collection)
    {
        _collection = collection;
        _occupiedNodes = new SemaphoreSlim(collection.Count);
    }

    /// <summary>元素个数</summary>
    public Int32 Count => _collection.Count;

    /// <summary>集合是否为空</summary>
    public Boolean IsEmpty
    {
        get
        {
            if (_collection is ConcurrentQueue<T> queue) return queue.IsEmpty;
            if (_collection is ConcurrentStack<T> stack) return stack.IsEmpty;

            throw new NotSupportedException();
        }
    }

    /// <summary>确认消费</summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public Int32 Acknowledge(params String[] keys) => 0;

    /// <summary>生产添加</summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public Int32 Add(params T[] values)
    {
        var count = 0;
        foreach (var item in values)
        {
            if (_collection.TryAdd(item))
            {
                count++;
                _occupiedNodes.Release();
            }
        }

        return count;
    }

    /// <summary>消费获取</summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public IEnumerable<T> Take(Int32 count = 1)
    {
        if (count <= 0) yield break;

        for (var i = 0; i < count; i++)
        {
            if (!_occupiedNodes.Wait(0)) break;
            if (!_collection.TryTake(out var item)) break;

            yield return item;
        }
    }

    /// <summary>消费一个</summary>
    /// <param name="timeout">超时。默认0秒，永久等待</param>
    /// <returns></returns>
    public T? TakeOne(Int32 timeout = 0)
    {
        if (!_occupiedNodes.Wait(0))
        {
            if (timeout <= 0 || !_occupiedNodes.Wait(timeout * 1000)) return default;
        }

        return _collection.TryTake(out var item) ? item : default;
    }

    /// <summary>消费获取，异步阻塞</summary>
    /// <param name="timeout">超时。单位秒，0秒表示永久等待</param>
    /// <returns></returns>
    public async Task<T?> TakeOneAsync(Int32 timeout = 0)
    {
        if (!_occupiedNodes.Wait(0))
        {
            if (timeout <= 0) return default;

            if (!await _occupiedNodes.WaitAsync(timeout * 1000)) return default;
        }

        return _collection.TryTake(out var item) ? item : default;
    }

    /// <summary>消费获取，异步阻塞</summary>
    /// <param name="timeout">超时。单位秒，0秒表示永久等待</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public async Task<T?> TakeOneAsync(Int32 timeout, CancellationToken cancellationToken)
    {
        if (!_occupiedNodes.Wait(0, cancellationToken))
        {
            if (timeout <= 0) return default;

            if (!await _occupiedNodes.WaitAsync(timeout * 1000, cancellationToken)) return default;
        }

        return _collection.TryTake(out var item) ? item : default;
    }

    /// <summary>销毁</summary>
    /// <param name="disposing"></param>
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);

        _occupiedNodes.TryDispose();
    }
}

//#nullable restore
