namespace ThingsGateway.Application.Core;
public enum HisDbType
{
    QuestDB,
}
/// <summary>
/// 历史数据数据库配置表
/// </summary>
[SugarTable("his_config", "历史数据数据库配置表")]
[Description("历史数据数据库配置表")]
[Tenant(ApplicationConst.ConfigId)]
[SystemTable]
public class HisConfig : EntityBase
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库类型")]
    public HisDbType DbType { get; set; }

    /// <summary>
    /// 链接字符串 MD5加密
    /// </summary>
    [SugarColumn(ColumnDescription = "链接字符串")]
    public string? ConnStr { get; set; }

    /// <summary>
    /// 启用
    /// </summary>
    [SugarColumn(ColumnDescription = "启用")]
    public bool Enable { get; set; }


}