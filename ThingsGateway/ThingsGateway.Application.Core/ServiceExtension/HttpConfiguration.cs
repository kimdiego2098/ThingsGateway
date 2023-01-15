using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace ThingsGateway.Application.Core
{
    public static class HttpConfiguration
    {
        public static void ConfigureHttpServer(this KestrelServerOptions option)
        {
            var config = App.GetConfig<HttpServerSettings>("HttpServer", false);
            option.ListenAnyIP(config.Port);
            if (config.HttpsEnable)
                option.ListenAnyIP(config.HttpsPort, a => a.UseHttps());
        }


        public class HttpServerSettings
        {
            public int Port { get; set; }
            public bool HttpsEnable { get; set; }
            public int HttpsPort { get; set; }
        }


    }

}
