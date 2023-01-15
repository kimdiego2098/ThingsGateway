namespace ThingsGateway.Core;

/// <summary>
/// 登录类型枚举
/// </summary>
public enum LoginTypeEnum
{
    /// <summary>
    /// 登录
    /// </summary>
    [Description("登录")]
    Login = 1,

    /// <summary>
    /// 退出
    /// </summary>
    [Description("退出")]
    Logout = 2,

    /// <summary>
    /// 注册
    /// </summary>
    [Description("注册")]
    Register = 3,

    /// <summary>
    /// 改密
    /// </summary>
    [Description("改密")]
    ChangePassword = 4,

    /// <summary>
    /// 三方授权登陆
    /// </summary>
    [Description("授权登陆")]
    AuthorizedLogin = 5
}