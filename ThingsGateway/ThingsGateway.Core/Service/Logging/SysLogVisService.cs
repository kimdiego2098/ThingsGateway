namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统访问日志服务
/// </summary>
[ApiDescriptionSettings(Order = 180)]
public class SysLogVisService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLogVis> _sysLogVisRep;

    public SysLogVisService(SqlSugarRepository<SysLogVis> sysLogVisRep)
    {
        _sysLogVisRep = sysLogVisRep;
    }

    /// <summary>
    /// 获取访问日志分页列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [SuppressMonitor]
    public async Task<SqlSugarPagedList<SysLogVis>> GetLogVisPage([FromQuery] PageLogInput input)
    {
        return await _sysLogVisRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 清空访问日志
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> ClearLogVis()
    {
        return await _sysLogVisRep.DeleteAsync(u => u.Id > 0);
    }
}