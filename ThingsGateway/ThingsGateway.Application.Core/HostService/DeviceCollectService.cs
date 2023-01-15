using Furion.Logging.Extensions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MQTTnet.Server;

using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Application.Core;

/// <summary>
/// 设备采集后台服务
/// </summary>
public class DeviceCollectService : BackgroundService, ISingleton
{

    private readonly ILogger<DeviceCollectService> _logger;
    /// <summary>
    /// 全局设备信息
    /// </summary>
    private AllDeviceData _allDeviceData;
    /// <summary>
    /// 设备仓储类
    /// </summary>
    private SqlSugarRepository<Device> _deviceRep;
    ///// <summary>
    ///// 内存变量仓储类
    ///// </summary>
    //private SqlSugarRepository<MemoryVariable> _memoryVariableRep;
    /// <summary>
    /// 全局插件服务
    /// </summary>
    private PluginService _pluginService;
    /// <summary>
    /// 后台设备子服务列表
    /// </summary>
    public ConcurrentList<DeviceCollectCore> DeviceCollectCores { get; private set; } = new ConcurrentList<DeviceCollectCore>();
    /// <summary>
    /// 后台设备Id与名称
    /// </summary>
    public List<DevIdName> DevIdNames
    {
        get
        {
            return DeviceCollectCores.Select(it => new DevIdName() { Id = it.DeviceId, Name = it.Device.Name }).ToList();
        }
    }

    public void SetPluginProperties(long id, List<DeviceProperty> deviceProperties)
    {
        var devcore = DeviceCollectCores.FirstOrDefault(it => it?.DeviceId == id);
        if (devcore == null) throw new("找不到该设备！");
        devcore.SetPluginProperties(deviceProperties);
    }
    private MqttServer _mqttServer;
    private IServiceProvider _serviceProvider;
    public DeviceCollectService(ILogger<DeviceCollectService> logger, MqttServer mqttServer,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _mqttServer = mqttServer;
        _serviceProvider = serviceProvider.CreateScope().ServiceProvider;


        _allDeviceData = _serviceProvider.GetService<AllDeviceData>();
        _pluginService = _serviceProvider.GetService<PluginService>();
        _deviceRep = _serviceProvider.GetService<SqlSugarRepository<Device>>();
        //_memoryVariableRep = services.GetService<SqlSugarRepository<MemoryVariable>>();
    }

    #region 设备创建更新结束

    /// <summary>
    /// 创建设备采集线程并启动
    /// </summary>
    public void CreateDeviceThread(Device device, bool isUpDriver, bool isUpVariable)
    {
        _logger?.LogDebug($"创建设备线程 启动:{device.Name}");

        DeviceCollectCore deviceCollectCore = new DeviceCollectCore(_logger, _pluginService, _allDeviceData, _mqttServer);
        deviceCollectCore.Init(device, isUpDriver, isUpVariable);
        deviceCollectCore.StartThread();
        DeviceCollectCores.Add(deviceCollectCore);

        _logger?.LogDebug($"创建设备线程 结束:{device.Name}");
    }

    /// <summary>
    /// 创建设备采集线程不启动
    /// </summary>
    public void CreateDeviceThreadNoStart(Device device, bool isUpDriver, bool isUpVariable)
    {
        _logger?.LogDebug($"创建设备线程 启动:{device.Name}");

        DeviceCollectCore deviceCollectCore = new DeviceCollectCore(_logger, _pluginService, _allDeviceData, _mqttServer);
        deviceCollectCore.Init(device, isUpDriver, isUpVariable);
        DeviceCollectCores.Add(deviceCollectCore);

        _logger?.LogDebug($"创建设备线程 结束:{device.Name}");
    }

    /// <summary>
    /// 启动全部线程
    /// </summary>
    public void StartAllDeviceThread()
    {
        foreach (DeviceCollectCore deviceCollectCore in DeviceCollectCores)
        {
            deviceCollectCore.StartThread();
        }
    }

    /// <summary>
    /// 更新设备
    /// </summary>
    public void UpDeviceThread(Device device, bool isUpDriver, bool IsUpVariable)
    {
        var devcore = DeviceCollectCores.FirstOrDefault(it => it?.DeviceId == device?.Id);
        if (devcore == null) throw new("找不到该设备！");
        //这里先停止采集，操作会使线程取消，需要重新恢复线程
        devcore.StopThread();
        devcore.Init(device, isUpDriver, IsUpVariable);
        devcore.StartThread();

    }

    /// <summary>
    /// 移除设备线程，并且释放资源
    /// </summary>
    /// <param name="devices"></param>
    public void RemoveDeviceThread(Device devices)
    {
        var deviceThread = DeviceCollectCores.FirstOrDefault(x => x.DeviceId == devices.Id);
        if (deviceThread != null)
        {
            deviceThread.Dispose();
            DeviceCollectCores.Remove(deviceThread);
        }
    }

    /// <summary>
    /// 控制设备线程启停
    /// </summary>
    public void ConfigDeviceThread(long deviceId, bool isStart)
    {
        if (deviceId <= 0)
        {
            DeviceCollectCores.ForEach(it => it.PasueThread(isStart));
        }
        else
            DeviceCollectCores.FirstOrDefault(it => it.DeviceId == deviceId)?.PasueThread(isStart);
    }

    #endregion

    #region 设备信息获取

    /// <summary>
    /// 根据设备名称获取设备特殊方法
    /// </summary>
    public List<string> GetDeviceMethodsAsync(long deviceId)
    {
        var devcore = DeviceCollectCores.FirstOrDefault(it => it.Device.Id == deviceId);
        return devcore.Methods.Select(it => it.Name).ToList();
    }

    /// <summary>
    /// 获取设备属性
    /// </summary>
    public Task<List<DeviceProperty>> GetDevicePropertysAsync(string driverAssembleName)
    {
        List<DeviceProperty> Propertys = new();
        var driver = _pluginService.DriverInfos.
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
            var _driver = _pluginService.CreateDriver(_driverInfo, _logger);
            Propertys = _pluginService.GetDriverProperties(_driver);

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
        foreach (var item in DeviceCollectCores)
        {
            try
            {
                item.Dispose();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, item?.Device?.Name);
            }
        }
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Init();
        while (!stoppingToken.IsCancellationRequested)
        {
            //这里不采用CancellationToken控制子线程，直接循环保持，结束时调用子设备线程Dispose
            //检测设备采集线程假死
            var num = DeviceCollectCores.Count;
            for (int i = 0; i < num; i++)
            {
                DeviceCollectCore devcore = DeviceCollectCores[i];
                if (devcore.Device.DeviceStatus.ActiveTime != DateTime.MinValue && devcore.Device.DeviceStatus.ActiveTime.AddMinutes(1) <= DateTime.Now)
                {
                    if (devcore.StoppingToken.Token.IsCancellationRequested)
                        continue;
                    if (devcore.Device.DeviceStatus.DeviceOnLineStatus == DeviceOnLineStatusEnum.Pause)
                        continue;
                    _logger?.LogWarning(devcore.Device.Name + "采集线程假死，重启线程中");
                    RemoveDeviceThread(devcore.Device);
                    CreateDeviceThread(devcore.Device, true, true);
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

        var dev = await _deviceRep.AsQueryable()
            .Includes(a => a.DeviceProperties, t => t.Device)
           .Includes(b => b.DeviceVariables, a => a.VariableAlarms, c => c.Variable)
           .Includes(b => b.DeviceVariables, a => a.VariableHiss, c => c.Variable)
           .Includes(b => b.DeviceVariables, a => a.VariablePropertys, c => c.Variable)
           .Includes(a => a.DeviceVariables, b => b.Device)
           .ToListAsync();
        //var data = await _memoryVariableRep.AsQueryable().ToListAsync();
        //if (data?.Count > 0)
        //    _allDeviceData.MemoryVariables.AddRange(data);
        foreach (var device in dev)
        {
            try
            {
                CreateDeviceThreadNoStart(device, true, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, device.Name);
            }
        }
        StartAllDeviceThread();
    }


    public async Task<OperResult> InvokeDeviceMed(object MethodBase, Dictionary<string, object> par, string id = null)
    {
        OperResult data = new("UnKnow");
        if (par?.Count == 0) throw new Exception("不存在有效参数");
        foreach (var item in par)
        {
            var tag = _allDeviceData.DeviceVariables.FirstOrDefault(it => it.Name == item.Key);
            if (tag == null) throw new Exception("不存在变量:" + item.Key);
            if (tag.ProtectType == ProtectTypeEnum.ReadOnly) throw new Exception("只读变量");
            var dev = DeviceCollectCores.FirstOrDefault(it => it.Device.Id == tag.DeviceId);

            if (tag.OtherMethod == null)
            {
                data = (await dev.InVokeWriteAsync(tag, item.Value?.ToString())).Copy();
                _logger.LogInformation(string.Format(Environment.NewLine + "{0};执行写入;"
    + Environment.NewLine + "变量:{1};"
    + Environment.NewLine + "值:{2};" +
    Environment.NewLine + "结果:{3}",
    MethodBase.ToString(), tag.Name, item.Value?.ToString(), data.Message));
            }
            else
            {
                var med = dev.Methods.FirstOrDefault(it => it.Name == tag.OtherMethod);
                var coreMed = new Method(med);
                try
                {
                    data = dev.InvokeMed(coreMed, item.Value);
                }
                catch (Exception ex)
                {
                    data = new OperResult<string>(ex);
                }

                _logger.LogInformation(string.Format(Environment.NewLine + "{0};执行方法调用;"
    + Environment.NewLine + "变量:{1};"
    + Environment.NewLine + "方法名称:{2};"
    + Environment.NewLine + "参数:{3};" +
    Environment.NewLine + "结果:{4}",
    MethodBase.ToString(), tag.Name, tag.OtherMethod, item.Value?.ToString(), data.Message));
            }

        }
        return data;
    }
}

