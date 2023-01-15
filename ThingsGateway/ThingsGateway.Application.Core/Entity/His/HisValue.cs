using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ThingsGateway.Application.Core;
/// <summary>
/// 历史数据表
/// </summary>
[Description("历史报警表")]
public class HisValue
{
    [TimeDbSplitField(DateType.Month)]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CollectTime { get; set; }

    [SugarColumn(ColumnDataType = "symbol")]
    public string Name { get; set; }

    public int Quality { get; set; }

    public double Value { get; set; }
}
