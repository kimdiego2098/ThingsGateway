using Microsoft.Extensions.Hosting;
namespace ThingsGateway.Application.Core;

/// <summary>
/// 服务扩展
/// </summary>
public static class SevicesExtension
{
    /// <summary>
    /// 添加windows服务支持
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureWindowsService(this IHostBuilder hostBuilder)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        hostBuilder.UseContentRoot(AppContext.BaseDirectory);
        System.IO.Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        hostBuilder.UseWindowsService();

        return hostBuilder;
    }
    /// <summary>
    /// 添加linux服务支持
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureLinuxService(this IHostBuilder hostBuilder)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        hostBuilder.UseContentRoot(AppContext.BaseDirectory);
        System.IO.Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        hostBuilder.UseSystemd();

        return hostBuilder;
    }


    /// <summary>
    /// 获取后台服务
    /// </summary>
    public static T GetBackgroundService<T>(this IServiceProvider serviceProvider) where T : class, IHostedService
    {
        var hostedService = serviceProvider.GetServices<IHostedService>().FirstOrDefault(it => it is T) as T;
        return hostedService;
    }

}