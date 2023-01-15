using AspNetCoreRateLimit;

namespace ThingsGateway.Core;

/// <summary>
/// IP限流配置选项
/// </summary>
public sealed class IpRateLimitingOptions : IpRateLimitOptions
{
}

/// <summary>
/// IP限流策略配置选项
/// </summary>
public sealed class IpRateLimitPoliciesOptions : IpRateLimitPolicies, IConfigurableOptions
{
}

/// <summary>
/// 客户端限流配置选项
/// </summary>
public sealed class ClientRateLimitingOptions : ClientRateLimitOptions, IConfigurableOptions
{
}

/// <summary>
/// 客户端限流策略配置选项
/// </summary>
public sealed class ClientRateLimitPoliciesOptions : ClientRateLimitPolicies, IConfigurableOptions
{
}