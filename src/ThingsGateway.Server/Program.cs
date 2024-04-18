﻿
//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://diego2098.gitee.io/thingsgateway-docs/
//  QQ群：605534569
//------------------------------------------------------------------------------



using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

using ThingsGateway.Admin.Application;

namespace ThingsGateway.Server;

public class Program
{
    public static void Main(string[] args)
    {
        //当前工作目录设为程序集的基目录
        System.IO.Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        // 增加中文编码支持
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        #region 控制台输出Logo

        Console.Write(Environment.NewLine);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("████████╗    ██╗  ██╗    ██╗    ███╗   ██╗     ██████╗     ███████╗     ██████╗      █████╗     ████████╗    ███████╗    ██╗    ██╗     █████╗     ██╗   ██╗\r\n╚══██╔══╝    ██║  ██║    ██║    ████╗  ██║    ██╔════╝     ██╔════╝    ██╔════╝     ██╔══██╗    ╚══██╔══╝    ██╔════╝    ██║    ██║    ██╔══██╗    ╚██╗ ██╔╝\r\n   ██║       ███████║    ██║    ██╔██╗ ██║    ██║  ███╗    ███████╗    ██║  ███╗    ███████║       ██║       █████╗      ██║ █╗ ██║    ███████║     ╚████╔╝ \r\n   ██║       ██╔══██║    ██║    ██║╚██╗██║    ██║   ██║    ╚════██║    ██║   ██║    ██╔══██║       ██║       ██╔══╝      ██║███╗██║    ██╔══██║      ╚██╔╝  \r\n   ██║       ██║  ██║    ██║    ██║ ╚████║    ╚██████╔╝    ███████║    ╚██████╔╝    ██║  ██║       ██║       ███████╗    ╚███╔███╔╝    ██║  ██║       ██║   \r\n   ╚═╝       ╚═╝  ╚═╝    ╚═╝    ╚═╝  ╚═══╝     ╚═════╝     ╚══════╝     ╚═════╝     ╚═╝  ╚═╝       ╚═╝       ╚══════╝     ╚══╝╚══╝     ╚═╝  ╚═╝       ╚═╝   \r\n                                                                                                                                                            ");
        Console.ResetColor();

        #endregion 控制台输出Logo

        #region config

        var builder = WebApplication.CreateBuilder(args);
        // 增加中文编码支持网页源码显示汉字
        builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

        //并发启动/停止host
        builder.Services.Configure<HostOptions>(options =>
        {
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
        });

        if (!builder.Environment.IsDevelopment())
        {
            builder.Services.AddResponseCompression(
                opts =>
                {
                    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
                });
        }

        builder.Services.AddCors();

        builder.WebHost.UseWebRoot("wwwroot");
        builder.WebHost.UseStaticWebAssets();

        builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();//用户ID提供器
        builder.Services.AddSignalR();//注册SignalR

        builder.Services.AddControllers();
        // 添加全局数据验证

        // 关闭原生 ModelStateInvalidFilter 验证
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressMapClientErrors = false;
            options.SuppressModelStateInvalidFilter = true;
        });
        builder.Services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<ResultFilter>();
            // 关闭空引用对象验证
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(a => a.ConfigureSecurities());

        builder.ConfigureServices();

        builder.Services
            .AddRazorComponents(options =>
        {
            options.TemporaryRedirectionUrlValidityDuration = TimeSpan.FromMinutes(10);
        })
            .AddInteractiveServerComponents(options =>
            {
                options.RootComponents.MaxJSRootComponents = 500;
                options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(2);
                options.MaxBufferedUnacknowledgedRenderBatches = 20;
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(10);
            })
            .AddHubOptions(options =>
            {
                //单个传入集线器消息的最大大小。默认 32 KB
                options.MaximumReceiveMessageSize = null;
                //可为客户端上载流缓冲的最大项数。 如果达到此限制，则会阻止处理调用，直到服务器处理流项。
                options.StreamBufferCapacity = 30;
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(1);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.HandshakeTimeout = TimeSpan.FromSeconds(30);
            });

        //Nginx代理的话获取真实IP
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            //新增如下两行
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        #endregion config

        var app = builder.Build();

        app.UseServices();

        #region build

        app.UseBootstrapBlazor();

        // 启用转发中间件
        app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });

        // 启用本地化
        var option = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
        if (option != null)
        {
            app.UseRequestLocalization(option.Value);
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseResponseCompression();
            app.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = ctx => ctx.ProcessCache(app.Configuration) });
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = (stf) =>
            {
                stf.ProcessCache(app.Configuration);
                stf.Context.Response.Headers.AccessControlAllowOrigin = "*";
                stf.Context.Response.Headers.AccessControlAllowHeaders = "*";
            }
        });
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".properties"] = "application/octet-stream";
        provider.Mappings[".moc"] = "application/x-msdownload";
        provider.Mappings[".moc3"] = "application/x-msdownload";
        provider.Mappings[".mtn"] = "application/x-msdownload";

        app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });

        app.UseStaticFiles();
        var cors = app.Configuration["AllowOrigins"]?.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (cors?.Length > 0)
        {
            app.UseCors(builder => builder.WithOrigins()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.MapDefaultControllerRoute();

        app.MapRazorComponents<BlazorApp>()
            .AddAdditionalAssemblies(App.RazorAssemblies.ToArray())
            .AddInteractiveServerRenderMode();

        app.MapHub<SysHub>(HubConst.SysHubUrl);

        app.Run();

        #endregion build
    }
}