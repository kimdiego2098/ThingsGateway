namespace ThingsGateway.Application.Core;

/// <summary>
/// 当前工程的信息
/// </summary>
public class ApplicationInfo
{
    /// <summary>
    /// 当前启动IP地址
    /// </summary>
    public static string AppIp { get; set; }
    /// <summary>
    /// 当前Dll版本
    /// </summary>
    public static Version CoreVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version;
    /// <summary>
    /// 文档地址
    /// </summary>
    public static string Doc { get; } = @"https://www.yuque.com/books/share/17c7859d-27c7-444e-89cf-389d8dd02194";
    /// <summary>
    /// DESC密钥
    /// </summary>
    public static string DESCKey { get; } = "DiegoThingsGateway";
}

