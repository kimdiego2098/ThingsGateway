namespace ThingsGateway.Application.Core;
public enum SqlDbType
{
    SqlServer,
    Mysql,
    Sqlite,
    PostgreSQL,
    Oracle,
}
/// <summary>
/// 报警数据库配置表
/// </summary>
[SugarTable("alarm_config", "报警配置表")]
[Description("报警配置表")]
[Tenant(ApplicationConst.ConfigId)]
[SystemTable]
public class AlarmConfig : EntityBase
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库类型")]
    public SqlDbType DbType { get; set; }

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