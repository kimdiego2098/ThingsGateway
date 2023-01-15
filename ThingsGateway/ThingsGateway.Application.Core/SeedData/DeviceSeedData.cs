

namespace ThingsGateway.Application.Core;


/// <summary>
/// 实体种子数据
/// </summary>
public class DeviceSeedData : ISqlSugarEntitySeedData<Device>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<Device> HasData()
    {
        List<Device> devices = new List<Device>();
        //devices.Add(new Device
        //{
        //    Id = 1,
        //    Description = "测试ModbusRtuOverTcp设备",
        //    DeviceGroupId = 1,

        //    DriverId = 1,
        //    DriverAssembleName = "ModbusRtuOverTcp",
        //    InvokeEnable = true,
        //    Name = "dev1",

        //});
        //devices.Add(new Device
        //{
        //    Id = 2,
        //    Description = "测试ModbusRtuOverTcp设备1",
        //    DeviceGroupId = 1,

        //    DriverId = 1,
        //    DriverAssembleName = "ModbusRtuOverTcp",
        //    InvokeEnable = true,
        //    Name = "dev2",

        //});
        return devices;
    }
}