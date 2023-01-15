namespace ThingsGateway.Core;

/// <summary>
/// 系统微信用户表
/// </summary>
[SugarTable(null, "系统微信用户表")]
[SystemTable]
public class SysWechatUser : EntityBase
{
    /// <summary>
    /// 系统用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "系统用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 系统用户
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public SysUser SysUser { get; set; }

    /// <summary>
    /// 平台类型
    /// </summary>
    [SugarColumn(ColumnDescription = "平台类型")]
    public PlatformTypeEnum PlatformType { get; set; } = PlatformTypeEnum.微信公众号;

    /// <summary>
    /// OpenId
    /// </summary>
    [SugarColumn(ColumnDescription = "OpenId", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string OpenId { get; set; }

    /// <summary>
    /// 缓存key
    /// </summary>
    [SugarColumn(ColumnDescription = "缓存key", Length = 256)]
    [MaxLength(256)]
    public string? SessionKey { get; set; }

    /// <summary>
    /// UnionId
    /// </summary>
    [SugarColumn(ColumnDescription = "UnionId", Length = 64)]
    [MaxLength(64)]
    public string? UnionId { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(ColumnDescription = "昵称", Length = 64)]
    [MaxLength(64)]
    public string? NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(ColumnDescription = "头像", Length = 256)]
    [MaxLength(256)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    [SugarColumn(ColumnDescription = "手机号码", Length = 16)]
    [MaxLength(16)]
    public string? Mobile { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [SugarColumn(ColumnDescription = "性别")]
    public int? Sex { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    [SugarColumn(ColumnDescription = "语言", Length = 64)]
    [MaxLength(64)]
    public string? Language { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarColumn(ColumnDescription = "城市", Length = 64)]
    [MaxLength(64)]
    public string? City { get; set; }

    /// <summary>
    /// 省
    /// </summary>
    [SugarColumn(ColumnDescription = "省", Length = 64)]
    [MaxLength(64)]
    public string? Province { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    [SugarColumn(ColumnDescription = "国家", Length = 64)]
    [MaxLength(64)]
    public string? Country { get; set; }

    /// <summary>
    /// AccessToken
    /// </summary>
    [SugarColumn(ColumnDescription = "AccessToken", ColumnDataType = "longtext,text,clob")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// RefreshToken
    /// </summary>
    [SugarColumn(ColumnDescription = "RefreshToken", ColumnDataType = "longtext,text,clob")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    [SugarColumn(ColumnDescription = "ExpiresIn")]
    public int? ExpiresIn { get; set; }
}