using Microsoft.Extensions.Logging;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

using System.Text;

using ThingsGateway.Application.Core;
using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.DefaultMqttUp;
public partial class DefaultMqttUp : UpLoadBase
{

    private IMqttClient mqttClient;

    /// <summary>
    /// 接口约定构造实现
    /// </summary>
    public DefaultMqttUp(ILogger logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }

    [DeviceProperty("连接Id", "")]
    public string ClientId { get; set; }
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

    [DeviceProperty("实时报警主题", "")]
    public string RealAlarmTopic { get; set; }

    [DeviceProperty("当前报警主题", "5秒一次传输全部实时报警")]
    public string AlarmTopic { get; set; }

    [DeviceProperty("实时数据主题", "")]
    public string RealValueTopic { get; set; }
    [DeviceProperty("当前数据主题", "30秒一次传输全部变量数据")]
    public string ValueTopic { get; set; }

    [DeviceProperty("实时设备状态主题", "")]
    public string RealDeviceStatusTopic { get; set; }
    [DeviceProperty("当前设备状态主题", "5秒一次传输全部设备状态")]
    public string DeviceStatusTopic { get; set; }


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
            try
            {
                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(ConnectTimeOut)))
                {
                    await mqttClient.ConnectAsync(mqttClientOptions, timeoutToken.Token);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ToString());
            }
        }
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000, stoppingToken);
        TimerTick alarm_TimerTick = new TimerTick(5000);
        TimerTick value_TimerTick = new TimerTick(30000);
        TimerTick device_TimerTick = new TimerTick(30000);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_uploadDevice.InvokeEnable) continue;
            _uploadDevice.UploadDeviceStatus.ActiveTime = DateTime.Now;
            _uploadDevice.UploadDeviceStatus.DeviceOnLineStatus = DeviceOnLineStatusEnum.OnLine;

            if (alarm_TimerTick.IsTickHappen())
            {
                MqttUp(alarmHostService.RealAlarmDeviceVariables, AlarmTopic);
            }
            if (value_TimerTick.IsTickHappen())
            {
                MqttUp(allDeviceData.DeviceVariables, ValueTopic);
            }
            if (device_TimerTick.IsTickHappen())
            {
                MqttUp(allDeviceData.Devices, DeviceStatusTopic);
            }

            await Task.Delay(1000, stoppingToken);

        }
    }


    protected override void DeviceVariableCollectChange(DeviceVariable variable)
    {
        if (!_uploadDevice.InvokeEnable) return;

        MqttUp(variable, RealValueTopic);
    }

    protected override void AlarmChnage(DeviceVariable alarm)
    {
        if (!_uploadDevice.InvokeEnable) return;

        MqttUp(alarm, RealAlarmTopic);
    }

    protected override void DeviceStatusChnage(Device device)
    {
        if (!_uploadDevice.InvokeEnable) return;
        MqttUp(device, RealDeviceStatusTopic);
    }

}
