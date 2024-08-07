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

using System.ComponentModel.DataAnnotations;

namespace ThingsGateway.Plugin.QuestDB;

public class QuestDBProducerProperty : BusinessPropertyWithCacheInterval
{
    public DbType DbType { get; set; } = DbType.QuestDB;

    [DynamicProperty]
    [Required]
    [AutoGenerateColumn(ComponentType = typeof(Textarea), Rows = 1)]
    public string BigTextConnectStr { get; set; } = "host=localhost;port=8812;username=admin;password=quest;database=qdb;ServerCompatibilityMode=NoTypeLoading;";
}
