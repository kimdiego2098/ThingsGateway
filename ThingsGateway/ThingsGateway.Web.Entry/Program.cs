using Furion.DependencyInjection;
using Furion.DynamicApiController;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Reflection;
using System.Text;

using ThingsGateway.Application.Core;
namespace ThingsGateway.Web.Entry;
public class Program
{

    public static async Task Main(string[] args)
    {
        //var builder = WebApplication.CreateBuilder(args).Inject();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        System.IO.Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(Program).Assembly.FullName,
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            WebRootPath = "wwwroot",
            Args = args
        });
        builder.Host.ConfigureWindowsService();
        builder.Host.ConfigureLinuxService();
        builder.Inject();

        builder.WebHost.UseKestrel(it =>
        {
            it.ConfigureMqttServer();
            it.ConfigureHttpServer();
        }).UseIISIntegration();

        var app = builder.Build();
        await app.StartAsync();

        await Preheat(app);
        await app.WaitForShutdownAsync();
    }

    /// <summary>
    /// 这里对微软Api预热，模拟客户端执行一次
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    private static async Task Preheat(WebApplication app)
    {
        var warm = string.Format(@"{0}/ok", app.Urls.Where(it => !it.Contains("https")).Last());
        Uri.TryCreate(warm, 0, out Uri uri);
        string serverUri = uri.Scheme + "://localhost:" + uri.Port + uri.LocalPath;
        await new HttpClient().GetAsync(serverUri);
    }
}
/// <summary>
/// 测试服务
/// </summary>
[AllowAnonymous]
[ApiDescriptionSettings(false)]
public class OkService : IDynamicApiController, ITransient
{
    public OkService()
    {
    }
    [HttpGet]
    public async Task Ok()
    {
        await Task.CompletedTask;
    }

}