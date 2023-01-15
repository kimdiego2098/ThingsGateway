namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统字典值服务
/// </summary>
[ApiDescriptionSettings(Order = 191)]
[AllowAnonymous]
public class SysDictDataService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysDictData> _sysDictDataRep;

    public SysDictDataService(SqlSugarRepository<SysDictData> sysDictDataRep)
    {
        _sysDictDataRep = sysDictDataRep;
    }

    /// <summary>
    /// 获取字典值分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<SysDictData>> GetDictDataPage([FromQuery] PageDictDataInput input)
    {
        var code = !string.IsNullOrEmpty(input.Code?.Trim());
        var value = !string.IsNullOrEmpty(input.Value?.Trim());
        return await _sysDictDataRep.AsQueryable()
            .Where(u => u.DictTypeId == input.DictTypeId)
            .WhereIF(code, u => u.Code.Contains(input.Code))
       .WhereIF(value, u => u.Value.Contains(input.Value))
            .OrderBy(u => u.OrderNo)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取字典值列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<SysDictData>> GetDictDataList([FromQuery] GetDataDictDataInput input)
    {
        return await GetDictDataListByDictTypeId(input.DictTypeId);
    }

    /// <summary>
    /// 增加字典值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AddDictData(AddDictDataInput input)
    {
        var isExist = await _sysDictDataRep.IsAnyAsync(u => (u.Code == input.Code || u.Value == input.Value) && u.DictTypeId == input.DictTypeId);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D3003);

        await _sysDictDataRep.InsertAsync(input.Adapt<SysDictData>());
    }

    /// <summary>
    /// 更新字典值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateDictData(UpdateDictDataInput input)
    {
        var isExist = await _sysDictDataRep.IsAnyAsync(u => u.Id == input.Id);
        if (!isExist) throw Oops.Oh(ErrorCodeEnum.D3004);

        isExist = await _sysDictDataRep.IsAnyAsync(u => (u.Value == input.Value || u.Code == input.Code) && u.DictTypeId == input.DictTypeId && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D3003);

        await _sysDictDataRep.UpdateAsync(input.Adapt<SysDictData>());
    }

    /// <summary>
    /// 获取字典值详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SysDictData> GetDictData([FromQuery] DictDataInput input)
    {
        return await _sysDictDataRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 删除字典值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteDictData(DeleteDictDataInput input)
    {
        var dictData = await _sysDictDataRep.GetFirstAsync(u => u.Id == input.Id);
        if (dictData == null)
            throw Oops.Oh(ErrorCodeEnum.D3004);

        await _sysDictDataRep.DeleteAsync(dictData);
    }

    /// <summary>
    /// 修改字典值状态
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task ChangeDictDataStatus(DictDataInput input)
    {
        var dictData = await _sysDictDataRep.GetFirstAsync(u => u.Id == input.Id);
        if (dictData == null)
            throw Oops.Oh(ErrorCodeEnum.D3004);

        if (!Enum.IsDefined(typeof(StatusEnum), input.Status))
            throw Oops.Oh(ErrorCodeEnum.D3005);

        dictData.Status = (StatusEnum)input.Status;
        await _sysDictDataRep.UpdateAsync(dictData);
    }

    /// <summary>
    /// 根据字典类型Id获取字典值集合
    /// </summary>
    /// <param name="dictTypeId"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<List<SysDictData>> GetDictDataListByDictTypeId(long dictTypeId)
    {
        return await _sysDictDataRep.AsQueryable()
            .Where(u => u.DictTypeId == dictTypeId)
            .OrderBy(u => u.OrderNo).ToListAsync();
    }

    /// <summary>
    /// 根据字典唯一编码获取下拉框集合
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<dynamic> GetDictDataDropdown(string code)
    {
        return await _sysDictDataRep.Context.Queryable<SysDictType>()
            .LeftJoin<SysDictData>((a, b) => a.Id == b.DictTypeId)
            .Where((a, b) => a.Code == code && a.Status == StatusEnum.Enable && b.Status == StatusEnum.Enable)
            .Select((a, b) => new
            {
                Label = b.Value,
                Value = b.Code
            }).ToListAsync();
    }

    /// <summary>
    /// 根据条件查询字典获取下拉框集合
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<dynamic> QueryDictDataDropdown([FromQuery] QueryDictDataInput input)
    {
        return await _sysDictDataRep.Context.Queryable<SysDictType>()
            .LeftJoin<SysDictData>((a, b) => a.Id == b.DictTypeId)
            .Where((a, b) => a.Code == input.Code)
            .WhereIF(input.Status.HasValue, (a, b) => b.Status == (StatusEnum)input.Status.Value)
            .Select((a, b) => new
            {
                Label = b.Value,
                Value = b.Code
            }).ToListAsync();
    }
}