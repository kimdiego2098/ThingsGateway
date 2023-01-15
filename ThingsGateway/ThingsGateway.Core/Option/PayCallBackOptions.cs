namespace ThingsGateway.Core;

/// <summary>
/// 支付回调配置选项
/// </summary>
public sealed class PayCallBackOptions : IConfigurableOptions
{
    /// <summary>
    /// 微信支付回调
    /// </summary>
    public string WechatPayUrl { get; set; }

    /// <summary>
    /// 微信退款回调
    /// </summary>
    public string WechatRefundUrl { get; set; }

    /// <summary>
    /// 支付宝支付回调
    /// </summary>
    public string AlipayUrl { get; set; }

    /// <summary>
    /// 支付宝退款回调
    /// </summary>
    public string AlipayRefundUrl { get; set; }
}