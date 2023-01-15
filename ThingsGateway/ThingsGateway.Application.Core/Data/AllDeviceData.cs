namespace ThingsGateway.Application.Core;
/// <summary>
/// 采集设备值与状态全局提供
/// </summary>
public class AllDeviceData : ISingleton
{
    public AllDeviceData()
    {
    }
    /// <summary>
    /// 全局设备对象
    /// </summary>
    public ChangeIntelligentConcurrentList<Device> Devices { get; set; } = new();
    /// <summary>
    /// 全局设备变量对象
    /// </summary>
    public List<DeviceVariable> DeviceVariables
    {
        get
        {
            return Devices?.SelectMany(it => it.DeviceVariables).ToList();
        }
    }
    /// <summary>
    /// 全局内存变量对象
    /// </summary>
    public ConcurrentList<MemoryVariable> MemoryVariables
    {
        get;
        set;
    }
    /// <summary>
    /// 全局设备状态对象
    /// </summary>
    public ConcurrentList<DeviceStatus> DeviceStatuses
    {
        get
        {
            return Devices?.Adapt<ConcurrentList<DeviceStatus>>();
        }
    }
}
