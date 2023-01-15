namespace ThingsGateway.Core;

/// <summary>
/// 缓存配置选项
/// </summary>
public sealed class CacheOptions : IConfigurableOptions
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    public string CacheType { get; set; }

    /// <summary>
    /// Redis连接字符串
    /// </summary>
    public string RedisConnectionString { get; set; }
}