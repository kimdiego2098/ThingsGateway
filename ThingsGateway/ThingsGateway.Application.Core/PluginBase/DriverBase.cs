using Microsoft.Extensions.Logging;

namespace ThingsGateway.Application.Core;
/// <summary>
/// 驱动插件，继承实现不同PLC通讯
/// 约定构造函数参数为<see cref="ILogger"/>
/// 属性暴露使用<see cref="DevicePropertyAttribute"/>特性标识
/// 读取字符串，DateTime等等不确定返回字节数量的方法属性特殊方法，需使用<see cref="MethodAttribute"/>特性标识
/// </summary>
public abstract class DriverBase : IDisposable
{

    public TouchSocketConfig config;

    protected ILogger _logger;

    private bool isLogOut;
    private ILogger privateLogger;

    public DriverBase(ILogger logger)
    {
        privateLogger = logger;
        config = new TouchSocketConfig();
        config.ConfigureContainer(a => a.RegisterSingleton<ILog>(new EasyLogger(Log_Out)));
    }

    [DeviceProperty("Debug日志", "")]
    public bool IsLogOut
    {
        get => isLogOut; set
        {
            isLogOut = value;
            if (value)
                _logger = privateLogger;
        }
    }
    /// <summary>
    /// 数据转换器
    /// </summary>
    /// <returns></returns>
    public abstract IThingsGatewayBitConverter ThingsGatewayBitConverter { get; }

    /// <summary>
    /// 结束通讯后执行的方法
    /// </summary>
    /// <returns></returns>
    public abstract void AfterStop();

    /// <summary>
    /// 开始通讯前执行的方法
    /// </summary>
    /// <returns></returns>
    public abstract void BeforStart();

    public abstract void Dispose();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    public virtual void Init(Device device, object client = null)
    {

    }

    /// <summary>
    /// 返回是否支持读取
    /// </summary>
    /// <returns></returns>
    public abstract bool IsSupportAddressRequest();

    /// <summary>
    /// 连读分包，返回实际通讯包信息<see cref="DeviceVariableSourceRead"/> 
    /// <br></br>这部分每个驱动不一样，所以需要实现这个接口
    /// </summary>
    /// <param name="deviceVariables">设备下的全部通讯点位</param>
    /// <returns></returns>
    public abstract OperResult<List<DeviceVariableSourceRead>> LoadSourceRead(List<DeviceVariable> deviceVariables);

    /// <summary>
    /// 采集驱动读取，支持PLC多读的，地址与长度转换为;分割
    /// </summary>
    /// <param name="deviceVariableSourceRead"></param>
    /// <returns></returns>
    public virtual async Task<OperResult<byte[]>> ReadSourceAsync(DeviceVariableSourceRead deviceVariableSourceRead)
    {
        ushort length;
        if (!ushort.TryParse(deviceVariableSourceRead.Length, out length))
            return new OperResult<byte[]>("解析失败 长度[" + deviceVariableSourceRead.Length + "] 解析失败 :");
        OperResult<byte[]> read = await ReadAsync(deviceVariableSourceRead.Address, length);
        if (!read.IsSuccess)
        {
            deviceVariableSourceRead.DeviceVariables.ForEach(it => it.Quality = 0);
        }
        return ReadWriteHelpers.DealWithReadResult(read, content =>
        ReadWriteHelpers.PraseStructContent(content, deviceVariableSourceRead.DeviceVariables,
            ThingsGatewayBitConverter));
    }

    /// <summary>
    /// 写入变量值
    /// </summary>
    /// <param name="deviceVariable">变量实体</param>
    /// <param name="value">变量写入值</param>
    /// <returns></returns>
    public abstract Task<OperResult> WriteValueByNameAsync(DeviceVariable deviceVariable, string value);

    /// <summary>
    /// 返回全部内容字节数组
    /// <br></br>
    /// 通常使用<see cref="IReadWrite.ReadAsync(string, ushort)"/>可以直接返回正确信息
    /// </summary>
    /// <param name="address">变量地址</param>
    /// <param name="length">读取长度</param>
    /// <returns></returns>
    protected abstract Task<OperResult<byte[]>> ReadAsync(string address, ushort length);

    private void Log_Out(LogType arg1, object arg2, string arg3, Exception arg4)
    {
        switch (arg1)
        {
            case LogType.None:
                _logger?.Log(LogLevel.None, 0, arg4, arg3);
                break;
            case LogType.Trace:
                _logger?.Log(LogLevel.Trace, 0, arg4, arg3);
                break;
            case LogType.Debug:
                _logger?.Log(LogLevel.Debug, 0, arg4, arg3);
                break;
            case LogType.Info:
                _logger?.Log(LogLevel.Information, 0, arg4, arg3);
                break;
            case LogType.Warning:
                privateLogger?.Log(LogLevel.Warning, 0, arg4, arg3);
                break;
            case LogType.Error:
                privateLogger?.Log(LogLevel.Error, 0, arg4, arg3);
                break;
            case LogType.Critical:
                privateLogger?.Log(LogLevel.Critical, 0, arg4, arg3);
                break;
            default:
                break;
        }
    }

}
