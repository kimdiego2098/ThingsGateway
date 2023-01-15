namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统职位服务
/// </summary>
[ApiDescriptionSettings(Order = 196)]
public class SysPosService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysPos> _sysPosRep;
    private readonly SysUserExtOrgService _sysUserExtOrgService;

    public SysPosService(SqlSugarRepository<SysPos> sysPosRep,
        SysUserExtOrgService sysUserExtOrgService)
    {
        _sysPosRep = sysPosRep;
        _sysUserExtOrgService = sysUserExtOrgService;
    }

    /// <summary>
    /// 获取职位列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<SysPos>> GetPosList([FromQuery] PosInput input)
    {
        return await _sysPosRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .OrderBy(u => u.OrderNo).ToListAsync();
    }

    /// <summary>
    /// 增加职位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AddPos(AddPosInput input)
    {
        var isExist = await _sysPosRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D6000);

        await _sysPosRep.InsertAsync(input.Adapt<SysPos>());
    }

    /// <summary>
    /// 更新职位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdatePos(UpdatePosInput input)
    {
        var isExist = await _sysPosRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D6000);

        await _sysPosRep.AsUpdateable(input.Adapt<SysPos>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除职位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task DeletePos(DeletePosInput input)
    {
        // 该职位下是否有用户
        var hasPosEmp = await _sysPosRep.ChangeRepository<SqlSugarRepository<SysUser>>()
            .IsAnyAsync(u => u.PosId == input.Id);
        if (hasPosEmp)
            throw Oops.Oh(ErrorCodeEnum.D6001);

        // 该附属职位下是否有用户
        var hasExtPosEmp = await _sysUserExtOrgService.HasUserPos(input.Id);
        if (hasExtPosEmp)
            throw Oops.Oh(ErrorCodeEnum.D6001);

        await _sysPosRep.DeleteAsync(u => u.Id == input.Id);
    }
}