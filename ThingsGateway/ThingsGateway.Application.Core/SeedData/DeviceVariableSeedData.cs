


namespace ThingsGateway.Application.Core;


/// <summary>
/// 实体种子数据
/// </summary>
public class DeviceVariableSeedData : ISqlSugarEntitySeedData<DeviceVariable>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<DeviceVariable> HasData()
    {
        List<DeviceVariable> data = new List<DeviceVariable>();
        //data.Add(new DeviceVariable
        //{
        //    Id = 1,
        //    Description = "测试ModbusRtuOverTcp设备",
        //    DeviceName = "qweqwe",
        //    Name = "dev1",
        //    CoreDataType = CoreDataType.String,
        //    InitialValue = "1234",
        //    ProtectType = ProtectTypeEnum.ReadWrite,
        //    InvokeInterval = 1000,
        //    VariableAddress = "1;text=Encoding.ASCII;len=4",
        //});
        //data.Add(new DeviceVariable
        //{
        //    Id = 2,
        //    Description = "测试ModbusRtuOverTcp设备1",
        //    DeviceName = "qweqwe",
        //    Name = "dev2",
        //    CoreDataType = CoreDataType.String,
        //    InitialValue = "1234",
        //    ProtectType = ProtectTypeEnum.ReadWrite,
        //    InvokeInterval = 1000,
        //    VariableAddress = "11;text=Encoding.UTF8;len=18",
        //});
        return data;
    }
}