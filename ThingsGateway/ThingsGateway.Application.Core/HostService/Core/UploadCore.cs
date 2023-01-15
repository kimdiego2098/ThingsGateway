using Furion.Logging.Extensions;

using Microsoft.Extensions.Logging;

using MQTTnet.Server;

using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Application.Core;

public class UploadCore : DisposableObject
{
    private readonly ILogger _logger;

    /// <summary>
    /// 当前设备信息
    /// </summary>
    private UploadDevice _uploadDevice;

    /// <summary>
    /// 当前的驱动插件设备
    /// </summary>
    private UpLoadBase _upload;

    /// <summary>
    /// 当前的驱动插件type
    /// </summary>
    private Type _driverInfo;

    /// <summary>
    /// 全局插件服务
    /// </summary>
    private PluginService _pluginService;


    /// <summary>
    /// 循环线程取消标识
    /// </summary>
    public CancellationTokenSource StoppingToken = new CancellationTokenSource();

    /// <summary>
    /// 当前设备
    /// </summary>
    public UploadDevice UploadDevice => _uploadDevice;

    /// <summary>
    /// 当前设备浅表
    /// </summary>
    public UploadDevice DeviceCopy
    {
        get
        {
            var vars = _uploadDevice.Adapt<UploadDevice>();
            return vars;
        }
    }


    /// <summary>
    /// 当前设备Id
    /// </summary>
    public long DeviceId => _uploadDevice.Id;

    /// <summary>
    /// 当前设备全部设备属性，执行初始化后获取正确值
    /// </summary>
    public List<UploadDeviceProperty> Propertys { get; private set; }
    private MqttServer _mqttServer;
    private AllDeviceData _allDeviceData;
    public UploadCore(ILogger logger, PluginService pluginService, AllDeviceData allDeviceData, MqttServer mqttServer)
    {
        _logger = logger; _mqttServer = mqttServer; _allDeviceData = allDeviceData;
        _pluginService = pluginService;
    }

    /// <summary>
    /// 初始化，这里只有在设备子线程创建或更新时才会执行
    /// </summary>
    public void Init(UploadDevice device, bool isUpUpload)
    {
        if (device == null) return;

        try
        {
            if (isUpUpload)
            {
                UpUpload(device.DriverAssembleName);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, device.Name);
        }

        //重新初始化设备属性 
        _uploadDevice = device;
        if (_upload != null)
        {
            SetPluginProperties(_uploadDevice.DeviceProperties);
        }
        _uploadDevice.UploadDeviceStatus.DeviceOnLineStatus = DeviceOnLineStatusEnum.Default;
        Init();
    }

    private void UpUpload(string driverAssembleName)
    {
        PluginAssemble driver = _pluginService.GetUploadAssemble(driverAssembleName);
        _driverInfo = driver?.Type;
        _upload = _pluginService.CreateUpload(_driverInfo, _logger);
        Propertys = _pluginService.GetUploadProperties(_upload);
    }

    #region 设备子线程上传启动停止
    private Task<Task> DeviceTask;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        StoppingToken = new();
        DeviceTask = new Task<Task>(async () =>
        {
            _logger?.LogInformation($"设备上传线程开始:{_uploadDevice.Name}");
            await Task.Delay(1000, StoppingToken.Token);
            try
            {
                if (_upload != null)
                    await _upload.StartAsync(_uploadDevice, StoppingToken.Token);
            }
            catch (TaskCanceledException)
            {

            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, _uploadDevice.Name + "设备上传线程出错");
            }

        }
         , StoppingToken.Token
         //, TaskCreationOptions.LongRunning

         );
    }


    /// <summary>
    /// 开始上传
    /// </summary>
    public void StartThread()
    {
        StoppingToken = new();
        DeviceTask.Start();
    }

    /// <summary>
    /// 停止线程
    /// </summary>
    public void StopThread()
    {
        StoppingToken?.Cancel();
        _logger?.LogInformation($"执行线程取消，等待线程结束:{_uploadDevice.Name}");
        var devResult = DeviceTask.Result;
        if (devResult?.Status != TaskStatus.Canceled)
            devResult?.Wait();
        _logger?.LogInformation($"线程即将结束:{_uploadDevice.Name}");
        //这里需要执行驱动的链接断开
        _upload?.Stop();
        _logger?.LogInformation($"线程结束:{_uploadDevice.Name}");
    }
    /// <summary>
    /// 暂停上传
    /// </summary>
    public void PasueThread(bool enable)
    {
        var str = enable == false ? "设备线程上传暂停" : "设备线程上传继续";
        _logger?.LogInformation($"{str}:{_uploadDevice.Name}");
        this.UploadDevice.InvokeEnable = enable;
    }

    #endregion


    #region 设备读写

    /// <summary>
    /// 设置驱动插件的属性值
    /// </summary>
    public void SetPluginProperties(List<UploadDeviceProperty> deviceProperties)
    {
        if (deviceProperties == null) return;
        var pluginPropertys = _upload.GetType().GetAllProperties();
        foreach (var propertyInfo in pluginPropertys)
        {
            var propAtt = propertyInfo.GetCustomAttribute(typeof(DevicePropertyAttribute));
            var deviceProperty = deviceProperties.FirstOrDefault(x => x.UploadDevicePropertyName == propertyInfo.Name);
            if (propAtt == null || deviceProperty == null || string.IsNullOrEmpty(deviceProperty?.Value?.ToString())) continue;
            var value = ReadWriteHelpers.ObjToTypeValue(propertyInfo, deviceProperty.Value);
            propertyInfo.SetValue(_upload, value);
        }
    }


    #endregion


    protected override void Dispose(bool disposing)
    {
        try
        {
            base.Dispose(disposing);
            StopThread();
            //这里需要执行驱动的资源释放
            _upload?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ToString());
        }

    }


}

