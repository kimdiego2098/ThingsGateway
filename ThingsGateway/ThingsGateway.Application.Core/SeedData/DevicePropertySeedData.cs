

namespace ThingsGateway.Application.Core;


/// <summary>
/// 实体种子数据
/// </summary>
public class DevicePropertySeedData : ISqlSugarEntitySeedData<DeviceProperty>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<DeviceProperty> HasData()
    {
        var lists = new List<DeviceProperty>();
        //lists.Add(new DeviceProperty()
        //{
        //    Id = 1,
        //    Description = "IP",
        //    DevicePropertyName = "IP",
        //    DeviceId = 1,
        //    Value = "127.0.0.1",
        //});
        //lists.Add(new DeviceProperty()
        //{
        //    Id = 2,
        //    DevicePropertyName = "Port",
        //    Description = "端口",
        //    DeviceId = 1,
        //    Value = "502",
        //});
        //lists.Add(new DeviceProperty()
        //{
        //    Id = 3,
        //    DevicePropertyName = "TimeOut",
        //    Description = "超时时间",
        //    DeviceId = 1,
        //    Value = "2000",
        //});
        //lists.Add(new DeviceProperty()
        //{
        //    Id = 4,
        //    DevicePropertyName = "ConnectTimeOut",
        //    Description = "连接超时时间",
        //    DeviceId = 1,
        //    Value = "2000",
        //});
        //lists.Add(new DeviceProperty()
        //{
        //    Id = 5,
        //    DevicePropertyName = "DataFormat",
        //    Description = "DataFormat",
        //    DeviceId = 1,
        //    Value = "ABCD",
        //});
        //lists.Add(new DeviceProperty()
        //{
        //    Id = 6,
        //    DevicePropertyName = "Station",
        //    Description = "Station",
        //    DeviceId = 1,
        //    Value = "1",
        //});
        //lists.Add(new DeviceProperty()
        //{
        //    Id = 7,
        //    DevicePropertyName = "MaxPack",
        //    Description = "MaxPack",
        //    DeviceId = 1,
        //    Value = "20",
        //});

        return lists;
    }
}