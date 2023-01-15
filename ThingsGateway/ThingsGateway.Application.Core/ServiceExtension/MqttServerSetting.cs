namespace ThingsGateway.Application.Core;
/// <summary>
/// MqttServer配置实体类
/// </summary>
public class MqttServerSettings : IConfigurableOptions
{
    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// 是否开启Tls
    /// </summary>
    public bool EnableTls { get; set; }
    /// <summary>
    /// Tls端口
    /// </summary>
    public int TlsPort { get; set; }

}

