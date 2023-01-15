using System.Text.Json.Serialization;

namespace ThingsGateway.Application.Core;

/// <summary>
/// 设备附加属性表
/// </summary>
[SugarTable("upload_device_property")]
[Description("上传设备附加属性表")]
[Tenant(ApplicationConst.ConfigId)]
[SystemTable]
public class UploadDeviceProperty : EntityBase
{
    #region SQL字段
    /// <summary>
    /// 属性名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称")]
    public string UploadDevicePropertyName { get; set; }
    /// <summary>
    /// 属性描述
    /// </summary>
    [SugarColumn(ColumnDescription = "描述", IsNullable = true)]
    public string? Description { get; set; }
    /// <summary>
    /// 属性值
    /// </summary>
    [SugarColumn(ColumnDescription = "属性值")]
    public string? Value { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注")]
    [MaxLength(50)]
    public string? Remark { get; set; }

    /// <summary>
    /// 上传设备Id
    /// </summary>
    [SugarColumn(ColumnDescription = "上传设备Id")]
    public long UploadDeviceId { get; set; }
    #endregion
    #region 其他属性
    /// <summary>
    /// 对应设备
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(UploadDeviceId))]
    [SugarColumn(IsIgnore = true)]
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public UploadDevice UploadDevice { get; set; }
    #endregion
}

