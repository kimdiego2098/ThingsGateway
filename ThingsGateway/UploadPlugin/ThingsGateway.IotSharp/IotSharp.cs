using Microsoft.Extensions.Logging;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

using Newtonsoft.Json;

using System.Collections.Concurrent;
using System.Text;

using ThingsGateway.Application.Core;
using ThingsGateway.Foundation;
using ThingsGateway.Foundation.Core;
using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.IotSharp;
/// <summary>
/// 这部分代码/主题规则等参考IotGateway：https://gitee.com/iioter/iotgateway
/// </summary>
public partial class IotSharp : UpLoadBase
{

    private IMqttClient mqttClient;

    /// <summary>
    /// 接口约定构造实现
    /// </summary>
    public IotSharp(ILogger logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }

    [DeviceProperty("连接Id", "")]
    public string ClientId { get; set; } = Guid.NewGuid().ToString();
    [DeviceProperty("Mqtt服务Ip", "")]
    public string ServerIp { get; set; }

    [DeviceProperty("Mqtt服务端口", "")]
    public ushort ServerPort { get; set; }

    [DeviceProperty("用户名", "")]
    public string UserName { get; set; }

    [DeviceProperty("密码", "")]
    public string Password { get; set; }

    [DeviceProperty("连接超时时间", "")]
    public ushort ConnectTimeOut { get; set; } = 3000;



    public override void Dispose()
    {
        mqttClient?.Dispose();
    }
    private async Task<bool> MqttClient(bool reconnect = false)
    {
        if (reconnect)
        {
            await Cilent();
            return mqttClient.IsConnected;
        }
        else
        {
            if (mqttClient?.IsConnected == true)
                return mqttClient.IsConnected;
            await Cilent();
            return mqttClient.IsConnected;
        }

        async Task Cilent()
        {
            var mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(ClientId)
                .WithCredentials(UserName, Password)//账密
                .WithTcpServer(ServerIp, ServerPort)//服务器
                .WithCleanSession(true)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(120.0))
            .Build();


            mqttClient.ConnectedAsync += mqttClient_ConnectedAsync;
            mqttClient.ApplicationMessageReceivedAsync += mqttClient_ApplicationMessageReceivedAsync;


            try
            {
                using var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(ConnectTimeOut));
                await mqttClient.ConnectAsync(mqttClientOptions, timeoutToken.Token);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, _uploadDevice.Name);
            }
        }
    }

    private async Task mqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        try
        {
            var topics = arg.ApplicationMessage.Topic.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var rpcDeviceName = topics[1];
            var rpcRequestId = topics[5];
            if (!string.IsNullOrEmpty(rpcDeviceName) &&
                !string.IsNullOrEmpty(rpcRequestId))
            {
                OperResult data = new();
                try
                {
                    var deviceCollectService = _serviceProvider.GetBackgroundService<DeviceCollectService>();
                    data = await deviceCollectService.InvokeDeviceMed(ToString(),
        JsonConvert.DeserializeObject<Dictionary<string, object>>(arg.ApplicationMessage
            .ConvertPayloadToString())
        , rpcRequestId
        );
                }
                catch (Exception ex)
                {
                    data.ResultCode = ResultCode.Error;
                    data.Message = ex.Message;
                }

                if (data?.IsSuccess == true)
                {
                    RpcResponse rpcResult = new()
                    {
                        DeviceId = rpcDeviceName,
                        ResponseId = rpcRequestId,
                        Data = JsonConvert.SerializeObject(new Dictionary<string, object>
                            {
                                { "success", data.IsSuccess }, { "description", data.Message }
                            })
                    };
                    var topic = $"devices/{rpcResult.DeviceId}/rpc/response/{rpcResult.Method}/{rpcResult.ResponseId}";
                    MqttUp(rpcResult, topic);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(
                ex, $"客户端:{arg.ClientId} 主题:{arg.ApplicationMessage.Topic},载荷:{arg.ApplicationMessage.ConvertPayloadToString()}");
        }

    }


    public override string ToString()
    {
        return string.Format("IotSharp;IP:{0};PORT:{1};CLIENTID:{2}", ServerIp, ServerPort, ClientId);
    }
    private async Task mqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        await mqttClient.SubscribeAsync("devices/+/rpc/request/+/+", MqttQualityOfServiceLevel.ExactlyOnce);
    }


    private async void MqttUp(object item, string topic)
    {
        if (topic.IsNullOrEmpty()) return;
        if (await MqttClient())
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
           .WithTopic(topic)
           .WithPayload(Encoding.UTF8.GetBytes(item.ToJsonString()))
           .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
           .WithRetainFlag(true)
           .Build();

            var result = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
            if (result.IsSuccess)
                _logger?.LogDebug(string.Format(@"上传：TOPIC:{0}" + Environment.NewLine + "Payload:{1}", applicationMessage.Topic, applicationMessage.ConvertPayloadToString()));
            else
                _logger?.LogWarning(new Exception(result.ReasonString), ToString());
        }
        else
        {
            _logger?.LogWarning(new Exception("上传失败，mqtt无法连接"), ToString());
        }
    }
    ///格林威治时间
    private static DateTime timeSpan_Greenwich = new DateTime(1970, 1, 1, 0, 0, 0);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000, stoppingToken);
        //上传客户端属性
        foreach (var item in allDeviceData.Devices)
        {
            var datas = item.DeviceProperties
                             .ToDictionary(x => x.DevicePropertyName, x => x.Value);
            MqttUp(datas, $"devices/{item.Name}/attributes");
        }

        TimerTick alarm_TimerTick = new TimerTick(1000);
        TimerTick value_TimerTick = new TimerTick(1000);
        TimerTick device_TimerTick = new TimerTick(30000);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_uploadDevice.InvokeEnable) continue;
            _uploadDevice.UploadDeviceStatus.ActiveTime = DateTime.Now;
            _uploadDevice.UploadDeviceStatus.DeviceOnLineStatus = DeviceOnLineStatusEnum.OnLine;


            if (value_TimerTick.IsTickHappen())
            {
                Dictionary<string, List<IotSharpTelemetry>> KVDatas = new();
                foreach (var item in _varQueues)
                {
                    var datas = item.Value.ToListWithDequeue(5000);
                    if (datas.Count > 0)
                    {
                        List<IotSharpTelemetry> iotSharpTelemetries = new();
                        iotSharpTelemetries.Add(new()
                        {
                            DeviceStatus = DeviceStatusEnums.Good,
                            TS = (long)(DateTime.UtcNow - timeSpan_Greenwich).TotalMilliseconds,
                            Values = datas.ToDictionary(o => o.Name, o => o.Value)
                        });
                        KVDatas.Add(item.Key, iotSharpTelemetries);
                        MqttUp(KVDatas, $"devices/{item.Key}/telemetry");

                    }

                }
            }


            await Task.Delay(1000, stoppingToken);

        }
    }

    private ConcurrentDictionary<string, IntelligentConcurrentQueue<DeviceVariable>> _varQueues = new();

    protected override void DeviceVariableValueChange(DeviceVariable variable)
    {
        if (!_uploadDevice.InvokeEnable) return;
        _varQueues.TryAdd(variable.Device.Name, new(20000));
        _varQueues[variable.Device.Name].Enqueue(variable);
    }


    protected override void DeviceStatusChnage(Device device)
    {
        if (!_uploadDevice.InvokeEnable) return;
        if (device.DeviceStatus.DeviceOnLineStatus == DeviceOnLineStatusEnum.OnLine)
        {

            MqttUp(JsonConvert.SerializeObject(new Dictionary<string, string>
                { { "device", device.Name } }), "v1/gateway/connect");

        }
        else
        {
            MqttUp(JsonConvert.SerializeObject(new Dictionary<string, string>
                { { "device", device.Name } }), "v1/gateway/disconnect");

        }


    }

}
