using Furion;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using ThingsGateway.Application.Core;

namespace ThingsGateway.Application;

[AppStartup(10)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {

        ThreadPool.SetMinThreads(10, 10);
        //ThreadPool.SetMaxThreads(100, 100);

        // logo显示
        services.AddLogoDisplay();
        if (App.GetConfig<bool>("Logging:Database:Enabled")) // 日志写入数据库
        {
            services.AddDatabaseLogging<GatewayDatabaseLoggingWriter>();
        }
        //添加mqtt服务
        services.AddConfigurableOptions<MqttServerSettings>();
        services.AddSingleton<MqttController>();
        services.AddMqttService();

        //添加采集/上传后台服务
        services.AddHostedService<DeviceCollectService>();
        services.AddHostedService<AlarmHostService>();
        services.AddHostedService<HisHostService>();
        services.AddHostedService<UploadService>();


    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MqttController mqttController)
    {
        app.UseMqttServer(mqttController);
    }


}