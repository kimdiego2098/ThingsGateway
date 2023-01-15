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
using System
.Threading.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// 具有远程键值存贮的操作端。
    /// </summary>
    public abstract class RedisClient : ICache<string, byte[]>
    {
        /// <summary>
        /// 序列化转换器。
        /// </summary>
        public BytesConverter Converter { get; set; }

        /// <summary>
        /// 超时设定。默认30000ms
        /// </summary>
        public int Timeout { get; set; } = 30 * 1000;

        /// <summary>
        /// <inheritdoc cref="ICache{TKey, TValue}.AddCache(ICacheEntry{TKey, TValue})"/>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public bool Add<TValue>(string key, TValue value, int duration = 60000)
        {
            var cache = new CacheEntry<string, byte[]>(key)
            {
                Duration = TimeSpan.FromSeconds(duration)
            };
            if (!(value is byte[]))
            {
                cache.Value = Converter.ConvertTo(value);
            }
            return AddCache(cache);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public bool AddCache(ICacheEntry<string, byte[]> entity)
        {
            if (ContainsCache(entity.Key))
            {
                return false;
            }
            else
            {
                return SetCache(entity);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public Task<bool> AddCacheAsync(ICacheEntry<string, byte[]> entity)
        {
            return EasyTask.Run(() =>
            {
                return AddCache(entity);
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public abstract void ClearCache();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public Task ClearCacheAsync()
        {
            return EasyTask.Run(() =>
            {
                ClearCache();
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public abstract bool ContainsCache(string key);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public Task<bool> ContainsCacheAsync(string key)
        {
            return EasyTask.Run(() =>
            {
                return ContainsCache(key);
            });
        }

        /// <summary>
        /// 获取缓存的键值对。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public TValue Get<TValue>(string key)
        {
            if (TryGet<TValue>(key, out var cache))
            {
                return cache;
            }
            return default;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public abstract ICacheEntry<string, byte[]> GetCache(string key);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public Task<ICacheEntry<string, byte[]>> GetCacheAsync(string key)
        {
            return EasyTask.Run(() =>
            {
                return GetCache(key);
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public abstract bool RemoveCache(string key);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public Task<bool> RemoveCacheAsync(string key)
        {
            return EasyTask.Run(() =>
            {
                return RemoveCache(key);
            });
        }

        /// <summary>
        /// <inheritdoc cref="ICache{TKey, TValue}.SetCache(ICacheEntry{TKey, TValue})"/>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public bool Set<TValue>(string key, TValue value, int duration = 60000)
        {
            var cache = new CacheEntry<string, byte[]>(key)
            {
                Duration = TimeSpan.FromSeconds(duration)
            };
            if (value is byte[] bytes)
            {
                cache.Value = bytes;
            }
            else
            {
                cache.Value = Converter.ConvertTo(value);
            }
            return SetCache(cache);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public abstract bool SetCache(ICacheEntry<string, byte[]> entity);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public Task<bool> SetCacheAsync(ICacheEntry<string, byte[]> entity)
        {
            return EasyTask.Run(() =>
            {
                return SetCache(entity);
            });
        }

        /// <summary>
        /// 获取指定键的值。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="TimeoutException">操作超时</exception>
        /// <exception cref="Exception">其他异常</exception>
        public bool TryGet<TValue>(string key, out TValue value)
        {
            var cache = GetCache(key);

            if (cache != null)
            {
                if (cache.Value is null)
                {
                    value = default;
                    return true;
                }
                if (cache.Value is TValue value1)
                {
                    value = value1;
                    return true;
                }
                value = (TValue)Converter.ConvertFrom(cache.Value, typeof(TValue));
                return true;
            }
            value = default;
            return false;
        }
    }

    /// <summary>
    /// RedisClient
    /// </summary>
    internal class InternalRedisClient : RedisClient
    {
        private readonly RpcActor m_rpcActor;

        public InternalRedisClient(RpcActor rpcActor, BytesConverter converter)
        {
            m_rpcActor = rpcActor;
            Converter = converter;
        }

        public override void ClearCache()
        {
            var package = new RedisRequestWaitPackage
            {
                packageType = RedisPackageType.Clear
            };

            var waitData = m_rpcActor.WaitHandlePool.GetWaitData(package);
            try
            {
                using (ByteBlock byteBlock = new ByteBlock())
                {
                    package.Package(byteBlock);
                    m_rpcActor.Send(TouchRpcUtility.P_600_Redis_Request, byteBlock.Buffer, 0, byteBlock.Len);
                }
                switch (waitData.Wait(Timeout))
                {
                    case WaitDataStatus.SetRunning:
                        {
                            if (waitData.WaitResult.Status == 1)
                            {
                                return;
                            }
                            else
                            {
                                throw new Exception(waitData.WaitResult.Message);
                            }
                        }
                    case WaitDataStatus.Overtime: throw new TimeoutException(TouchSocketStatus.Overtime.GetDescription());
                    case WaitDataStatus.Canceled:
                    case WaitDataStatus.Default:
                    case WaitDataStatus.Disposed:
                    default:
                        throw new Exception(TouchSocketStatus.UnknownError.GetDescription());
                }
            }
            finally
            {
                m_rpcActor.WaitHandlePool.Destroy(waitData);
            }
        }

        public override bool ContainsCache(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"“{nameof(key)}”不能为 null 或空。", nameof(key));
            }

            var package = new RedisRequestWaitPackage
            {
                key = key,
                packageType = RedisPackageType.Contains
            };

            var waitData = m_rpcActor.WaitHandlePool.GetWaitData(package);
            try
            {
                using (ByteBlock byteBlock = new ByteBlock())
                {
                    package.Package(byteBlock);
                    m_rpcActor.Send(TouchRpcUtility.P_600_Redis_Request, byteBlock.Buffer, 0, byteBlock.Len);
                }
                switch (waitData.Wait(Timeout))
                {
                    case WaitDataStatus.SetRunning:
                        {
                            if (waitData.WaitResult.Status == 1)
                            {
                                return true;
                            }
                            else if (waitData.WaitResult.Status == byte.MaxValue)
                            {
                                return false;
                            }
                            else
                            {
                                throw new Exception(waitData.WaitResult.Message);
                            }
                        }
                    case WaitDataStatus.Overtime: throw new TimeoutException(TouchSocketStatus.Overtime.GetDescription());
                    case WaitDataStatus.Canceled:
                    case WaitDataStatus.Default:
                    case WaitDataStatus.Disposed:
                    default:
                        throw new Exception(TouchSocketStatus.UnknownError.GetDescription());
                }
            }
            finally
            {
                m_rpcActor.WaitHandlePool.Destroy(waitData);
            }
        }

        public override ICacheEntry<string, byte[]> GetCache(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"“{nameof(key)}”不能为 null 或空。", nameof(key));
            }

            RedisRequestWaitPackage package = new RedisRequestWaitPackage()
            {
                key = key,
                packageType = RedisPackageType.Get
            };

            var waitData = m_rpcActor.WaitHandlePool.GetWaitData(package);
            try
            {
                using (ByteBlock byteBlock = new ByteBlock((package.value == null ? 0 : package.value.Length) + 1024))
                {
                    package.Package(byteBlock);
                    m_rpcActor.Send(TouchRpcUtility.P_600_Redis_Request, byteBlock.Buffer, 0, byteBlock.Len);
                }
                switch (waitData.Wait(Timeout))
                {
                    case WaitDataStatus.SetRunning:
                        {
                            RedisResponseWaitPackage responsePackage = (RedisResponseWaitPackage)waitData.WaitResult;
                            if (responsePackage.Status == 1)
                            {
                                return new CacheEntry<string, byte[]>(key)
                                {
                                    Value = responsePackage.value
                                };
                            }
                            else if (responsePackage.Status == byte.MaxValue)
                            {
                                return new CacheEntry<string, byte[]>(key);
                            }
                            else
                            {
                                return default;
                            }
                        }
                    case WaitDataStatus.Overtime: throw new TimeoutException(TouchSocketStatus.Overtime.GetDescription());
                    case WaitDataStatus.Canceled:
                    case WaitDataStatus.Default:
                    case WaitDataStatus.Disposed:
                    default:
                        throw new TimeoutException(TouchSocketStatus.UnknownError.GetDescription());
                }
            }
            finally
            {
                m_rpcActor.WaitHandlePool.Destroy(waitData);
            }
        }

        public override bool RemoveCache(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"“{nameof(key)}”不能为 null 或空。", nameof(key));
            }

            var package = new RedisRequestWaitPackage
            {
                key = key,
                packageType = RedisPackageType.Remove
            };

            var waitData = m_rpcActor.WaitHandlePool.GetWaitData(package);
            try
            {
                using (ByteBlock byteBlock = new ByteBlock((package.value == null ? 0 : package.value.Length) + 1024))
                {
                    package.Package(byteBlock);
                    m_rpcActor.Send(TouchRpcUtility.P_600_Redis_Request, byteBlock.Buffer, 0, byteBlock.Len);
                }
                switch (waitData.Wait(Timeout))
                {
                    case WaitDataStatus.SetRunning:
                        {
                            if (waitData.WaitResult.Status == 1)
                            {
                                return true;
                            }
                            else if (waitData.WaitResult.Status == byte.MaxValue)
                            {
                                return false;
                            }
                            else
                            {
                                throw new Exception(waitData.WaitResult.Message);
                            }
                        }
                    case WaitDataStatus.Overtime: throw new TimeoutException(Resources.TouchSocketStatus.Overtime.GetDescription());
                    case WaitDataStatus.Canceled: return false;
                    case WaitDataStatus.Default:
                    case WaitDataStatus.Disposed:
                    default:
                        throw new TimeoutException(Resources.TouchSocketStatus.UnknownError.GetDescription());
                }
            }
            finally
            {
                m_rpcActor.WaitHandlePool.Destroy(waitData);
            }
        }

        public override bool SetCache(ICacheEntry<string, byte[]> cache)
        {
            if (string.IsNullOrEmpty(cache.Key))
            {
                throw new ArgumentException($"“{nameof(cache.Key)}”不能为 null 或空。", nameof(cache.Key));
            }

            if (cache is null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            var package = new RedisRequestWaitPackage
            {
                key = cache.Key,
                timeSpan = cache.Duration,
                value = cache.Value,
                packageType = RedisPackageType.Set
            };

            var waitData = m_rpcActor.WaitHandlePool.GetWaitData(package);
            try
            {
                using (ByteBlock byteBlock = new ByteBlock((package.value == null ? 0 : package.value.Length) + 1024))
                {
                    package.Package(byteBlock);
                    m_rpcActor.Send(TouchRpcUtility.P_600_Redis_Request, byteBlock.Buffer, 0, byteBlock.Len);
                }
                switch (waitData.Wait(Timeout))
                {
                    case WaitDataStatus.SetRunning:
                        {
                            if (waitData.WaitResult.Status == 1)
                            {
                                return true;
                            }
                            else if (waitData.WaitResult.Status == byte.MaxValue)
                            {
                                return false;
                            }
                            else
                            {
                                throw new Exception(waitData.WaitResult.Message);
                            }
                        }
                    case WaitDataStatus.Overtime: throw new TimeoutException(Resources.TouchSocketStatus.Overtime.GetDescription());
                    case WaitDataStatus.Canceled: return false;
                    case WaitDataStatus.Default:
                    case WaitDataStatus.Disposed:
                    default:
                        throw new TimeoutException(Resources.TouchSocketStatus.UnknownError.GetDescription());
                }
            }
            finally
            {
                m_rpcActor.WaitHandlePool.Destroy(waitData);
            }
        }
    }
}