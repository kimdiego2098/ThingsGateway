using Furion.Logging.Extensions;

using Microsoft.Extensions.Logging;

using MQTTnet;
using MQTTnet.Server;


using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Application.Core;

public class DeviceCollectCore : DisposableObject
{

    private readonly ILogger _logger;

    /// <summary>
    /// 当前设备信息
    /// </summary>
    private Device _device;

    /// <summary>
    /// 当前的驱动插件设备
    /// </summary>
    private DriverBase _driver;

    /// <summary>
    /// 当前的驱动插件type
    /// </summary>
    private Type _driverInfo;

    /// <summary>
    /// 全部特殊方法变量信息
    /// </summary>
    public List<DeviceVariableMedRead> DeviceVariableMedReads = new();

    /// <summary>
    /// 全局插件服务
    /// </summary>
    private PluginService _pluginService;

    /// <summary>
    /// 全部基本读取变量信息
    /// </summary>
    private List<DeviceVariableSourceRead> DeviceVariableSourceReads = new();

    /// <summary>
    /// 循环线程取消标识
    /// </summary>
    public CancellationTokenSource StoppingToken = new CancellationTokenSource();

    /// <summary>
    /// 当前设备
    /// </summary>
    public Device Device => _device;

    /// <summary>
    /// 当前设备变量浅表
    /// </summary>
    public List<DeviceVariableCopy> DeviceVariablesCopy
    {
        get
        {
            var data = _device.DeviceVariables.ToJsonString();
            var vars = data.ToJsonObject<List<DeviceVariableCopy>>();
            return vars;
        }
    }

    /// <summary>
    /// 当前设备浅表
    /// </summary>
    public Device DeviceCopy
    {
        get
        {
            var data = _device.ToJsonString();
            var vars = data.ToJsonObject<Device>();
            return vars;
        }
    }


    /// <summary>
    /// 当前设备Id
    /// </summary>
    public long DeviceId => _device.Id;
    /// <summary>
    /// 当前设备全部特殊方法，执行初始化后获取正确值
    /// </summary>
    public List<MethodInfo> Methods { get; private set; }
    /// <summary>
    /// 当前设备全部设备属性，执行初始化后获取正确值
    /// </summary>
    public List<DeviceProperty> Propertys { get; private set; }
    private MqttServer _mqttServer;
    private AllDeviceData _allDeviceData;
    public DeviceCollectCore(ILogger logger, PluginService pluginService, AllDeviceData allDeviceData, MqttServer mqttServer)
    {
        _logger = logger; _mqttServer = mqttServer; _allDeviceData = allDeviceData;
        _pluginService = pluginService;
    }

    /// <summary>
    /// 初始化，这里只有在设备子线程创建或更新时才会执行
    /// </summary>
    public void Init(Device device, bool isUpDriver, bool IsUpVariable = false)
    {
        if (device == null) return;
        try
        {
            if (isUpDriver)
            {
                UpDriver(device.DriverAssembleName);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, device.Name);
        }

        _device = device;

        if (_driver != null)
        {
            SetPluginProperties(_device.DeviceProperties);
            _driver.Init(_device);
        }
        if (IsUpVariable)
        {
            LoadSourceReads(device.DeviceVariables);
        }
        else
        {
            if (_device != null)
                device.DeviceVariables = _device.DeviceVariables;
            LoadSourceReads(device.DeviceVariables);
        }
        //重新初始化设备属性 
        _allDeviceData.Devices.RemoveWhereChange(it => it.Id == device.Id);
        _allDeviceData.Devices.AddChange(device);
        if (_device.ChangeUpload)
            _device.DeviceVariables?.ForEach(v => { v.VariableCollectChange += DeviceVariableChange; });

        _device.DeviceStatus.Device = _device;
        _device.DeviceStatus.DeviceOnLineStatus = DeviceOnLineStatusEnum.Default;
        _device.DeviceStatus.ActiveTime = DateTime.MinValue;
        Init();
    }
    /// <summary>
    /// 设备变量变化后,添加到mqtt推送队列
    /// </summary>
    /// <param name="variable"></param>
    private void DeviceVariableChange(DeviceVariable variable)
    {
        var data = variable;
        CollectDeviceVariables.Enqueue(data);
    }

    private void UpDriver(string driverAssembleName)
    {
        PluginAssemble driver = _pluginService.GetDriverAssemble(driverAssembleName);
        _driverInfo = driver?.Type;
        _driver = _pluginService.CreateDriver(_driverInfo, _logger);
        Methods = _pluginService.GetMethod(_driver);
        Propertys = _pluginService.GetDriverProperties(_driver);
    }


    #region 设备子线程采集启动停止
    private Task<Task> DeviceTask;
    private Task<Task> MqttTask;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        StoppingToken = new CancellationTokenSource();
        DeviceTask = new Task<Task>(async () =>
        {
            _logger?.LogInformation($"设备采集线程开始:{_device.Name}");
            try
            {
                await Task.Delay(1000, StoppingToken.Token);
                //驱动插件执行循环前方法
                _driver?.BeforStart();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, _device.Name + "执行BeforStart方法出错：");
                Device.DeviceStatus.DeviceMedsVariableFailedNum = DeviceVariableSourceReads.Count;
                Device.DeviceStatus.DeviceSourceVariableFailedNum = DeviceVariableMedReads.Count;
                Device.DeviceStatus.DeviceOnLineStatus = DeviceOnLineStatusEnum.OnLineButNoInitialValue;
            }
            Device.DeviceStatus.DeviceSourceVariableNum = DeviceVariableSourceReads.Count;
            Device.DeviceStatus.DeviceMedsVariableNum = DeviceVariableMedReads.Count;
            while (!StoppingToken.Token.IsCancellationRequested)
            {
                try
                {
                    if (_driver == null) continue;

                    if (Device?.InvokeEnable == false)
                    {
                        Device.DeviceStatus.DeviceOnLineStatus = DeviceOnLineStatusEnum.Pause;
                        await Task.Delay(500, StoppingToken.Token);
                        continue;
                    }
                    try
                    {

                        await Task.Delay(500, StoppingToken.Token);

                        Device.DeviceStatus.ActiveTime = DateTime.Now;
                        int deviceMedsVariableSuccessNum = 0;
                        int deviceMedsVariableFailedNum = 0;
                        int deviceSourceVariableSuccessNum = 0;
                        int deviceSourceVariableFailedNum = 0;
                        if (StoppingToken.Token.IsCancellationRequested)
                            break;
                        if (_driver.IsSupportAddressRequest())
                        {
                            foreach (var deviceVariableSourceRead in DeviceVariableSourceReads)
                            {
                                if (Device?.InvokeEnable == false)
                                {
                                    continue;
                                }

                                if (StoppingToken.Token.IsCancellationRequested)
                                    break;

                                //连读变量
                                if (deviceVariableSourceRead.CheckIfRequestAndUpdateTime(DateTime.Now))
                                {

                                    var read = await _driver.ReadSourceAsync(deviceVariableSourceRead);
                                    if (read != null && read.IsSuccess)
                                    {
                                        _logger?.LogTrace(_device.Name + "采集[" + deviceVariableSourceRead.Address + " -" + deviceVariableSourceRead.Length + "] 数据成功" + read.Content?.ToHexString(" "));
                                        deviceMedsVariableSuccessNum += 1;
                                    }
                                    else if (read != null && read.IsSuccess == false)
                                    {
                                        _logger?.LogWarning(_device.Name + "采集[" + deviceVariableSourceRead.Address + " -" + deviceVariableSourceRead.Length + "] 数据失败 - " + read?.Message);
                                        deviceMedsVariableFailedNum += 1;
                                    }
                                }

                                await Task.Delay(50, StoppingToken.Token);

                            }

                            foreach (var deviceVariableMedRead in DeviceVariableMedReads)
                            {
                                if (Device?.InvokeEnable == false)
                                    continue;
                                if (StoppingToken.IsCancellationRequested)
                                    break;

                                //连读变量
                                if (deviceVariableMedRead.CheckIfRequestAndUpdateTime(DateTime.Now))
                                {
                                    var read = InVokeMed(deviceVariableMedRead);
                                    if (read != null && read.IsSuccess)
                                    {
                                        _logger?.LogDebug(_device.Name + "执行方法[" + deviceVariableMedRead.MedInfo.Name + "] 成功" + read.Content.ToJsonString());
                                        deviceMedsVariableSuccessNum += 1;
                                    }
                                    else if ((read != null && read.IsSuccess == false) || read == null)
                                    {
                                        _logger?.LogWarning(_device.Name + "执行方法[" + deviceVariableMedRead.MedInfo.Name + "] 失败" + read?.Message);
                                        deviceMedsVariableFailedNum += 1;
                                    }
                                }

                                await Task.Delay(50, StoppingToken.Token);

                            }

                            Device.DeviceStatus.DeviceMedsVariableSuccessNum = deviceMedsVariableSuccessNum;
                            Device.DeviceStatus.DeviceSourceVariableSuccessNum = deviceSourceVariableSuccessNum;
                            Device.DeviceStatus.DeviceMedsVariableFailedNum = deviceMedsVariableFailedNum;
                            Device.DeviceStatus.DeviceSourceVariableFailedNum = deviceSourceVariableFailedNum;
                            if (deviceMedsVariableFailedNum == 0 && deviceSourceVariableFailedNum == 0)
                            {
                                Device.DeviceStatus.DeviceOnLineStatus = DeviceOnLineStatusEnum.OnLine;
                            }
                            else
                            {
                                Device.DeviceStatus.DeviceOnLineStatus = DeviceOnLineStatusEnum.OnLineButNoInitialValue;
                            }



                        }

                    }
                    catch (TaskCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, $"采集线程循环异常{_device.Name}");
                    }

                }
                catch (TaskCanceledException)
                {

                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"采集线程循环异常{_device.Name}");
                }
            }
        }
         , StoppingToken.Token
         //, TaskCreationOptions.LongRunning

         );
        //MQTT推送
        MqttTask = new Task<Task>(async () =>
        {
            _logger?.LogInformation($"开始推送mqtt:{_device.Name}");
            //驱动插件执行循环前方法
            while (!StoppingToken.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(3000, StoppingToken.Token);
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                    if (Device?.ChangeUpload == false)
                    {
                        continue;
                    }
                    ////变化推送
                    var deviceMessage = new MqttApplicationMessageBuilder().WithTopic($"device/devicestatus/{Device.Name}")
                    .WithPayload(Device.ToJsonStringWithSettings(serializerSettings)).Build();
                    await _mqttServer.InjectApplicationMessage(
                            new InjectedMqttApplicationMessage(deviceMessage)
                            {
                                SenderClientId = "thingsgateway"
                            });

                    ////变化推送
                    var list = CollectDeviceVariables.ToListWithDequeue(1000);
                    if (list?.Count == 0) continue;
                    var variableMessage = new MqttApplicationMessageBuilder()
                    .WithTopic($"device/variablestatus/{Device.Name}")
                    .WithPayload(list.ToJsonStringWithSettings(serializerSettings)).Build();
                    await _mqttServer.InjectApplicationMessage(
                            new InjectedMqttApplicationMessage(variableMessage)
                            {
                                SenderClientId = "thingsgateway"
                            });

                }
                catch (TaskCanceledException)
                {

                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"采集线程循环异常{_device.Name}");
                }
            }
        }
, StoppingToken.Token

);


    }


    /// <summary>
    /// 开始采集
    /// </summary>
    public void StartThread()
    {
        DeviceTask.Start();
        MqttTask.Start();

    }

    /// <summary>
    /// 停止线程
    /// </summary>
    public void StopThread()
    {
        StoppingToken?.Cancel();
        _logger?.LogInformation($"执行线程取消，等待线程结束:{_device.Name}");
        var devResult = DeviceTask.Result;
        if (devResult?.Status != TaskStatus.Canceled)
            devResult?.Wait(5000);
        var mqttResult = MqttTask.Result;
        if (mqttResult?.Status != TaskStatus.Canceled)
            mqttResult?.Wait(5000);
        //这里需要执行驱动的链接断开
        _driver?.AfterStop();
        _logger?.LogInformation($"线程结束:{_device.Name}");
    }
    /// <summary>
    /// 暂停采集
    /// </summary>
    public void PasueThread(bool enable)
    {
        lock (this)
        {
            var str = enable == false ? "设备线程采集暂停" : "设备线程采集继续";
            _logger?.LogInformation($"{str}:{_device.Name}");
            this.Device.InvokeEnable = enable;
        }
    }

    #endregion

    #region 驱动信息获取

    /// <summary>
    /// 传入设备变量列表，执行后赋值<see cref="DeviceVariableSourceReads"/>
    /// </summary>
    /// <param name="deviceVariables"></param>
    private void LoadSourceReads(List<DeviceVariable> deviceVariables)
    {
        if (deviceVariables == null || _driver == null) { return; }
        try
        {
            var tag = deviceVariables.Where(it => it.ProtectType != ProtectTypeEnum.WriteOnly && it.OtherMethod.IsNullOrEmpty()).ToList();
            var result = _driver.LoadSourceRead(tag);
            if (result.IsSuccess)
                DeviceVariableSourceReads = result.Content;
        }
        catch (Exception ex)
        {
            _logger?.LogError("自动分包方法失败:{0}", ex);
        }
        try
        {
            var variablesMed = deviceVariables.Where(it => !string.IsNullOrEmpty(it.OtherMethod));
            var tag = variablesMed.Where(it => it.ProtectType != ProtectTypeEnum.WriteOnly).ToList();
            var variablesMedResult = new List<DeviceVariableMedRead>();
            foreach (var item in tag)
            {
                var medResult = new DeviceVariableMedRead(item.InvokeInterval);
                var med = Methods.FirstOrDefault(it => it.Name == item.OtherMethod);
                if (med != null)
                {
                    medResult.MedInfo = new Method(med);
                    medResult.MedStr = item.VariableAddress;
                    medResult.DeviceVariable = item;
                    variablesMedResult.Add(medResult);
                }
            }
            DeviceVariableMedReads = variablesMedResult;
        }
        catch (Exception ex)
        {
            _logger?.LogError("获取特殊方法失败:{0}", ex);

        }
    }

    #endregion

    #region 设备读写与mqtt推送
    private IntelligentConcurrentQueue<DeviceVariable> CollectDeviceVariables { get; set; } = new(10000);

    /// <summary>
    /// 执行特殊方法，方法参数已";"分割
    /// </summary>
    /// <param name="deviceVariableMedRead"></param>
    /// <returns></returns>
    public OperResult<string> InVokeMed(DeviceVariableMedRead deviceVariableMedRead)
    {
        var result = new OperResult<string>();
        var method = deviceVariableMedRead.MedInfo;
        if (method == null)
        {
            result.ResultCode = ResultCode.Error;
            result.Message = deviceVariableMedRead.DeviceVariable.Name +
            "找不到执行方法" + deviceVariableMedRead.DeviceVariable.OtherMethod;
        }
        else
        {
            if (deviceVariableMedRead.MedObj == null)
            {
                var ps = method.Info.GetParameters();
                deviceVariableMedRead.MedObj = new object[ps.Length];

                if (!deviceVariableMedRead.MedStr.IsNullOrEmpty())
                {
                    string[] strs = deviceVariableMedRead.MedStr?.Split(';');
                    int index = 0;
                    for (int i = 0; i < ps.Length; i++)
                    {
                        if (strs.Length <= i) { throw new("参数不足"); }

                        //os[i] = Convert.ChangeType(strs[index], ps[i].ParameterType);
                        deviceVariableMedRead.MedObj[i] = deviceVariableMedRead.Converter.ConvertFrom(strs[index], ps[i].ParameterType);
                        index++;
                    }

                }
            }
            try
            {
                var data = (OperResult)method.Invoke(_driver, deviceVariableMedRead.MedObj);
                var resultobj = data.ChangedType<object>();
                result = resultobj.Copy<string>();
                if (method.HasReturn && resultobj.IsSuccess)
                {
                    result.Content = deviceVariableMedRead.Converter.ConvertTo(resultobj.Content?.ToString()?.Replace($"\0", ""));
                    deviceVariableMedRead.DeviceVariable.Value = result.Content;
                }
                else
                {
                    deviceVariableMedRead.DeviceVariable.Quality = 0;
                    _logger?.LogError(resultobj.Message, deviceVariableMedRead.DeviceVariable.Name +
"执行方法失败" + deviceVariableMedRead.DeviceVariable.OtherMethod);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, deviceVariableMedRead.DeviceVariable.Name +
            "执行方法失败" + deviceVariableMedRead.DeviceVariable.OtherMethod);
            }
        }

        return result;
    }

    public OperResult InvokeMed(Method coreMethod, object par)
    {
        return (OperResult)coreMethod.Invoke(_driver, par);
    }

    /// <summary>
    /// 执行写入
    /// </summary>
    /// <returns></returns>
    public Task<OperResult> InVokeWriteAsync(DeviceVariable deviceVariable, string value)
    {
        try
        {
            return _driver.WriteValueByNameAsync(deviceVariable, value);
        }
        catch (Exception ex)
        {
            return Task.FromResult(new OperResult(ex.Message + Environment.NewLine + ex.StackTrace));
        }
    }

    /// <summary>
    /// 设置驱动插件的属性值
    /// </summary>
    public void SetPluginProperties(List<DeviceProperty> deviceProperties)
    {
        if (deviceProperties == null) return;
        var pluginPropertys = _driver.GetType().GetAllProperties();
        foreach (var propertyInfo in pluginPropertys)
        {
            var propAtt = propertyInfo.GetCustomAttribute(typeof(DevicePropertyAttribute));
            var deviceProperty = deviceProperties.FirstOrDefault(x => x.DevicePropertyName == propertyInfo.Name);
            if (propAtt == null || deviceProperty == null || string.IsNullOrEmpty(deviceProperty?.Value?.ToString())) continue;
            var value = ReadWriteHelpers.ObjToTypeValue(propertyInfo, deviceProperty.Value);
            propertyInfo.SetValue(_driver, value);
        }
    }


    #endregion




    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        StopThread();
        //这里需要执行驱动的资源释放
        _driver?.Dispose();
    }




}

