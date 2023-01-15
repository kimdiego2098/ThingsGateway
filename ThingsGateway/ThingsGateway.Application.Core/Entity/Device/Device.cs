using System.Text.Json.Serialization;

namespace ThingsGateway.Application.Core;
/// <summary>
/// 设备通用表
/// </summary>
[SugarTable("device")]
[Description("设备通用表")]
[Tenant(ApplicationConst.ConfigId)]
[SugarIndex("unique_device_name", nameof(Name), OrderByType.Desc, true)]
[SystemTable]
public class Device : EntityBase
{

    #region SQL字段
    /// <summary>
    /// 变化上传
    /// </summary>
    [SugarColumn(ColumnDescription = "变化上传")]
    public bool ChangeUpload { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(50)]
    [SugarColumn(ColumnDescription = "描述", IsNullable = true)]
    public string? Description { get; set; }



    /// <summary>
    /// 驱动子程序集名称
    /// </summary>
    [SugarColumn(ColumnDescription = "驱动子程序集名称")]
    public string DriverAssembleName { get; set; }

    /// <summary>
    /// 设备使能
    /// </summary>
    [SugarColumn(ColumnDescription = "设备使能")]
    public bool InvokeEnable { get; set; }

    /// <summary>
    /// 默认执行间隔
    /// </summary>
    [SugarColumn(ColumnDescription = "默认执行间隔")]
    public int InvokeInterval { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [MaxLength(50)]
    [SugarColumn(ColumnDescription = "名称")]
    public string Name { get; set; }
    #endregion Public Properties

    #region 其他属性

    /// <summary>
    /// 设备属性
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(DeviceProperty.DeviceId))]
    [SugarColumn(IsIgnore = true)]
    public List<DeviceProperty> DeviceProperties { get; set; }
    /// <summary>
    /// 设备属性数量
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public int DevicePropertieNum { get => DeviceProperties == null ? 0 : DeviceProperties.Count; }
    /// <summary>
    /// 设备变量
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(DeviceVariable.DeviceId), nameof(Id))]
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [SugarColumn(IsIgnore = true)]
    public List<DeviceVariable> DeviceVariables { get; set; }
    /// <summary>
    /// 设备变量数量
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public int DeviceVariableNum { get => DeviceVariables == null ? 0 : DeviceVariables.Count; }

    [SugarColumn(IsIgnore = true)]
    public DeviceStatus DeviceStatus { get; set; } = new();

    #endregion
}


