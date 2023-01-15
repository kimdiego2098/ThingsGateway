namespace ThingsGateway.Core;

/// <summary>
/// 缓存类型枚举
/// </summary>
public enum CacheTypeEnum
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    [Description("内存缓存")]
    Memory,

    /// <summary>
    /// Redis缓存
    /// </summary>
    [Description("Redis缓存")]
    Redis
}