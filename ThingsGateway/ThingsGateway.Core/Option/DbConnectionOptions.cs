namespace ThingsGateway.Core;

/// <summary>
/// 数据库配置选项
/// </summary>
public sealed class DbConnectionOptions : IConfigurableOptions
{
    /// <summary>
    /// 数据库集合
    /// </summary>
    public List<DbConnectionConfig> ConnectionConfigs { get; set; }

    public void PostConfigure(DbConnectionOptions options, IConfiguration configuration)
    {
        foreach (var dbConfig in options.ConnectionConfigs)
        {
            if (string.IsNullOrWhiteSpace(dbConfig.ConfigId))
                dbConfig.ConfigId = SqlSugarConst.ConfigId;
        }
    }
}

public sealed class DbConnectionConfig : ConnectionConfig
{
    /// <summary>
    /// 启用库表初始化
    /// </summary>
    public bool EnableInitDb { get; set; }

    /// <summary>
    /// 启用库表差异日志
    /// </summary>
    public bool EnableDiffLog { get; set; }

    /// <summary>
    /// 启用驼峰转下划线
    /// </summary>
    public bool EnableUnderLine { get; set; }

    /// <summary>
    /// 启用sql控制台输出
    /// </summary>
    public bool EnableConsoleOut { get; set; }

}