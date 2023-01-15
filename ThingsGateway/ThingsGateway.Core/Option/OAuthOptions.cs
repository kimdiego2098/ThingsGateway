namespace ThingsGateway.Core;

/// <summary>
/// 第三方登录授权配置选项
/// </summary>
public sealed class OAuthOptions : IConfigurableOptions
{
    /// <summary>
    /// 微信配置
    /// </summary>
    public OAuthOptionItem Weixin { get; set; }
}

public class OAuthOptionItem
{
    /// <summary>
    /// ClientId
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// ClientSecret
    /// </summary>
    public string ClientSecret { get; set; }
}