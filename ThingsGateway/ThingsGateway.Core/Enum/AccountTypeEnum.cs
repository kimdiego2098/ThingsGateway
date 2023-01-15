namespace ThingsGateway.Core;

/// <summary>
/// 账号类型枚举
/// </summary>
public enum AccountTypeEnum
{
    /// <summary>
    /// 其他
    /// </summary>
    [Description("其他")]
    None = 0,

    /// <summary>
    /// 普通账号
    /// </summary>
    [Description("普通账号")]
    User = 1,

    /// <summary>
    /// 系统管理员
    /// </summary>
    [Description("系统管理员")]
    Admin = 4,

    /// <summary>
    /// 超级管理员
    /// </summary>
    [Description("超级管理员")]
    SuperAdmin = 999,
}