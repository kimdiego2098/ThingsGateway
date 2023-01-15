namespace ThingsGateway.Core;

/// <summary>
/// 微信支付配置选项
/// </summary>
public sealed class WechatPayOptions : IConfigurableOptions
{
    /// <summary>
    /// 微信公众平台AppId、开放平台AppId、小程序AppId、企业微信CorpId
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    ///商户平台的商户号
    /// </summary>
    public string MerchantId { get; set; }

    /// <summary>
    /// 商户平台的APIv3密钥
    /// </summary>
    public string MerchantV3Secret { get; set; }

    /// <summary>
    /// 商户平台的证书序列号
    /// </summary>
    public string MerchantCertificateSerialNumber { get; set; }

    /// <summary>
    /// 商户平台的API证书私钥(apiclient_key.pem文件内容)
    /// </summary>
    public string MerchantCertificatePrivateKey { get; set; }

    /// <summary>
    /// RSA公钥 仅调用"企业付款到银行卡API"时使用
    /// </summary>
    public string RsaPublicKey { get; set; }
}