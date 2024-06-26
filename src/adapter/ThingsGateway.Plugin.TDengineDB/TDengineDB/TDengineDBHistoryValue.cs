﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using BootstrapBlazor.Components;

using SqlSugar;
using SqlSugar.TDengine;

using ThingsGateway.Core;

namespace ThingsGateway.Plugin.TDengineDB;

/// <summary>
/// 历史数据表
/// </summary>
[SugarTable("historyValue")]
public class TDengineDBHistoryValue : STable, IPrimaryIdEntity, IDBHistoryValue
{
    public long Id { get; set; }

    /// <summary>
    /// 上传时间
    /// </summary>
    [SugarColumn(InsertServerTime = true)]
    [AutoGenerateColumn(Order = 1, Visible = true, Sortable = true, Filterable = false)]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 采集时间
    /// </summary>
    [AutoGenerateColumn(Order = 1, Visible = true, Sortable = true, Filterable = false)]
    public DateTime CollectTime { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    [AutoGenerateColumn(Order = 1, Visible = true, Sortable = true, Filterable = false)]
    public string DeviceName { get; set; }

    /// <summary>
    /// 变量名称
    /// </summary>
    [AutoGenerateColumn(Order = 1, Visible = true, Sortable = true, Filterable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 是否在线
    /// </summary>
    [AutoGenerateColumn(Order = 1, Visible = true, Sortable = true, Filterable = false)]
    public bool IsOnline { get; set; }

    /// <summary>
    /// 变量值
    /// </summary>
    [SugarColumn(Length = 18, DecimalDigits = 2)]
    [AutoGenerateColumn(Order = 1, Visible = true, Sortable = true, Filterable = false)]
    public string Value { get; set; }
}
