using Magicodes.ExporterAndImporter.Excel;

namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统操作日志服务
/// </summary>
[ApiDescriptionSettings(Order = 179)]
public class SysLogOpService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLogOp> _sysLogOpRep;

    public SysLogOpService(SqlSugarRepository<SysLogOp> sysLogOpRep)
    {
        _sysLogOpRep = sysLogOpRep;
    }

    /// <summary>
    /// 获取操作日志分页列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [SuppressMonitor]
    public async Task<SqlSugarPagedList<SysLogOp>> GetLogOpPage([FromQuery] PageLogInput input)
    {
        return await _sysLogOpRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
            //.OrderBy(u => u.CreateTime, OrderByType.Desc)
            .OrderBuilder(input)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 清空操作日志
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> ClearLogOp()
    {
        return await _sysLogOpRep.DeleteAsync(u => u.Id > 0);
    }

    /// <summary>
    /// 导出操作日志
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ExporLogOp(LogInput input)
    {
        var lopOpList = await _sysLogOpRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                    u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .Select<ExportLogDto>().ToListAsync();

        IExcelExporter excelExporter = new ExcelExporter();
        var res = await excelExporter.ExportAsByteArray(lopOpList);

        return new FileStreamResult(new MemoryStream(res), "application/octet-stream") { FileDownloadName = DateTime.Now.ToString("yyyyMMddHHmm") + "操作日志.xlsx" };
    }
}