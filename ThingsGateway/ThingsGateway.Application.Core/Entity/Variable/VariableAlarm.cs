using System.Text.Json.Serialization;

namespace ThingsGateway.Application.Core;
/// <summary>
/// 变量报警属性表
/// </summary>
[SugarTable("variable_alarm")]
[Description("变量报警属性表")]
[Tenant(ApplicationConst.ConfigId)]
[SystemTable]
public class VariableAlarm : EntityBaseId
{
    #region SQL字段
    /// <summary>
    /// 报警组
    /// </summary>
    [SugarColumn(ColumnDescription = "报警组")]
    public string? AlarmGroup { get; set; } = "";
    /// <summary>
    /// 报警死区
    /// </summary>
    [SugarColumn(ColumnDescription = "报警死区")]
    public int AlarmDeadZone { get; set; }
    /// <summary>
    /// 报警延时
    /// </summary>
    [SugarColumn(ColumnDescription = "报警延时")]
    public int AlarmDelayTime { get; set; }
    /// <summary>
    /// 布尔开报警使能
    /// </summary>
    [SugarColumn(ColumnDescription = "布尔开报警使能")]
    public bool BoolOpenAlarmEnable { get; set; }
    /// <summary>
    /// 布尔开报警约束
    /// </summary>
    [SugarColumn(ColumnDescription = "布尔开报警约束")]
    public string? BoolOpenRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 布尔开报警文本
    /// </summary>
    [SugarColumn(ColumnDescription = "布尔开报警文本")]
    public string? BoolOpenAlarmText { get; set; } = "";


    /// <summary>
    /// 布尔关报警使能
    /// </summary>
    [SugarColumn(ColumnDescription = "布尔关报警使能")]
    public bool BoolCloseAlarmEnable { get; set; }
    /// <summary>
    /// 布尔关报警约束
    /// </summary>
    [SugarColumn(ColumnDescription = "布尔关报警约束")]
    public string? BoolCloseRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 布尔关报警文本
    /// </summary>
    [SugarColumn(ColumnDescription = "布尔关报警文本")]
    public string? BoolCloseAlarmText { get; set; } = "";

    /// <summary>
    /// 高报使能
    /// </summary>
    [SugarColumn(ColumnDescription = "高报使能")]
    public bool HAlarmEnable { get; set; }
    /// <summary>
    /// 高报约束
    /// </summary>
    [SugarColumn(ColumnDescription = "高报约束")]
    public string? HRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 高报文本
    /// </summary>
    [SugarColumn(ColumnDescription = "高报文本")]
    public string? HAlarmText { get; set; } = "";
    /// <summary>
    /// 高限值
    /// </summary>
    [SugarColumn(ColumnDescription = "高限值")]
    public double HAlarmCode { get; set; }


    /// <summary>
    /// 高高报使能
    /// </summary>
    [SugarColumn(ColumnDescription = "高高报使能")]
    public bool HHAlarmEnable { get; set; }
    /// <summary>
    /// 高高报约束
    /// </summary>
    [SugarColumn(ColumnDescription = "高高报约束")]
    public string? HHRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 高高报文本
    /// </summary>
    [SugarColumn(ColumnDescription = "高高报文本")]
    public string? HHAlarmText { get; set; } = "";
    /// <summary>
    /// 高高限值
    /// </summary>
    [SugarColumn(ColumnDescription = "高高限值")]
    public double HHAlarmCode { get; set; }

    /// <summary>
    /// 低报使能
    /// </summary>
    [SugarColumn(ColumnDescription = "低报使能")]
    public bool LAlarmEnable { get; set; }
    /// <summary>
    /// 低报约束
    /// </summary>
    [SugarColumn(ColumnDescription = "低报约束")]
    public string? LRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 低报文本
    /// </summary>
    [SugarColumn(ColumnDescription = "低报文本")]
    public string? LAlarmText { get; set; } = "";
    /// <summary>
    /// 低限值
    /// </summary>
    [SugarColumn(ColumnDescription = "低限值")]
    public double LAlarmCode { get; set; }

    /// <summary>
    /// 低低报使能
    /// </summary>
    [SugarColumn(ColumnDescription = "低低报使能")]
    public bool LLAlarmEnable { get; set; }
    /// <summary>
    /// 低低报约束
    /// </summary>
    [SugarColumn(ColumnDescription = "低低报约束")]
    public string? LLRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 低低报文本
    /// </summary>
    [SugarColumn(ColumnDescription = "低低报文本")]
    public string? LLAlarmText { get; set; } = "";
    /// <summary>
    /// 低低限值
    /// </summary>
    [SugarColumn(ColumnDescription = "低低限值")]
    public double LLAlarmCode { get; set; }

    /// <summary>
    /// 变量Id
    /// </summary>
    [SugarColumn(ColumnDescription = "变量Id")]
    public long VariableId { get; set; }
    #endregion

    #region 其他属性
    /// <summary>
    /// 对应变量
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(VariableId), nameof(MemoryVariable.Id))]
    [SugarColumn(IsIgnore = true)]
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public MemoryVariable Variable { get; set; }

    [SugarColumn(IsIgnore = true)]
    public bool AlarmEnable
    {
        get
        {
            return LAlarmEnable || LLAlarmEnable || HAlarmEnable || HHAlarmEnable || BoolOpenAlarmEnable || BoolCloseAlarmEnable;
        }
    }

    #endregion

    #region 运行属性

    /// <summary>
    /// 报警时间
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public DateTime AlarmTime { get; set; }
    /// <summary>
    /// 事件时间
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public DateTime EventTime { get; set; }
    /// <summary>
    /// 报警类型
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public AlarmEnum AlarmType
    {
        get;
        set;
    }
    /// <summary>
    /// 事件类型
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public EventEnum EventType { get; set; }

    /// <summary>
    /// 报警值
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public object AlarmCode { get; set; }

    /// <summary>
    /// 报警限值
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public object AlarmLimit { get; set; }

    /// <summary>
    /// 报警文本
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string AlarmText { get; set; }

    /// <summary>
    /// 报警源IP
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string MachineIP { get; set; }

    #endregion
}
