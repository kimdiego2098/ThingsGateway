using Elasticsearch.Net;

using Nest;

namespace ThingsGateway.Core;

/// <summary>
/// ES服务注册，默认不开启
/// </summary>
public static class ElasticSearchSetup
{
    public static void AddElasticSearch(this IServiceCollection services)
    {
        var enabled = App.GetConfig<bool>("Logging:ElasticSearch:Enabled");
        if (!enabled) return;

        var serverUris = App.GetConfig<List<string>>("Logging:ElasticSearch:ServerUris");
        var defaultIndex = App.GetConfig<string>("Logging:ElasticSearch:DefaultIndex");

        var uris = serverUris.Select(u => new Uri(u));
        var connectionPool = new SniffingConnectionPool(uris);
        var settings = new ConnectionSettings(connectionPool).DefaultIndex(defaultIndex);
        var client = new ElasticClient(settings);
        client.Indices.Create(defaultIndex, i => i.Map<SysLogOp>(m => m.AutoMap()));

        services.AddSingleton(client); // 单例注册
    }
}