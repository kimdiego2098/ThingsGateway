using Newtonsoft.Json.Linq;

namespace ThingsGateway.Core;

/// <summary>
/// 序列化时long转string（防止js精度溢出）
/// </summary>
public class LongJsonConverter : JsonConverter<long>
{
    public override void WriteJson(JsonWriter writer, long value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.ToString());
    }

    public override long ReadJson(JsonReader reader, Type objectType, long existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JToken jt = JValue.ReadFrom(reader);
        return string.IsNullOrWhiteSpace(jt.Value<string>()) ? 0 : jt.Value<long>();
    }
}