using CodingSeb.ExpressionEvaluator;

using Furion.Logging.Extensions;

using Masuit.Tools;

using System.Text.Json.Serialization;

namespace ThingsGateway.Application.Core;
/// <summary>
/// 设备变量表
/// </summary>
[SugarTable("device_variable")]
[Description("设备变量表")]
[Tenant(ApplicationConst.ConfigId)]
[SugarIndex("unique_variable_name", nameof(Name), OrderByType.Desc, true)]
[SystemTable]
public class DeviceVariable : MemoryVariable, IDeviceVariable
{

    #region SQL字段
    /// <summary>
    /// 设备名称
    /// </summary>
    [SugarColumn(ColumnDescription = "设备Id")]
    public long DeviceId { get; set; }

    /// <summary>
    /// 变量值表达式
    /// </summary>
    [SugarColumn(ColumnDescription = "表达式")]
    public string? Expressions { get; set; }

    /// <summary>
    /// 其他方法，若不为空，此时Address为方法参数
    /// </summary>
    [SugarColumn(ColumnDescription = "其他方法")]
    public string? OtherMethod { get; set; }

    /// <summary>
    /// 变量地址，可能带有额外的信息，比如<see cref="DataFormat"/> ，以;分割
    /// </summary>
    [SugarColumn(ColumnDescription = "变量地址")]
    public string? VariableAddress { get; set; }

    /// <summary>
    /// 执行间隔，小于等于0时使用<see cref="Device.InvokeInterval"/>
    /// </summary>
    [SugarColumn(ColumnDescription = "执行间隔")]
    public int InvokeInterval { get; set; }

    #endregion Public Properties
    /// <summary>
    /// 设备
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(DeviceId))]
    public Device Device { get; set; }
    /// <summary>
    /// 设备名称，仅用于excel同步
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    [SugarColumn(IsIgnore = true)]
    public string DeviceName { get; set; }

    private void Evaluator_PreEvaluateVariable(object sender, VariablePreEvaluationEventArg e)
    {
        var data = App.GetService<AllDeviceData>();
        var obj = data.DeviceVariables.FirstOrDefault(it => it.Name == e.Name);
        if (obj == null)
        {
            return;
        }
        if (obj.Value != null)
            e.Value = Convert.ChangeType(obj.Value, obj.DataType);
        else
            e.Value = null;
    }

    public DeviceVariable()
    {

    }
    private ExpressionEvaluator expressionEvaluator;
    [SugarColumn(IsIgnore = true)]
    public override object Value
    {
        get
        {
            return base.Value;
        }
        set
        {
            if (value != null) { Quality = 192; }
            if (!Expressions.IsNullOrEmpty() && expressionEvaluator == null)
            {
                expressionEvaluator = new();
                expressionEvaluator.PreEvaluateVariable += Evaluator_PreEvaluateVariable;

            }
            RawValue = value;
            if (!Expressions.IsNullOrEmpty() && value != null)
            {
                object data = null;
                try
                {
                    data = expressionEvaluator.GetExpressionsResult(Expressions, RawValue);
                }
                catch (Exception ex)
                {
                    (Name + "转换表达式失败：" + ex.Message).LogError();
                }
                if (data?.ToString() != base.Value?.ToString())
                {
                    base.Value = data;
                    CollectTime = DateTime.Now;
                    ChangeTime = DateTime.Now;
                }
                else
                {
                    base.Value = data;
                    CollectTime = DateTime.Now;
                }

            }
            else
            {
                if (value?.ToString() != base.Value?.ToString())
                {
                    base.Value = RawValue;
                    CollectTime = DateTime.Now;
                    ChangeTime = DateTime.Now;
                }
                else
                {
                    base.Value = RawValue;
                    CollectTime = DateTime.Now;
                }
            }


        }
    }

    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    [SugarColumn(IsIgnore = true)]
    public VariableCahngeEventHandler VariableCollectChange { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    [SugarColumn(IsIgnore = true)]
    public VariableCahngeEventHandler VariableCollectIntelnalChange { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    [SugarColumn(IsIgnore = true)]
    public VariableCahngeEventHandler VariableValueChange { get; set; }

    private DateTime changeTime;
    [SugarColumn(IsIgnore = true)]
    public override DateTime ChangeTime
    {
        get
        {
            return changeTime;
        }

        set
        {
            changeTime = value;
            if (DeviceName.IsNullOrEmpty()) DeviceName = Device?.Name;
            //Task.Run(() =>
            VariableValueChange?.Invoke(this)
                //)
                ;
        }
    }

    private DateTime collectTime;
    [SugarColumn(IsIgnore = true)]
    public DateTime CollectTime
    {
        get
        {
            return collectTime;
        }

        set
        {
            collectTime = value;
            if (DeviceName.IsNullOrEmpty()) DeviceName = Device?.Name;
            //Task.Run(() =>
            VariableCollectIntelnalChange?.Invoke(this)
            //)
                ;
            VariableCollectChange?.Invoke(this.DeepClone());
        }
    }
    [SugarColumn(IsIgnore = true)]
    public int Quality { get; set; }

    /// <summary>
    /// <see cref="DriverBase.ReadAsync(string, ushort)"/>返回字节组中的索引位置
    /// <br></br>
    /// 这个参数值由自动分包方法写入<see cref="DriverBase.LoadSourceRead(List{DeviceVariable})"/>
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public int Index { get; set; }
    /// <summary>
    /// 变量在属于字符串类型时的字符串长度
    /// <br></br>
    /// 这个参数值由自动分包方法写入<see cref="DriverBase.LoadSourceRead(List{DeviceVariable})"/>
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public int StringLength { get; set; }
    /// <summary>
    /// 变量在属于字符串类型时的Encoding类型
    /// <br></br>
    /// 这个参数值由自动分包方法写入<see cref="DriverBase.LoadSourceRead(List{DeviceVariable})"/>
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    [Mapster.AdaptIgnore]
    public Encoding StringEncoding { get; set; }
    /// <summary>
    /// 变量在属于字符串类型时的BCD类型
    /// <br></br>
    /// 这个参数值由自动分包方法写入<see cref="DriverBase.LoadSourceRead(List{DeviceVariable})"/>
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public BCDFormat StringBCDFormat { get; set; }
    /// <summary>
    /// 变量在属于字符串类型时的BCD类型
    /// <br></br>
    /// 这个参数值由自动分包方法写入<see cref="DriverBase.LoadSourceRead(List{DeviceVariable})"/>
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    public IThingsGatewayBitConverter ThingsGatewayBitConverter { get; set; }

    /// <summary>
    /// 分配资源
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    public object ObjectTag { get; set; }

}

/// <summary>
/// 变量触发变化
/// </summary>
public delegate void VariableCahngeEventHandler(DeviceVariable variable);


/// <summary>
/// 设备变量表，用于mqtt推送
/// </summary>
public class MqttDeviceVariable
{
    public long Id { get; set; }
    public string Name { get; set; }
    public object Value { get; set; }
    public object RawValue { get; set; }
    public DateTime CollectTime { get; set; }
    public DateTime ChangeTime { get; set; }
    public int Quality { get; set; }
}
