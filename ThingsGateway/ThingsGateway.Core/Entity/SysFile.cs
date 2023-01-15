namespace ThingsGateway.Core;

/// <summary>
/// 系统文件表
/// </summary>
[SugarTable(null, "系统文件表")]
[SystemTable]
public class SysFile : EntityBase
{
    /// <summary>
    /// 提供者
    /// </summary>
    [SugarColumn(ColumnDescription = "提供者", Length = 128)]
    [MaxLength(128)]
    public string? Provider { get; set; }

    /// <summary>
    /// 仓储名称
    /// </summary>
    [SugarColumn(ColumnDescription = "仓储名称", Length = 128)]
    [MaxLength(128)]
    public string? BucketName { get; set; }

    /// <summary>
    /// 文件名称（上传时名称）
    /// </summary>文件名称
    [SugarColumn(ColumnDescription = "文件名称", Length = 128)]
    [MaxLength(128)]
    public string? FileName { get; set; }

    /// <summary>
    /// 文件后缀
    /// </summary>
    [SugarColumn(ColumnDescription = "文件后缀", Length = 16)]
    [MaxLength(16)]
    public string? Suffix { get; set; }

    /// <summary>
    /// 存储路径
    /// </summary>
    [SugarColumn(ColumnDescription = "存储路径", Length = 128)]
    [MaxLength(128)]
    public string? FilePath { get; set; }

    /// <summary>
    /// 文件大小KB
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小KB", Length = 16)]
    [MaxLength(16)]
    public string? SizeKb { get; set; }

    /// <summary>
    /// 文件大小信息-计算后的
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小信息", Length = 64)]
    [MaxLength(64)]
    public string? SizeInfo { get; set; }

    /// <summary>
    /// 外链地址-OSS上传后生成外链地址方便前端预览
    /// </summary>
    [SugarColumn(ColumnDescription = "外链地址", Length = 128)]
    [MaxLength(128)]
    public string? Url { get; set; }
}