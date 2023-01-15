namespace ThingsGateway.Application.Core;
/// <summary>
/// 历史报警表
/// </summary>
[SugarTable("alarm_his")]
[Description("历史报警表")]
public class AlarmHis : EntityBaseId
{

    [SugarColumn(ColumnDescription = "变量地址")]
    public string VariableAddress { get; set; }

    [SugarColumn(ColumnDescription = "采集时间")]
    public DateTime CollectTime { get; set; }

    [SugarColumn(ColumnDescription = "质量戳")]
    public int Quality { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    [SugarColumn(ColumnDescription = "数据类型")]
    public CoreDataType CoreDataType { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(ColumnDescription = "描述")]
    public string Description { get; set; }
    /// <summary>
    /// 变量名称
    /// </summary>
    [SugarColumn(ColumnDescription = "变量名称")]
    public string Name { get; set; }
    /// <summary>
    /// 设备名称
    /// </summary>
    [SugarColumn(ColumnDescription = "设备名称")]
    public string DeviceName { get; set; }

    public string Value { get; set; }
    public DateTime ChangeTime { get; set; }

    #region 运行属性
    /// <summary>
    /// 报警时间
    /// </summary>
    public DateTime VariableAlarmsAlarmTime { get; set; }
    /// <summary>
    /// 事件时间
    /// </summary>
    public DateTime VariableAlarmsEventTime { get; set; }
    /// <summary>
    /// 报警类型
    /// </summary>
    public AlarmEnum VariableAlarmsAlarmType { get; set; }

    /// <summary>
    /// 事件类型
    /// </summary>
    public EventEnum VariableAlarmsEventType { get; set; }

    /// <summary>
    /// 报警值
    /// </summary>
    public string VariableAlarmsAlarmCode { get; set; }

    /// <summary>
    /// 报警限值
    /// </summary>
    public string VariableAlarmsAlarmLimit { get; set; }

    /// <summary>
    /// 报警文本
    /// </summary>
    public string VariableAlarmsAlarmText { get; set; }

    /// <summary>
    /// 报警源IP
    /// </summary>
    public string VariableAlarmsMachineIP { get; set; }

    #endregion
}
