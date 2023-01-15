
namespace ThingsGateway.Application.Core;

/// <summary>
/// 实体种子数据
/// </summary>
public class UploadSeedData : ISqlSugarEntitySeedData<UploadPlugin>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<UploadPlugin> HasData()
    {
        yield return new UploadPlugin
        {
            Id = 1,
            PluginName = "中石化智能化油库数据采集(盈科)",
            FileName = "Uploads/ThingsGateway.Yingke.dll",
        };
        yield return new UploadPlugin
        {
            Id = 2,
            PluginName = "中石化智能化油库双防平台数据采集(双防双控)",
            FileName = "Uploads/ThingsGateway.SFSK.dll",
        };
        yield return new UploadPlugin
        {
            Id = 3,
            PluginName = "默认Mqtt",
            FileName = "Uploads/ThingsGateway.DefaultMqttUp.dll",
        };
        yield return new UploadPlugin
        {
            Id = 4,
            PluginName = "IotSharp",
            FileName = "Uploads/ThingsGateway.IotSharp.dll",
        };
        yield return new UploadPlugin
        {
            Id = 5,
            PluginName = "OPCUAServer",
            FileName = "Uploads/ThingsGateway.OPCUAServer.dll",
        };
    }
}