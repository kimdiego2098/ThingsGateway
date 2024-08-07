﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Components;

using System.Diagnostics.CodeAnalysis;

namespace BootstrapBlazor.Components;

/// <summary>
/// 构造函数
/// </summary>
/// <param name="fieldName">字段名称</param>
/// <param name="fieldType">字段类型</param>
/// <param name="fieldText">显示文字</param>
internal class InternalEditorItem(string fieldName, Type fieldType, string? fieldText = null) : IEditorItem
{
    public IEnumerable<KeyValuePair<string, object>>? ComponentParameters { get; set; }
    public Type? ComponentType { get; set; }

    [ExcludeFromCodeCoverage]
    public bool Editable { get; set; } = true;

    public RenderFragment<object>? EditTemplate { get; set; }
    public string? GroupName { get; set; }
    public int GroupOrder { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool? Ignore { get; set; }

    public bool IsPopover { get; set; }
    public bool? IsReadonlyWhenAdd { get; set; }
    public bool? IsReadonlyWhenEdit { get; set; }
    public bool? IsVisibleWhenAdd { get; set; } = true;
    public bool? IsVisibleWhenEdit { get; set; } = true;
    public IEnumerable<SelectedItem>? Items { get; set; }
    public IEnumerable<SelectedItem>? Lookup { get; set; }
    public object? LookupServiceData { get; set; }
    public string? LookupServiceKey { get; set; }
    public StringComparison LookupStringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;
    public int Order { get; set; }
    public string? PlaceHolder { get; set; }
    public Type PropertyType => fieldType;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool? Readonly { get; set; }

    public int Rows { get; set; }
    public bool? ShowLabelTooltip { get; set; }
    public bool ShowSearchWhenSelect { get; set; }
    public bool SkipValidate { get; set; }
    public string? Step { get; set; }
    public string? Text { get; set; } = fieldText;
    public List<IValidator>? ValidateRules { get; set; }

    public string GetDisplayName() => Text;

    public string GetFieldName() => fieldName;
}
