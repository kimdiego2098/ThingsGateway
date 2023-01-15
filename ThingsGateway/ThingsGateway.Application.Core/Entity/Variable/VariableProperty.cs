using System.Text.Json.Serialization;

namespace ThingsGateway.Application.Core;
/// <summary>
/// 变量附加属性表
/// </summary>
[SugarTable("variable_propertie")]
[Description("变量附加属性表")]
[Tenant(ApplicationConst.ConfigId)]
[SystemTable]
public class VariableProperty : EntityBaseId
{
    #region SQL字段

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


    /// <summary>
    /// 变量Id
    /// </summary>
    [SugarColumn(ColumnDescription = "变量Id")]
    public long VariableId { get; set; }
    #endregion
    #region 其他属性
    /// <summary>
    /// 对应设备
    /// </summary>

    [Navigate(NavigateType.OneToOne, nameof(VariableId))]
    [SugarColumn(IsIgnore = true)]
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public MemoryVariable Variable { get; set; }
    #endregion
}
