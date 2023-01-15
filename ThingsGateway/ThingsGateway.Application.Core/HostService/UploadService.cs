using Furion.Logging.Extensions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MQTTnet.Server;

namespace ThingsGateway.Application.Core;

/// <summary>
/// 设备采集后台服务
/// </summary>
public class UploadService : BackgroundService, ISingleton
{
    public void SetPluginProperties(long id, List<UploadDeviceProperty> deviceProperties)
    {
        var devcore = UploadCores.FirstOrDefault(it => it?.DeviceId == id);
        if (devcore == null) throw new("找不到该设备！");
        devcore.SetPluginProperties(deviceProperties);
    }
    private readonly ILogger<UploadService> _logger;
    /// <summary>
    /// 全局设备信息
    /// </summary>
    private AllDeviceData _allDeviceData;
    /// <summary>
    /// 设备仓储类
    /// </summary>
    private SqlSugarRepository<UploadDevice> _deviceRep;

    /// <summary>
    /// 全局插件服务
    /// </summary>
    private PluginService _pluginService;
    /// <summary>
    /// 后台设备子服务列表
    /// </summary>
    public ConcurrentList<UploadCore> UploadCores { get; private set; } = new ConcurrentList<UploadCore>();
    /// <summary>
    /// 后台设备Id与名称
    /// </summary>
    public List<DevIdName> DevIdNames
    {
        get
        {
            return UploadCores.Select(it => new DevIdName() { Id = it.DeviceId, Name = it.UploadDevice.Name }).ToList();
        }
    }

    private MqttServer _mqttServer;
    public UploadService(ILogger<UploadService> logger, MqttServer mqttServer
        , IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _mqttServer = mqttServer;
        var scope = scopeFactory.CreateScope();
        var services = scope.ServiceProvider;


        _allDeviceData = services.GetService<AllDeviceData>();
        _pluginService = services.GetService<PluginService>();
        _deviceRep = services.GetService<SqlSugarRepository<UploadDevice>>();

    }

    #region 设备创建更新结束

    /// <summary>
    /// 创建设备采集线程并启动
    /// </summary>
    public void CreateDeviceThread(UploadDevice device, bool isUpDriver)
    {
        _logger?.LogDebug($"创建设备线程 启动:{device.Name}");

        UploadCore deviceCollectCore = new UploadCore(_logger, _pluginService, _allDeviceData, _mqttServer);
        UploadCores.Add(deviceCollectCore);
        deviceCollectCore.Init(device, isUpDriver);
        deviceCollectCore.StartThread();

        _logger?.LogDebug($"创建设备线程 结束:{device.Name}");
    }

    /// <summary>
    /// 创建设备采集线程不启动
    /// </summary>
    public void CreateDeviceThreadNoStart(UploadDevice device, bool isUpDriver)
    {
        _logger?.LogDebug($"创建设备线程 启动:{device.Name}");

        UploadCore deviceCollectCore = new UploadCore(_logger, _pluginService, _allDeviceData, _mqttServer);
        UploadCores.Add(deviceCollectCore);
        deviceCollectCore.Init(device, isUpDriver);

        _logger?.LogDebug($"创建设备线程 结束:{device.Name}");
    }

    /// <summary>
    /// 启动全部线程
    /// </summary>
    public void StartAllDeviceThread()
    {
        foreach (UploadCore deviceCollectCore in UploadCores)
        {
            deviceCollectCore.StartThread();
        }
    }

    /// <summary>
    /// 更新设备
    /// </summary>
    public void UpDeviceThread(UploadDevice device, bool isUpDriver)
    {
        var devcore = UploadCores.FirstOrDefault(it => it?.DeviceId == device?.Id);
        if (devcore == null) throw new("找不到该设备！");
        //这里先停止采集，操作会使线程取消，需要重新恢复线程
        devcore.StopThread();
        devcore.Init(device, isUpDriver);
        devcore.StartThread();

    }

    /// <summary>
    /// 移除设备线程，并且释放资源
    /// </summary>
    /// <param name="devices"></param>
    public void RemoveDeviceThread(UploadDevice devices)
    {
        var deviceThread = UploadCores.FirstOrDefault(x => x.DeviceId == devices.Id);
        if (deviceThread != null)
        {
            UploadCores.Remove(deviceThread);
            deviceThread.Dispose();
        }
    }

    /// <summary>
    /// 控制设备线程启停
    /// </summary>
    public void ConfigDeviceThread(long deviceId, bool isStart)
    {
        if (deviceId <= 0)
        {
            UploadCores.ForEach(it => it.PasueThread(isStart));
        }
        else
            UploadCores.FirstOrDefault(it => it.DeviceId == deviceId)?.PasueThread(isStart);
    }

    #endregion

    #region 设备信息获取

    /// <summary>
    /// 获取设备属性
    /// </summary>
    public Task<List<UploadDeviceProperty>> GetUploadDevicePropertysAsync(string driverAssembleName)
    {
        List<UploadDeviceProperty> Propertys = new();
        var driver = _pluginService.UploadInfos.
        Select(it => it.PluginAssemble).
        FirstOrDefault(it =>
        it.Any(it => it.AssembleName == driverAssembleName)
                    );
        if (driver == null)
        {
            throw new($"找不到设备驱动:[{driverAssembleName}]");
        }
        else
        {
            var _driverInfo = driver.Where(it => it.AssembleName == driverAssembleName).FirstOrDefault()?.Type;
            var _driver = _pluginService.CreateUpload(_driverInfo, _logger);
            Propertys = _pluginService.GetUploadProperties(_driver);

        }
        return Task.FromResult(Propertys);

    }

    #endregion

    #region worker服务
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var item in UploadCores)
        {
            try
            {
                item.Dispose();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, item.UploadDevice.Name);
            }
        }
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);
        await Init();
        while (!stoppingToken.IsCancellationRequested)
        {
            //这里不采用CancellationToken控制子线程，直接循环保持，结束时调用子设备线程Dispose
            //检测设备采集线程假死
            int num = UploadCores.Count;
            for (int i = 0; i < num; i++)
            {
                UploadCore devcore = UploadCores[i];
                if (devcore.UploadDevice.UploadDeviceStatus.ActiveTime != DateTime.MinValue && devcore.UploadDevice.UploadDeviceStatus.ActiveTime.AddMinutes(1) <= DateTime.Now)
                {
                    if (devcore.StoppingToken.Token.IsCancellationRequested)
                        continue;
                    if (devcore.UploadDevice.UploadDeviceStatus.DeviceOnLineStatus == DeviceOnLineStatusEnum.Pause)
                        continue;
                    _logger?.LogWarning(devcore.UploadDevice.Name + "上传线程假死，重启线程中");
                    RemoveDeviceThread(devcore.UploadDevice);
                    CreateDeviceThread(devcore.UploadDevice, true);
                    i--;
                    num--;
                    GC.Collect();
                }
            }
            await Task.Delay(60000, stoppingToken);
        }
    }

    #endregion

    /// <summary>
    ///  启动进程时初始化，后续动态增减设备由对应方法执行
    /// </summary>
    /// <returns></returns>
    private async Task Init()
    {

        await Task.Delay(100);
        var dev = await _deviceRep.Context.CopyNew().Queryable<UploadDevice>()
            .Includes(a => a.DeviceProperties, t => t.UploadDevice)
           .ToListAsync();
        foreach (var device in dev)
        {
            try
            {
                CreateDeviceThreadNoStart(device, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, device.Name);
            }
        }
        StartAllDeviceThread();
    }

}

