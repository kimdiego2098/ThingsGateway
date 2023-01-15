using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;

using MQTTnet.AspNetCore;

namespace ThingsGateway.Application.Core
{
    public static class MqttConfiguration
    {
        public static void ConfigureMqttServer(this KestrelServerOptions option)
        {
            var config = App.GetConfig<MqttServerSettings>("MqttServer", false);
            option.ListenAnyIP(config.Port, config => config.UseMqtt());
        }

        public static void UseMqttServer(this IApplicationBuilder app, MqttController mqttController)
        {
            app.UseMqttServer(mqttServer =>
            {
                mqttServer.ValidatingConnectionAsync += mqttController.ValidateConnection;
                mqttServer.ClientConnectedAsync += mqttController.OnClientConnected;
            });

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapConnectionHandler<MqttConnectionHandler>(
    "/mqtt",
    httpConnectionDispatcherOptions =>
    {
        httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
                  protocolList => protocolList.FirstOrDefault() ?? string.Empty;
    }
    );
            });
        }

        public static void AddMqttService(
            this IServiceCollection services)
        {
            var config = App.GetConfig<MqttServerSettings>("MqttServer", false);
            services.AddHostedMqttServerWithServices(options =>
            {
                options.WithDefaultEndpointPort(config.Port).WithDefaultEndpoint();
            });
            services.AddMqttConnectionHandler();
            services.AddMqttWebSocketServerAdapter();
            services.AddMqttTcpServerAdapter();
            services.AddConnections();
        }

    }
}
