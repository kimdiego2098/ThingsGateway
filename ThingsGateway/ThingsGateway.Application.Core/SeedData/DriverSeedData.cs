

namespace ThingsGateway.Application.Core;


/// <summary>
/// 实体种子数据
/// </summary>
public class DriverSeedData : ISqlSugarEntitySeedData<DriverPlugin>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<DriverPlugin> HasData()
    {
        yield return new DriverPlugin
        {
            Id = 1,
            PluginName = "Modbus",
            FileName = "Drivers/ThingsGateway.Modbus.dll",
        };
        yield return new DriverPlugin
        {
            Id = 2,
            PluginName = "Siemens",
            FileName = "Drivers/ThingsGateway.Siemens.dll",
        };
        yield return new DriverPlugin
        {
            Id = 3,
            PluginName = "OPCDAClient",
            FileName = "Drivers/ThingsGateway.OPCDAClient.dll",
        };
        yield return new DriverPlugin
        {
            Id = 4,
            PluginName = "AllenBradleyCipNet",
            FileName = "Drivers/ThingsGateway.AllenBradley.dll",
        };
    }
}