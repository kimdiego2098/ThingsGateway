namespace ThingsGateway.Core;

/// <summary>
/// 微信相关配置选项
/// </summary>
public sealed class WechatOptions : IConfigurableOptions
{
    //公众号
    public string WechatAppId { get; set; }

    public string WechatAppSecret { get; set; }

    public string EncodingAESKey { get; set; }

    public string Token { get; set; }

    //小程序
    public string WxOpenAppId { get; set; }

    public string WxOpenAppSecret { get; set; }

    public string WxOpenToken { get; set; }

    public string WxOpenEncodingAESKey { get; set; }
}