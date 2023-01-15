using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;

namespace ThingsGateway.Application.Core;
/// <summary>
/// 设备变量表
/// </summary>

[ExcelExporter(Name = "设备变量表")]
public class DeviceVariableExcel
{
    /// <summary>
    /// 变量名称
    /// </summary>
    [ImporterHeader(Name = "变量名称")]
    [ExporterHeader(DisplayName = "变量名称")]
    public string Name { get; set; }


    /// <summary>
    /// 描述
    /// </summary>
    [ImporterHeader(Name = "描述")]
    [ExporterHeader(DisplayName = "描述")]
    public string Description { get; set; }

    /// <summary>
    /// 设备Id
    /// </summary>
    [ImporterHeader(IsIgnore = true)]
    [ExporterHeader(IsIgnore = true)]
    [Magicodes.ExporterAndImporter.Core.IEIgnore]
    public long DeviceId { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    [ImporterHeader(Name = "设备名称")]
    [ExporterHeader(DisplayName = "设备名称")]
    public string DeviceName { get; set; }


    /// <summary>
    /// 变量地址，可能带有额外的信息，比如<see cref="DataFormat"/> ，以;分割
    /// </summary>
    [ImporterHeader(Name = "变量地址")]
    [ExporterHeader(DisplayName = "变量地址")]
    public string VariableAddress { get; set; }

    /// <summary>
    /// 其他方法，若不为空，此时Address为方法参数
    /// </summary>
    [ImporterHeader(Name = "其他方法")]
    [ExporterHeader(DisplayName = "其他方法")]
    public string OtherMethod { get; set; }


    /// <summary>
    /// 初始值
    /// </summary>
    [ImporterHeader(Name = "初始值")]
    [ExporterHeader(DisplayName = "初始值")]
    public string InitialValue { get; set; }

    /// <summary>
    /// 读写权限
    /// </summary>
    [ImporterHeader(Name = "读写权限")]
    [ExporterHeader(DisplayName = "读写权限")]
    public ProtectTypeEnum ProtectType { get; set; }


    /// <summary>
    /// 数据类型
    /// </summary>
    [ImporterHeader(Name = "数据类型")]
    [ExporterHeader(DisplayName = "数据类型")]
    public CoreDataType CoreDataType { get; set; }


    /// <summary>
    /// 变量值表达式
    /// </summary>
    [ImporterHeader(Name = "表达式")]
    [ExporterHeader(DisplayName = "表达式")]
    public string Expressions { get; set; }



    /// <summary>
    /// 执行间隔，小于等于0时使用<see cref="Device.InvokeInterval"/>
    /// </summary>
    [ImporterHeader(Name = "执行间隔")]
    [ExporterHeader(DisplayName = "执行间隔")]
    public int InvokeInterval { get; set; }





}

/// <summary>
/// 设备变量报警表
/// </summary>

[ExcelExporter(Name = "设备变量报警表")]
public class DeviceVariableAlarmExcel
{

    /// <summary>
    /// 变量Id
    /// </summary>
    [ImporterHeader(IsIgnore = true)]
    [ExporterHeader(IsIgnore = true)]
    [Magicodes.ExporterAndImporter.Core.IEIgnore]
    public long VariableId { get; set; }

    /// <summary>
    /// 变量名称
    /// </summary>
    [ImporterHeader(Name = "变量名称")]
    [ExporterHeader(DisplayName = "变量名称")]
    public string VariableName { get; set; }

    /// <summary>
    /// 报警组
    /// </summary>
    [ImporterHeader(Name = "报警组")]
    [ExporterHeader(DisplayName = "报警组")]
    public string AlarmGroup { get; set; } = "";
    /// <summary>
    /// 报警死区
    /// </summary>
    [ImporterHeader(Name = "报警死区")]
    [ExporterHeader(DisplayName = "报警死区")]
    public int AlarmDeadZone { get; set; }
    /// <summary>
    /// 报警延时
    /// </summary>
    [ImporterHeader(Name = "报警延时")]
    [ExporterHeader(DisplayName = "报警延时")]
    public int AlarmDelayTime { get; set; }
    /// <summary>
    /// 布尔开报警使能
    /// </summary>
    [ImporterHeader(Name = "布尔开报警使能")]
    [ExporterHeader(DisplayName = "布尔开报警使能")]
    public bool BoolOpenAlarmEnable { get; set; }
    /// <summary>
    /// 布尔开报警约束
    /// </summary>
    [ImporterHeader(Name = "布尔开报警约束")]
    [ExporterHeader(DisplayName = "布尔开报警约束")]
    public string BoolOpenRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 布尔开报警文本
    /// </summary>
    [ImporterHeader(Name = "布尔开报警文本")]
    [ExporterHeader(DisplayName = "布尔开报警文本")]
    public string BoolOpenAlarmText { get; set; } = "";


    /// <summary>
    /// 布尔关报警使能
    /// </summary>
    [ImporterHeader(Name = "布尔关报警使能")]
    [ExporterHeader(DisplayName = "布尔关报警使能")]
    public bool BoolCloseAlarmEnable { get; set; }
    /// <summary>
    /// 布尔关报警约束
    /// </summary>
    [ImporterHeader(Name = "布尔关报警约束")]
    [ExporterHeader(DisplayName = "布尔关报警约束")]
    public string BoolCloseRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 布尔关报警文本
    /// </summary>
    [ImporterHeader(Name = "布尔关报警文本")]
    [ExporterHeader(DisplayName = "布尔关报警文本")]
    public string BoolCloseAlarmText { get; set; } = "";

    /// <summary>
    /// 高报使能
    /// </summary>
    [ImporterHeader(Name = "高报使能")]
    [ExporterHeader(DisplayName = "高报使能")]
    public bool HAlarmEnable { get; set; }
    /// <summary>
    /// 高报约束
    /// </summary>
    [ImporterHeader(Name = "高报约束")]
    [ExporterHeader(DisplayName = "高报约束")]
    public string HRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 高报文本
    /// </summary>
    [ImporterHeader(Name = "高报文本")]
    [ExporterHeader(DisplayName = "高报文本")]
    public string HAlarmText { get; set; } = "";
    /// <summary>
    /// 高限值
    /// </summary>
    [ImporterHeader(Name = "高限值")]
    [ExporterHeader(DisplayName = "高限值")]
    public string HAlarmCode { get; set; } = "";


    /// <summary>
    /// 高高报使能
    /// </summary>
    [ImporterHeader(Name = "高高报使能")]
    [ExporterHeader(DisplayName = "高高报使能")]
    public bool HHAlarmEnable { get; set; }
    /// <summary>
    /// 高高报约束
    /// </summary>
    [ImporterHeader(Name = "高高报约束")]
    [ExporterHeader(DisplayName = "高高报约束")]
    public string HHRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 高高报文本
    /// </summary>
    [ImporterHeader(Name = "高高报文本")]
    [ExporterHeader(DisplayName = "高高报文本")]
    public string HHAlarmText { get; set; } = "";
    /// <summary>
    /// 高高限值
    /// </summary>
    [ImporterHeader(Name = "高高限值")]
    [ExporterHeader(DisplayName = "高高限值")]
    public string HHAlarmCode { get; set; } = "";

    /// <summary>
    /// 低报使能
    /// </summary>
    [ImporterHeader(Name = "低报使能")]
    [ExporterHeader(DisplayName = "低报使能")]
    public bool LAlarmEnable { get; set; }
    /// <summary>
    /// 低报约束
    /// </summary>
    [ImporterHeader(Name = "低报约束")]
    [ExporterHeader(DisplayName = "低报约束")]
    public string LRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 低报文本
    /// </summary>
    [ImporterHeader(Name = "低报文本")]
    [ExporterHeader(DisplayName = "低报文本")]
    public string LAlarmText { get; set; } = "";
    /// <summary>
    /// 低限值
    /// </summary>
    [ImporterHeader(Name = "低限值")]
    [ExporterHeader(DisplayName = "低限值")]
    public string LAlarmCode { get; set; } = "";

    /// <summary>
    /// 低低报使能
    /// </summary>
    [ImporterHeader(Name = "低低报使能")]
    [ExporterHeader(DisplayName = "低低报使能")]
    public bool LLAlarmEnable { get; set; }
    /// <summary>
    /// 低低报约束
    /// </summary>
    [ImporterHeader(Name = "低低报约束")]
    [ExporterHeader(DisplayName = "低低报约束")]
    public string LLRestrainExpressions { get; set; } = "";
    /// <summary>
    /// 低低报文本
    /// </summary>
    [ImporterHeader(Name = "低低报文本")]
    [ExporterHeader(DisplayName = "低低报文本")]
    public string LLAlarmText { get; set; } = "";
    /// <summary>
    /// 低低限值
    /// </summary>
    [ImporterHeader(Name = "低低限值")]
    [ExporterHeader(DisplayName = "低低限值")]
    public string LLAlarmCode { get; set; } = "";



}

/// <summary>
/// 设备变量历史表
/// </summary>

[ExcelExporter(Name = "设备变量历史表")]
public class DeviceVariableHisExcel
{
    /// <summary>
    /// 变量Id
    /// </summary>
    [ImporterHeader(IsIgnore = true)]
    [ExporterHeader(IsIgnore = true)]
    [Magicodes.ExporterAndImporter.Core.IEIgnore]
    public long VariableId { get; set; }

    /// <summary>
    /// 变量名称
    /// </summary>
    [ImporterHeader(Name = "变量名称")]
    [ExporterHeader(DisplayName = "变量名称")]
    public string VariableName { get; set; }

    /// <summary>
    /// 存储类型
    /// </summary>
    [ImporterHeader(Name = "存储类型")]
    [ExporterHeader(DisplayName = "存储类型")]
    public HisType HisType { get; set; }

    /// <summary>
    /// 启用
    /// </summary>
    [ImporterHeader(Name = "启用")]
    [ExporterHeader(DisplayName = "启用")]
    public bool Enable { get; set; }

}

/// <summary>
/// 设备变量历史表
/// </summary>

[ExcelExporter(Name = "设备变量扩展域表")]
public class DeviceVariablePropertyExcel
{
    /// <summary>
    /// 变量Id
    /// </summary>
    [ImporterHeader(IsIgnore = true)]
    [ExporterHeader(IsIgnore = true)]
    [Magicodes.ExporterAndImporter.Core.IEIgnore]
    public long VariableId { get; set; }

    /// <summary>
    /// 变量名称
    /// </summary>
    [ImporterHeader(Name = "变量名称")]
    [ExporterHeader(DisplayName = "变量名称")]
    public string VariableName { get; set; }

    public string? ExtendFieldString1 { get; set; }
    public string? ExtendFieldString2 { get; set; }
    public string? ExtendFieldString3 { get; set; }
    public string? ExtendFieldString4 { get; set; }
    public string? ExtendFieldString5 { get; set; }
    public string? ExtendFieldString6 { get; set; }
    public string? ExtendFieldString7 { get; set; }
    public string? ExtendFieldString8 { get; set; }
    public string? ExtendFieldString9 { get; set; }
    public string? ExtendFieldString10 { get; set; }

    public string? ExtendFieldString11 { get; set; }
    public string? ExtendFieldString12 { get; set; }
    public string? ExtendFieldString13 { get; set; }
    public string? ExtendFieldString14 { get; set; }
    public string? ExtendFieldString15 { get; set; }
    public string? ExtendFieldString16 { get; set; }
    public string? ExtendFieldString17 { get; set; }
    public string? ExtendFieldString18 { get; set; }
    public string? ExtendFieldString19 { get; set; }
    public string? ExtendFieldString20 { get; set; }

    public string? ExtendFieldString21 { get; set; }
    public string? ExtendFieldString22 { get; set; }
    public string? ExtendFieldString23 { get; set; }
    public string? ExtendFieldString24 { get; set; }
    public string? ExtendFieldString25 { get; set; }
    public string? ExtendFieldString26 { get; set; }
    public string? ExtendFieldString27 { get; set; }
    public string? ExtendFieldString28 { get; set; }
    public string? ExtendFieldString29 { get; set; }
    public string? ExtendFieldString30 { get; set; }

}



public class ImportDeviceVariableExcelExcel
{
    [ExcelImporter(SheetName = "设备变量表")]
    public DeviceVariableExcel DeviceVariableExcel { get; set; }
    [ExcelImporter(SheetName = "设备变量报警表")]
    public DeviceVariableAlarmExcel DeviceVariableAlarmExcel { get; set; }
    [ExcelImporter(SheetName = "设备变量历史表")]
    public DeviceVariableHisExcel DeviceVariableHisExcel { get; set; }
    [ExcelImporter(SheetName = "设备变量扩展域表")]
    public DeviceVariablePropertyExcel DeviceVariablePropertyExcel { get; set; }
}

