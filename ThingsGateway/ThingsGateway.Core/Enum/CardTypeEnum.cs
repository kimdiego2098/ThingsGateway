namespace ThingsGateway.Core;

/// <summary>
/// 证件类型枚举
/// </summary>
public enum CardTypeEnum
{
    /// <summary>
    /// 身份证
    /// </summary>
    [Description("身份证")]
    IdCard = 0,

    /// <summary>
    /// 护照
    /// </summary>
    [Description("护照")]
    PassportCard = 1,

    /// <summary>
    /// 出生证
    /// </summary>
    [Description("出生证")]
    BirthCard = 2,

    /// <summary>
    /// 港澳台通行证
    /// </summary>
    [Description("港澳台通行证")]
    GatCard = 3,

    /// <summary>
    /// 外国人居留证
    /// </summary>
    [Description("外国人居留证")]
    ForeignCard = 4
}