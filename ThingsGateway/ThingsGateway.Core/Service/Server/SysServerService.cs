using AngleSharp.Html.Parser;

using AspNetCoreRateLimit;

using Lazy.Captcha.Core;

using Magicodes.ExporterAndImporter.Excel;
using Magicodes.ExporterAndImporter.Pdf;

using Microsoft.Extensions.Hosting;

using Nest;

using NewLife.Caching;

using OnceMi.AspNetCore.OSS;

namespace ThingsGateway.Core;

/// <summary>
/// 系统服务器监控服务
/// </summary>
[ApiDescriptionSettings(Order = 185)]
public class SysServerService : IDynamicApiController, ITransient
{
    public SysServerService()
    {
    }

    /// <summary>
    /// 服务器配置信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public dynamic GetServerBase()
    {
        return new
        {
            HostName = Environment.MachineName, // 主机名称
            SystemOs = RuntimeInformation.OSDescription, // 操作系统
            OsArchitecture = Environment.OSVersion.Platform.ToString() + " " + RuntimeInformation.OSArchitecture.ToString(), // 系统架构
            ProcessorCount = Environment.ProcessorCount + " 核", // CPU核心数
            SysRunTime = ComputerUtil.GetRunTime(), // 系统运行时间
            RemoteIp = ComputerUtil.GetIpFromOnline(), // 外网地址
            LocalIp = App.HttpContext?.Connection?.LocalIpAddress.ToString(), // 本地地址
            FrameworkDescription = RuntimeInformation.FrameworkDescription, // NET框架
            Environment = App.HostEnvironment.IsDevelopment() ? "Development" : "Production",
            Wwwroot = App.WebHostEnvironment.WebRootPath, // 网站根目录
            Stage = App.HostEnvironment.IsStaging() ? "Stage环境" : "非Stage环境", // 是否Stage环境
        };
    }

    /// <summary>
    /// 服务器内存信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public dynamic GetServerUsed()
    {
        var programStartTime = Process.GetCurrentProcess().StartTime;
        var totalMilliseconds = (DateTime.Now - programStartTime).TotalMilliseconds.ToString();
        var ts = totalMilliseconds.Contains('.') ? totalMilliseconds.Split('.')[0] : totalMilliseconds;
        var programRunTime = DateTimeUtil.FormatTime(ts.ParseToLong());

        var memoryMetrics = ComputerUtil.GetComputerInfo();
        return new
        {
            memoryMetrics.FreeRam, // 空闲内存
            memoryMetrics.UsedRam, // 已用内存
            memoryMetrics.TotalRam, // 总内存
            memoryMetrics.RamRate, // 内存使用率
            memoryMetrics.CpuRate, // Cpu使用率
            StartTime = programStartTime.ToString("yyyy-MM-dd HH:mm:ss"), // 服务启动时间
            RunTime = programRunTime, // 服务运行时间
        };
    }

    /// <summary>
    /// 服务器磁盘信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public dynamic GetServerDisk()
    {
        return ComputerUtil.GetDiskInfos();
    }

    /// <summary>
    /// 框架主要程序集
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public dynamic GetAssembly()
    {
        var furionAssembly = typeof(App).Assembly.GetName();
        var sqlSugarAssembly = typeof(ISqlSugarClient).Assembly.GetName();
        var yitIdAssembly = typeof(Yitter.IdGenerator.YitIdHelper).Assembly.GetName();
        var redisAssembly = typeof(Redis).Assembly.GetName();
        var jsonAssembly = typeof(NewtonsoftJsonMvcCoreBuilderExtensions).Assembly.GetName();
        var excelAssembly = typeof(IExcelImporter).Assembly.GetName();
        var pdfAssembly = typeof(IPdfExporter).Assembly.GetName();
        var captchaAssembly = typeof(ICaptcha).Assembly.GetName();
        var wechatApiAssembly = typeof(WechatApiClient).Assembly.GetName();
        var ossAssembly = typeof(IOSSServiceFactory).Assembly.GetName();
        var parserAssembly = typeof(Parser).Assembly.GetName();
        var nestAssembly = typeof(IElasticClient).Assembly.GetName();
        var limitAssembly = typeof(IpRateLimitMiddleware).Assembly.GetName();
        var htmlParserAssembly = typeof(HtmlParser).Assembly.GetName();

        return new[]
        {
            new { furionAssembly.Name, furionAssembly.Version },
            new { sqlSugarAssembly.Name, sqlSugarAssembly.Version },
            new { yitIdAssembly.Name, yitIdAssembly.Version },
            new { redisAssembly.Name, redisAssembly.Version },
            new { jsonAssembly.Name, jsonAssembly.Version },
            new { excelAssembly.Name, excelAssembly.Version },
            new { pdfAssembly.Name, pdfAssembly.Version },
            new { captchaAssembly.Name, captchaAssembly.Version },
            new { wechatApiAssembly.Name, wechatApiAssembly.Version },
            new { ossAssembly.Name, ossAssembly.Version },
            new { parserAssembly.Name, parserAssembly.Version },
            new { nestAssembly.Name, nestAssembly.Version },
            new { limitAssembly.Name, limitAssembly.Version },
            new { htmlParserAssembly.Name, htmlParserAssembly.Version },
        };
    }
}