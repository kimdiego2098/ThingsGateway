namespace ThingsGateway.Core.Service;

/// <summary>
/// 微信API客户端
/// </summary>
[ApiDescriptionSettings(false)]
public partial class WechatApiHttpClient : ISingleton
{
    public readonly WechatOptions _wechatOptions;

    public WechatApiHttpClient(IOptions<WechatOptions> wechatOptions)
    {
        _wechatOptions = wechatOptions.Value;
    }

    /// <summary>
    /// 微信公众号
    /// </summary>
    /// <returns></returns>
    public WechatApiClient CreateWechatClient()
    {
        if (string.IsNullOrEmpty(_wechatOptions.WechatAppId) || string.IsNullOrEmpty(_wechatOptions.WechatAppSecret))
            throw Oops.Oh("微信公众号配置错误");

        var wechatApiClient = new WechatApiClient(new WechatApiClientOptions()
        {
            AppId = _wechatOptions.WechatAppId,
            AppSecret = _wechatOptions.WechatAppSecret,
        });

        wechatApiClient.Configure(settings =>
        {
            settings.JsonSerializer = new FlurlNewtonsoftJsonSerializer();
        });

        return wechatApiClient;
    }

    /// <summary>
    /// 微信小程序
    /// </summary>
    /// <returns></returns>
    public WechatApiClient CreateWxOpenClient()
    {
        if (string.IsNullOrEmpty(_wechatOptions.WxOpenAppId) || string.IsNullOrEmpty(_wechatOptions.WxOpenAppSecret))
            throw Oops.Oh("微信小程序配置错误");

        var WechatApiClient = new WechatApiClient(new WechatApiClientOptions()
        {
            AppId = _wechatOptions.WxOpenAppId,
            AppSecret = _wechatOptions.WxOpenAppSecret
        });

        WechatApiClient.Configure(settings =>
        {
            settings.JsonSerializer = new FlurlNewtonsoftJsonSerializer();
        });

        return WechatApiClient;
    }
}