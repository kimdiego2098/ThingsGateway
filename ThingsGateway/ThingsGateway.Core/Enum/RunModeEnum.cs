namespace ThingsGateway.Core;

/// <summary>
/// 运行模式枚举
/// </summary>
public enum RunModeEnum
{
    /// <summary>
    /// 账号模式
    /// </summary>
    [Description("账号模式")]
    Account = 1,

    /// <summary>
    /// 三方授权
    /// </summary>
    [Description("三方授权")]
    OpenID = 2
}