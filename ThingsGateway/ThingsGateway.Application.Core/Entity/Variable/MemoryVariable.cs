namespace ThingsGateway.Application.Core;
/// <summary>
/// 内存变量表
/// </summary>
[SugarTable("memory_variable")]
[Description("内存变量表")]
[Tenant(ApplicationConst.ConfigId)]
[SugarIndex("unique_variable_name", nameof(Name), OrderByType.Desc, true)]
[SystemTable]
public class MemoryVariable : EntityBase
{
    #region SQL字段

    /// <summary>
    /// 数据类型
    /// </summary>
    [SugarColumn(ColumnDescription = "数据类型")]
    public CoreDataType CoreDataType { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 初始值
    /// </summary>
    [SugarColumn(ColumnDescription = "初始值")]
    public string? InitialValue { get; set; }

    /// <summary>
    /// 读写权限
    /// </summary>
    [SugarColumn(ColumnDescription = "读写权限")]
    public ProtectTypeEnum ProtectType { get; set; }

    /// <summary>
    /// 变量名称
    /// </summary>
    [SugarColumn(ColumnDescription = "变量名称")]
    public string Name { get; set; }

    #endregion Public Properties

    #region 其他属性


    /// <summary>
    /// 变量属性
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(Id), nameof(VariableProperty.VariableId))]
    [SugarColumn(IsIgnore = true)]
    public VariableProperty VariablePropertys { get; set; }

    /// <summary>
    /// 变量报警
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(Id), nameof(VariableAlarm.VariableId))]
    //[Navigate(NavigateType.Dynamic, null)]
    [SugarColumn(IsIgnore = true)]
    public VariableAlarm VariableAlarms
    {
        get;
        set;
    }

    /// <summary>
    /// 变量历史
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(Id), nameof(VariableHis.VariableId))]
    //[Navigate(NavigateType.Dynamic, null)]
    [SugarColumn(IsIgnore = true)]
    public VariableHis VariableHiss
    {
        get;
        set;
    }


    /// <summary>
    /// Net数据类型
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Type DataType
    {
        get
        {
            if (Value != null && CoreDataType == CoreDataType.Object)
            {
                return Value.GetType();
            }
            else
            {
                return DataTypeItem.DictTypes[CoreDataType].Type;
            }
        }
    }

    /// <summary>
    /// 实时值
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public virtual object Value
    {

        get
        {
            return objvalue;
        }
        set
        {

            objvalue = value;
        }
    }
    private object objvalue;
    [SugarColumn(IsIgnore = true)]
    public virtual DateTime ChangeTime { get; set; }

    /// <summary>
    /// 原始值
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public object RawValue { get; set; }
    #endregion
}
