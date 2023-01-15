using Microsoft.Extensions.Logging;

using MQTTnet.Protocol;
using MQTTnet.Server;

namespace ThingsGateway.Application.Core;
public sealed class MqttController
{
    private ILogger<MqttController> _logger;
    private ISqlSugarClient _sysConfigRep;
    public MqttController(ILogger<MqttController> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;

        var scope = scopeFactory.CreateScope();
        var services = scope.ServiceProvider;

        _sysConfigRep = services.GetService<SqlSugarRepository<SysConfig>>().Context;
    }

    public Task OnClientConnected(ClientConnectedEventArgs eventArgs)
    {
        if (eventArgs.ClientId.ToUpper() == "THINGSGATEWAYVUE3")
            return Task.CompletedTask;
        _logger?.LogInformation($"Mqtt客户端 '{eventArgs.ClientId}' 连接成功.");
        return Task.CompletedTask;
    }


    public async Task ValidateConnection(ValidatingConnectionEventArgs e)
    {
        var password = await GetConfig<string>(CommonConst.MqttPassword);
        var username = await GetConfig<string>(CommonConst.MqttUserName);
        if (e.ClientId.ToUpper() == "THINGSGATEWAYVUE3")
        {
            if (e.UserName == username && e.Password == password)
                return;//前端信息不输出日志
        }

        if (e.UserName != username)
        {
            e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        }

        if (e.Password != password)
        {
            e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        }
        _logger?.LogInformation($"Mqtt客户端 '{e.ClientId}' 连接，结果：{e.ReasonCode}");

        async Task<T> GetConfig<T>(string code)
        {
            var config = await _sysConfigRep.Queryable<SysConfig>().FirstAsync(u => u.Code == code);
            var value = config != null ? config.Value : default;
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
