using NewLife.Caching;

namespace ThingsGateway.Core;

public static class CacheSetup
{
    /// <summary>
    /// 缓存注册（新生命Redis组件）
    /// </summary>
    /// <param name="services"></param>
    public static void AddCache(this IServiceCollection services)
    {
        services.AddSingleton(options =>
        {
            var cacheOptions = App.GetOptions<CacheOptions>();
            if (cacheOptions.CacheType == CacheTypeEnum.Redis.ToString())
            {
                var redis = new FullRedis();
                redis.Init(cacheOptions.RedisConnectionString);
                return redis;
            }
            else
            {
                return Cache.Default;
            }
        });
    }
}