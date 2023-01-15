using System.Text.Json.Serialization;

namespace ThingsGateway.Application.Core;
public enum HisType
{
    Change,
    Collect,
}
/// <summary>
/// 变量历史属性表
/// </summary>
[SugarTable("variable_his")]
[Description("变量历史属性表")]
[Tenant(ApplicationConst.ConfigId)]
[SystemTable]
public class VariableHis : EntityBaseId
{
    #region SQL字段
    /// <summary>
    /// 存储类型
    /// </summary>
    [SugarColumn(ColumnDescription = "存储类型")]
    public HisType HisType { get; set; }

    /// <summary>
    /// 启用
    /// </summary>
    [SugarColumn(ColumnDescription = "启用")]
    public bool Enable { get; set; }


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
    #endregion

}
