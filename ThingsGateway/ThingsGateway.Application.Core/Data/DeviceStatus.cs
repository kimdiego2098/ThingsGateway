using Newtonsoft.Json;


namespace ThingsGateway.Application.Core;



public class DeviceStatus
{
    /// <summary>
    /// 设备读取分包数量
    /// </summary>
    public int DeviceSourceVariableNum { get; set; }
    /// <summary>
    /// 设备分包读取成功数量
    /// </summary>
    public int DeviceSourceVariableSuccessNum { get; set; }
    /// <summary>
    /// 设备分包读取失败数量
    /// </summary>
    public int DeviceSourceVariableFailedNum { get; set; }
    /// <summary>
    /// 设备特殊方法数量
    /// </summary>
    public int DeviceMedsVariableNum { get; set; }
    /// <summary>
    /// 设备特殊方法成功数量
    /// </summary>
    public int DeviceMedsVariableSuccessNum { get; set; }
    /// <summary>
    /// 设备特殊方法失败数量
    /// </summary>
    public int DeviceMedsVariableFailedNum { get; set; }
    /// <summary>
    /// 设备活跃时间
    /// </summary>
    public DateTime ActiveTime { get; set; }
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Device Device { get; set; }
    /// <summary>
    /// 设备状态
    /// </summary>
    public DeviceOnLineStatusEnum DeviceOnLineStatus
    {
        get
        {
            return deviceOnLineStatus;
        }
        set
        {
            if (deviceOnLineStatus != value)
            {
                deviceOnLineStatus = value;
                DeviceStatusCahnge?.Invoke(Device);
            }
        }
    }
    private DeviceOnLineStatusEnum deviceOnLineStatus = DeviceOnLineStatusEnum.Default;

    public event DeviceCahngeEventHandler DeviceStatusCahnge;
    private string deviceOffMsg;
    public string DeviceOffMsg
    {
        get
        {
            if (DeviceOnLineStatus == DeviceOnLineStatusEnum.OnLine)
            {
                return "";
            }
            else
                return deviceOffMsg;
        }

        set => deviceOffMsg = value;
    }

}


public class UploadDeviceStatus
{
    /// <summary>
    /// 设备活跃时间
    /// </summary>
    public DateTime ActiveTime { get; set; }
    /// <summary>
    /// 设备状态
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public DeviceOnLineStatusEnum DeviceOnLineStatus { get; set; } = DeviceOnLineStatusEnum.Default;
}



public delegate void DelegateOnAlarmChanged(DeviceVariable alarm);
public delegate void DelegateOnDeviceStatusChanged(Device alarm);


/// <summary>
/// 设备触发变化
/// </summary>
public delegate void DeviceCahngeEventHandler(Device variable);