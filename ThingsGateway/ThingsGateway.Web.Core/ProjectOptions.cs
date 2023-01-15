using AspNetCoreRateLimit;

using Microsoft.Extensions.DependencyInjection;


namespace ThingsGateway.Api.Core;

public static class ProjectOptions
{
    /// <summary>
    /// 注册项目配置选项
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddProjectOptions(this IServiceCollection services)
    {
        services.AddConfigurableOptions<DbConnectionOptions>();
        services.AddConfigurableOptions<RefreshTokenOptions>();
        services.AddConfigurableOptions<SnowIdOptions>();
        services.AddConfigurableOptions<CacheOptions>();
        services.AddConfigurableOptions<OSSProviderOptions>();
        services.AddConfigurableOptions<UploadOptions>();
        services.AddConfigurableOptions<WechatOptions>();
        services.AddConfigurableOptions<WechatPayOptions>();
        services.AddConfigurableOptions<PayCallBackOptions>();
        services.AddConfigurableOptions<EmailOptions>();
        services.AddConfigurableOptions<OAuthOptions>();
        //services.AddConfigurableOptions<IpRateLimitingOptions>();
        //services.AddConfigurableOptions<IpRateLimitPoliciesOptions>();
        //services.AddConfigurableOptions<ClientRateLimitingOptions>();
        //services.AddConfigurableOptions<ClientRateLimitPoliciesOptions>();
        services.Configure<IpRateLimitOptions>(App.Configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(App.Configuration.GetSection("IpRateLimitPolicies"));
        services.Configure<ClientRateLimitOptions>(App.Configuration.GetSection("ClientRateLimiting"));
        services.Configure<ClientRateLimitPolicies>(App.Configuration.GetSection("ClientRateLimitPolicies"));

        return services;
    }
}