namespace ThingsGateway.Core;

/// <summary>
/// 刷新Token配置选项
/// </summary>
public sealed class RefreshTokenOptions : IConfigurableOptions
{
    /// <summary>
    /// 令牌过期时间(分钟) 默认2天
    /// </summary>
    public int ExpiredTime { get; set; }
}