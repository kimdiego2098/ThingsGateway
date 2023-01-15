using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;

namespace ThingsGateway.Application.Core;
/// <summary>
/// 设备通用表
/// </summary>

[ExcelExporter(Name = "上传设备表")]
public class UploadDeviceExcel
{
    /// <summary>
    /// 名称
    /// </summary>
    [MaxLength(50)]
    [ImporterHeader(Name = "名称")]
    [ExporterHeader(DisplayName = "名称")]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(50)]
    [ImporterHeader(Name = "描述")]
    [ExporterHeader(DisplayName = "描述")]
    public string Description { get; set; }



    /// <summary>
    /// 设备使能
    /// </summary>
    [ImporterHeader(Name = "设备使能")]
    [ExporterHeader(DisplayName = "设备使能")]
    public bool InvokeEnable { get; set; }


    /// <summary>
    /// 驱动子程序集名称
    /// </summary>
    [ImporterHeader(Name = "驱动子程序集名称")]
    [ExporterHeader(DisplayName = "驱动子程序集名称")]
    public string DriverAssembleName { get; set; }




}

/// <summary>
/// 设备附加属性表
/// </summary>
[ExcelExporter(Name = "上传设备附加属性表")]
public class UploadDevicePropertyExcel
{
    /// <summary>
    /// 属性名称
    /// </summary>
    [ImporterHeader(Name = "名称")]
    [ExporterHeader(DisplayName = "名称")]
    public string UploadDevicePropertyName { get; set; }
    /// <summary>
    /// 属性描述
    /// </summary>
    [ImporterHeader(Name = "描述")]
    [ExporterHeader(DisplayName = "描述")]
    public string Description { get; set; }
    /// <summary>
    /// 属性值
    /// </summary>
    [ImporterHeader(Name = "属性值")]
    [ExporterHeader(DisplayName = "属性值")]
    public string Value { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    [ImporterHeader(Name = "备注")]
    [ExporterHeader(DisplayName = "备注")]
    [MaxLength(50)]
    public string Remark { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    [ImporterHeader(Name = "设备名称")]
    [ExporterHeader(DisplayName = "设备名称")]
    public string UploadDeviceName
    {
        get;
        set;
    }
}
public class ImportUploadDeviceExcelAndUploadDevicePropertyExcel
{
    [ExcelImporter(SheetName = "上传设备表")]
    public UploadDeviceExcel DeviceExcel { get; set; }

    [ExcelImporter(SheetName = "上传设备附加属性表")]
    public UploadDevicePropertyExcel DevicePropertyExcel { get; set; }
}

