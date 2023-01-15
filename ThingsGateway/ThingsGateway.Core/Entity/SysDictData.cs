namespace ThingsGateway.Core;

/// <summary>
/// 系统字典值表
/// </summary>
[SugarTable(null, "系统字典值表")]
[SystemTable]
public class SysDictData : EntityBase
{
    /// <summary>
    /// 字典类型Id
    /// </summary>
    [SugarColumn(ColumnDescription = "字典类型Id")]
    public long DictTypeId { get; set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(DictTypeId))]
    public SysDictType DictType { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [SugarColumn(ColumnDescription = "值", Length = 128)]
    [Required, MaxLength(128)]
    public virtual string Value { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    [SugarColumn(ColumnDescription = "编码", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string Code { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 128)]
    [MaxLength(128)]
    public string? Remark { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;
}