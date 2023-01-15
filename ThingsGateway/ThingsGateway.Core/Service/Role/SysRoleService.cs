namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统角色服务
/// </summary>
[ApiDescriptionSettings(Order = 198)]
public class SysRoleService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysRole> _sysRoleRep;
    private readonly SysCacheService _sysCacheService;
    private readonly SysRoleOrgService _sysRoleOrgService;
    private readonly SysRoleMenuService _sysRoleMenuService;
    private readonly SysOrgService _sysOrgService;
    private readonly SysUserRoleService _sysUserRoleService;

    public SysRoleService(UserManager userManager,
        SqlSugarRepository<SysRole> sysRoleRep,
        SysCacheService sysCacheService,
        SysRoleOrgService sysRoleOrgService,
        SysRoleMenuService sysRoleMenuService,
        SysOrgService sysOrgService,
        SysUserRoleService sysUserRoleService)
    {
        _userManager = userManager;
        _sysRoleRep = sysRoleRep;
        _sysCacheService = sysCacheService;
        _sysRoleOrgService = sysRoleOrgService;
        _sysRoleMenuService = sysRoleMenuService;
        _sysOrgService = sysOrgService;
        _sysUserRoleService = sysUserRoleService;
    }

    /// <summary>
    /// 获取角色分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<SysRole>> GetRolePage([FromQuery] PageRoleInput input)
    {
        return await _sysRoleRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .OrderBy(u => u.OrderNo)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<RoleOutput>> GetRoleList()
    {
        return await _sysRoleRep.AsQueryable().OrderBy(u => u.OrderNo).Select<RoleOutput>().ToListAsync();
    }

    /// <summary>
    /// 增加角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AddRole(AddRoleInput input)
    {
        var isExist = await _sysRoleRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D1006);

        var newRole = await _sysRoleRep.AsInsertable(input.Adapt<SysRole>()).ExecuteReturnEntityAsync();
        input.Id = newRole.Id;
        await UpdateRoleMenu(input);
    }

    /// <summary>
    /// 更新角色菜单权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task UpdateRoleMenu(AddRoleInput input)
    {
        if (input.MenuIdList == null || input.MenuIdList.Count < 1)
            return;
        await GrantRoleMenu(new RoleMenuInput()
        {
            Id = input.Id,
            MenuIdList = input.MenuIdList
        });
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateRole(UpdateRoleInput input)
    {
        var isExist = await _sysRoleRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D1006);

        await _sysRoleRep.AsUpdateable(input.Adapt<SysRole>()).IgnoreColumns(true)
            .IgnoreColumns(u => new { u.DataScope }).ExecuteCommandAsync();

        await UpdateRoleMenu(input);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [UnitOfWork]
    public async Task DeleteRole(DeleteRoleInput input)
    {
        var sysRole = await _sysRoleRep.GetFirstAsync(u => u.Id == input.Id);
        if (sysRole.Code == CommonConst.SysAdminRole)
            throw Oops.Oh(ErrorCodeEnum.D1019);

        await _sysRoleRep.DeleteAsync(sysRole);

        // 级联删除角色机构数据
        await _sysRoleOrgService.DeleteRoleOrgByRoleId(sysRole.Id);

        // 级联删除用户角色数据
        await _sysUserRoleService.DeleteUserRoleByRoleId(sysRole.Id);

        // 级联删除角色菜单数据
        await _sysRoleMenuService.DeleteRoleMenuByRoleId(sysRole.Id);
    }

    /// <summary>
    /// 授权角色菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task GrantRoleMenu(RoleMenuInput input)
    {
        await _sysRoleMenuService.GrantRoleMenu(input);
    }

    /// <summary>
    /// 授权角色数据范围
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task GrantRoleDataScope(RoleOrgInput input)
    {
        // 删除所有用户机构缓存
        _sysCacheService.RemoveByPrefixKey(CacheConst.KeyOrgIdList);

        var role = await _sysRoleRep.GetFirstAsync(u => u.Id == input.Id);
        var dataScope = input.DataScope;
        if (!_userManager.SuperAdmin)
        {
            // 非超级管理员没有全部数据范围权限
            if (dataScope == (int)DataScopeEnum.All)
                throw Oops.Oh(ErrorCodeEnum.D1016);

            // 若数据范围自定义，则判断授权数据范围是否有权限
            if (dataScope == (int)DataScopeEnum.Define)
            {
                var grantOrgIdList = input.OrgIdList;
                if (grantOrgIdList.Count > 0)
                {
                    var orgIdList = await _sysOrgService.GetUserOrgIdList();
                    if (orgIdList.Count < 1)
                        throw Oops.Oh(ErrorCodeEnum.D1016);
                    else if (!grantOrgIdList.All(u => orgIdList.Any(c => c == u)))
                        throw Oops.Oh(ErrorCodeEnum.D1016);
                }
            }
        }
        role.DataScope = (DataScopeEnum)dataScope;
        await _sysRoleRep.AsUpdateable(role).UpdateColumns(u => new { u.DataScope }).ExecuteCommandAsync();
        await _sysRoleOrgService.GrantRoleOrg(input);
    }

    /// <summary>
    /// 根据角色Id获取菜单集合(Element)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<long>> GetRoleOwnMenuList([FromQuery] RoleInput input)
    {
        return await _sysRoleMenuService.GetRoleMenuIdList(new List<long> { input.Id });
    }

    /// <summary>
    /// 根据角色Id获取机构Id集合
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<long>> GetRoleOwnOrg([FromQuery] RoleInput input)
    {
        return await _sysRoleOrgService.GetRoleOrgIdList(new List<long> { input.Id });
    }

    /// <summary>
    /// 设置角色状态
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<int> SetRoleStatus(RoleInput input)
    {
        if (!Enum.IsDefined(typeof(StatusEnum), input.Status))
            throw Oops.Oh(ErrorCodeEnum.D3005);

        return await _sysRoleRep.AsUpdateable()
            .SetColumns(u => u.Status == (StatusEnum)input.Status)
            .Where(u => u.Id == input.Id)
            .ExecuteCommandAsync();
    }
}