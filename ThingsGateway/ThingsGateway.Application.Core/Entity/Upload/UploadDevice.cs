namespace ThingsGateway.Application.Core;
/// <summary>
/// 设备通用表
/// </summary>
[SugarTable("upload_device")]
[Description("设备通用表")]
[Tenant(ApplicationConst.ConfigId)]
[SugarIndex("unique_upload_device_name", nameof(Name), OrderByType.Desc, true)]
[SystemTable]
public class UploadDevice : EntityBase
{

    #region SQL字段
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
    [Navigate(NavigateType.OneToMany, nameof(UploadDeviceProperty.UploadDeviceId))]
    [SugarColumn(IsIgnore = true)]
    public List<UploadDeviceProperty> DeviceProperties { get; set; }
    /// <summary>
    /// 设备属性数量
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public int UploadDevicePropertieNum { get => DeviceProperties == null ? 0 : DeviceProperties.Count; }

    [SugarColumn(IsIgnore = true)]
    public UploadDeviceStatus UploadDeviceStatus { get; set; } = new();
    #endregion
}


