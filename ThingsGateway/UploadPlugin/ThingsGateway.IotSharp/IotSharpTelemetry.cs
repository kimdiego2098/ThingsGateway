using Newtonsoft.Json;

namespace ThingsGateway.IotSharp
{
    public class RpcResponse
    {
        public string DeviceId { get; set; }
        public string Method { get; set; }
        public string ResponseId { get; set; }
        public string Data { get; set; }

    }

    public class IotSharpTelemetry
    {
        [JsonProperty(PropertyName = "ts")]
        public long TS { get; set; } = DateTime.Now.Ticks;
        [JsonProperty(PropertyName = "devicestatus")]
        public DeviceStatusEnums DeviceStatus { get; set; } = DeviceStatusEnums.Good;
        [JsonProperty(PropertyName = "values")]
        public Dictionary<string, object> Values { get; set; } = new();
    }
    public enum DeviceStatusEnums
    {
        Good,
        PartGood,
        Bad,
        UnKnow
    }
}
