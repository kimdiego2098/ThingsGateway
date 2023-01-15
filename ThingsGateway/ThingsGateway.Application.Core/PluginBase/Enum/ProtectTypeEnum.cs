namespace ThingsGateway.Application.Core;

/// <summary>
/// 用户选择权限
/// </summary>
public enum ProtectTypeEnum
{
    [Display(Name = "只读")]
    ReadOnly,

    [Display(Name = "读写")]
    ReadWrite,

    [Display(Name = "只写")]
    WriteOnly,
}
