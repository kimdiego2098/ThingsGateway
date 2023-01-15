using Magicodes.ExporterAndImporter.Excel;

namespace ThingsGateway.Application.Core;

/// <summary>
/// 系统网关日志服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 200)]
public class GatewayLogOpService : IDynamicApiController, IScoped
{
    private readonly SqlSugarRepository<GatewayLogOp> _sysLogOpRep;

    public GatewayLogOpService(SqlSugarRepository<GatewayLogOp> sysLogOpRep)
    {
        _sysLogOpRep = sysLogOpRep;
    }

    /// <summary>
    /// 获取网关日志分页列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [SuppressMonitor]
    public async Task<SqlSugarPagedList<GatewayLogOp>> GetLogOpPage([FromQuery] PageGatewayLogLogInput input)
    {
        return await _sysLogOpRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
                    .WhereIF(!string.IsNullOrWhiteSpace(input.LogName?.ToString()),
            u => u.LogName.Contains(input.LogName))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.LogMessage?.ToString()),
            u => u.Message.Contains(input.LogMessage))
                     .WhereIF(!string.IsNullOrWhiteSpace(input.LogLevel?.ToString()),
            u => u.LogLevel.Contains(input.LogLevel))
            .OrderBy(u => u.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 清空网关日志
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> ClearLogOp()
    {
        return await _sysLogOpRep.DeleteAsync(u => u.Id > 0);
    }

    /// <summary>
    /// 导出网关日志
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ExportLogOp(GatewayLogLogInput input)
    {
        var lopOpList = await _sysLogOpRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                    u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
                                    .WhereIF(!string.IsNullOrWhiteSpace(input.LogName?.ToString()),
                u => u.LogName.Contains(input.LogName))
                        .WhereIF(!string.IsNullOrWhiteSpace(input.LogMessage?.ToString()),
                u => u.Message.Contains(input.LogMessage))
                         .WhereIF(!string.IsNullOrWhiteSpace(input.LogLevel?.ToString()),
                u => u.LogLevel.Contains(input.LogLevel))
            .OrderBy(u => u.Id, OrderByType.Desc).Take(10000)
            .Select<ExportLogDto>().ToListAsync();

        IExcelExporter excelExporter = new ExcelExporter();
        var res = await excelExporter.ExportAsByteArray(lopOpList);

        return new FileStreamResult(new MemoryStream(res), "application/octet-stream") { FileDownloadName = DateTime.Now.ToString("yyyyMMddHHmmss") + "网关日志.xlsx" };

    }
}