﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://diego2098.gitee.io/thingsgateway-docs/
//  QQ群：605534569
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

using System.Data;

namespace ThingsGateway.Gateway.Application;

/// <summary>
/// RPC日志服务
/// </summary>
public interface IRpcLogService : ISugarService, ITransient
{
    /// <summary>
    /// 删除
    /// </summary>
    /// <returns></returns>
    Task DeleteAsync();

    /// <summary>
    /// 分页查询
    /// </summary>
    Task<SqlSugarPagedList<RpcLog>> PageAsync(RpcLogPageInput input);

    /// <summary>
    /// 导出
    /// </summary>
    Task<FileStreamResult> ExportFileAsync(RpcLogInput input);

    /// <summary>
    /// 导出
    /// </summary>
    Task<FileStreamResult> ExportFileAsync(IDataReader? input = null);

    /// <summary>
    /// 按天统计
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    Task<List<RpcLogDayStatisticsOutput>> StatisticsByDayAsync(int day);
}