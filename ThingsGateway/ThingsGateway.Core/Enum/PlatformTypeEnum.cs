namespace ThingsGateway.Core;

/// <summary>
/// 平台类型枚举
/// </summary>
public enum PlatformTypeEnum
{
    /// <summary>
    /// 微信公众号
    /// </summary>
    [Description("微信公众号")]
    微信公众号 = 1,

    /// <summary>
    /// 微信小程序
    /// </summary>
    [Description("微信小程序")]
    微信小程序 = 2,

    /// <summary>
    /// 支付宝小程序
    /// </summary>
    [Description("支付宝小程序")]
    支付宝小程序 = 3,

    /// <summary>
    /// 微信APP快捷登陆
    /// </summary>
    [Description("微信APP快捷登陆")]
    微信APP快捷登陆 = 4,

    /// <summary>
    /// QQ在APP中快捷登陆
    /// </summary>
    [Description("QQ在APP中快捷登陆")]
    QQ在APP中快捷登陆 = 5,

    /// <summary>
    /// 头条系小程序
    /// </summary>
    [Description("头条系小程序")]
    头条系小程序 = 6,
}