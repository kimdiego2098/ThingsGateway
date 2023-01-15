namespace ThingsGateway.Core;

/// <summary>
/// 文化程度枚举
/// </summary>
public enum CultureLevelEnum
{
    /// <summary>
    /// 小学
    /// </summary>
    [Description("小学")]
    Level1 = 0,

    /// <summary>
    /// 初中
    /// </summary>
    [Description("初中")]
    Level2 = 1,

    /// <summary>
    /// 高中
    /// </summary>
    [Description("高中")]
    Level3 = 2,

    /// <summary>
    /// 中专
    /// </summary>
    [Description("中专")]
    Level4 = 3,

    /// <summary>
    /// 大专
    /// </summary>
    [Description("大专")]
    Level5 = 4,

    /// <summary>
    /// 本科
    /// </summary>
    [Description("本科")]
    Level6 = 5,

    /// <summary>
    /// 硕士研究生
    /// </summary>
    [Description("硕士研究生")]
    Level7 = 6,

    /// <summary>
    /// 博士研究生
    /// </summary>
    [Description("博士研究生")]
    Level8 = 7,
}