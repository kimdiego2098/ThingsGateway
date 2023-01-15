namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统机构服务
/// </summary>
[ApiDescriptionSettings(Order = 197)]
public class SysOrgService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysOrg> _sysOrgRep;
    private readonly SysCacheService _sysCacheService;
    private readonly SysUserExtOrgService _sysUserExtOrgService;
    private readonly SysUserRoleService _sysUserRoleService;
    private readonly SysRoleOrgService _sysRoleOrgService;

    public SysOrgService(UserManager userManager,
        SqlSugarRepository<SysOrg> sysOrgRep,
        SysCacheService sysCacheService,
        SysUserExtOrgService sysUserExtOrgService,
        SysUserRoleService sysUserRoleService,
        SysRoleOrgService sysRoleOrgService)
    {
        _sysOrgRep = sysOrgRep;
        _userManager = userManager;
        _sysCacheService = sysCacheService;
        _sysUserExtOrgService = sysUserExtOrgService;
        _sysUserRoleService = sysUserRoleService;
        _sysRoleOrgService = sysRoleOrgService;
    }

    /// <summary>
    /// 获取机构列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<SysOrg>> GetOrgList([FromQuery] OrgInput input)
    {
        var orgIdList = await GetUserOrgIdList();

        var iSugarQueryable = _sysOrgRep.AsQueryable().OrderBy(u => u.OrderNo);

        // 条件筛选可能造成无法构造树（列表数据）
        if (!string.IsNullOrWhiteSpace(input.Name) || !string.IsNullOrWhiteSpace(input.Code))
        {
            return await iSugarQueryable.WhereIF(orgIdList.Count > 0, u => orgIdList.Contains(u.Id))
                .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
                .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
                .ToListAsync();
        }

        if (input.Id > 0)
        {
            return await iSugarQueryable.WhereIF(orgIdList.Count > 0, u => orgIdList.Contains(u.Id)).ToChildListAsync(u => u.Pid, input.Id, true);
        }
        else
        {
            return _userManager.SuperAdmin ?
                await iSugarQueryable.ToTreeAsync(u => u.Children, u => u.Pid, 0) :
                await iSugarQueryable.ToTreeAsync(u => u.Children, u => u.Pid, 0, orgIdList.Select(d => (object)d).ToArray());
        }
    }

    /// <summary>
    /// 增加机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<long> AddOrg(AddOrgInput input)
    {
        var isExist = await _sysOrgRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D2002);

        var orgIdList = await GetUserOrgIdList();
        if (!_userManager.SuperAdmin)
        {
            // 新增机构父Id不是0，则进行权限校验
            if (input.Pid != 0)
            {
                // 新增机构的父机构不在自己的数据范围内
                if (orgIdList.Count < 1 || !orgIdList.Contains(input.Pid))
                    throw Oops.Oh(ErrorCodeEnum.D2003);
            }
            else
                throw Oops.Oh(ErrorCodeEnum.D2006);
        }

        // 删除当前用户的机构缓存
        _sysCacheService.Remove(CacheConst.KeyOrgIdList + _userManager.UserId);
        var sysOrg = input.Adapt<SysOrg>();
        var newOrg = await _sysOrgRep.AsInsertable(sysOrg).ExecuteReturnEntityAsync();
        return newOrg.Id;
    }

    /// <summary>
    /// 更新机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [UnitOfWork]
    public async Task UpdateOrg(UpdateOrgInput input)
    {
        if (input.Pid != 0)
        {
            var pOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Pid);
            _ = pOrg ?? throw Oops.Oh(ErrorCodeEnum.D2000);
        }
        if (input.Id == input.Pid)
            throw Oops.Oh(ErrorCodeEnum.D2001);

        var sysOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Id);
        var isExist = await _sysOrgRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code && u.Id != sysOrg.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D2002);

        // 父Id不能为自己的子节点
        var childIdList = await GetChildIdListWithSelfById(input.Id);
        if (childIdList.Contains(input.Pid))
            throw Oops.Oh(ErrorCodeEnum.D2001);

        // 是否有权限操作此机构
        var dataScopes = await GetUserOrgIdList();
        if (!_userManager.SuperAdmin && (dataScopes.Count < 1 || !dataScopes.Contains(sysOrg.Id)))
            throw Oops.Oh(ErrorCodeEnum.D2003);

        await _sysOrgRep.AsUpdateable(input.Adapt<SysOrg>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [UnitOfWork]
    public async Task DeleteOrg(DeleteOrgInput input)
    {
        var sysOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Id);

        // 是否有权限操作此机构
        if (!_userManager.SuperAdmin)
        {
            var dataScopes = await GetUserOrgIdList();
            if (dataScopes.Count < 1 || !dataScopes.Contains(sysOrg.Id))
                throw Oops.Oh(ErrorCodeEnum.D2003);
        }

        // 若扩展机构有用户则禁止删除
        var hasExtOrgEmp = await _sysUserExtOrgService.HasUserOrg(sysOrg.Id);
        if (hasExtOrgEmp)
            throw Oops.Oh(ErrorCodeEnum.D2005);

        // 若子机构有用户则禁止删除
        var orgTreeList = await _sysOrgRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
        var orgIdList = orgTreeList.Select(u => u.Id).ToList();

        // 级联删除机构子节点
        await _sysOrgRep.DeleteAsync(u => orgIdList.Contains(u.Id));

        // 级联删除角色机构数据
        await _sysRoleOrgService.DeleteRoleOrgByOrgIdList(orgIdList);

        // 级联删除用户机构数据
        await _sysUserExtOrgService.DeleteUserExtOrgByOrgIdList(orgIdList);
    }

    /// <summary>
    /// 根据用户Id获取机构Id集合
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetUserOrgIdList()
    {
        if (_userManager.SuperAdmin)
            return new List<long>();

        var userId = _userManager.UserId;
        var orgIdList = _sysCacheService.Get<List<long>>(CacheConst.KeyOrgIdList + userId); // 取缓存
        if (orgIdList == null || orgIdList.Count < 1)
        {
            // 扩展机构集合
            var orgList1 = await _sysUserExtOrgService.GetUserExtOrgList(userId);
            // 角色机构集合
            var orgList2 = await GetUserRoleOrgIdList(userId);
            // 机构并集
            orgIdList = orgList1.Select(u => u.OrgId).Union(orgList2).ToList();
            // 当前所属机构
            if (!orgIdList.Contains(_userManager.OrgId))
                orgIdList.Add(_userManager.OrgId);
            _sysCacheService.Set(CacheConst.KeyOrgIdList + userId, orgIdList); // 存缓存
        }
        return orgIdList;
    }

    /// <summary>
    /// 获取用户角色机构Id集合
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<List<long>> GetUserRoleOrgIdList(long userId)
    {
        var roleList = await _sysUserRoleService.GetUserRoleList(userId);
        if (roleList.Count < 1)
            return new List<long>(); // 空机构Id集合

        return await GetUserOrgIdList(roleList);
    }

    /// <summary>
    /// 根据角色Id集合获取机构Id集合
    /// </summary>
    /// <param name="roleList"></param>
    /// <returns></returns>
    private async Task<List<long>> GetUserOrgIdList(List<SysRole> roleList)
    {
        // 按最大范围策略设定(如果同时拥有ALL和SELF的权限，则结果ALL)
        int strongerDataScopeType = (int)DataScopeEnum.Self;

        // 数据范围拥有的角色集合
        var customDataScopeRoleIdList = new List<long>();
        if (roleList != null && roleList.Count > 0)
        {
            roleList.ForEach(u =>
            {
                if (u.DataScope == DataScopeEnum.Define)
                    customDataScopeRoleIdList.Add(u.Id);
                else if ((int)u.DataScope <= strongerDataScopeType)
                    strongerDataScopeType = (int)u.DataScope;
            });
        }

        // 根据角色集合获取机构集合
        var orgIdList1 = await _sysRoleOrgService.GetRoleOrgIdList(customDataScopeRoleIdList);
        // 根据数据范围获取机构集合
        var orgIdList2 = await GetOrgIdListByDataScope(strongerDataScopeType);

        // 缓存当前用户最大角色数据范围
        _sysCacheService.Set(CacheConst.KeyMaxDataScopeType + _userManager.UserId, strongerDataScopeType);

        // 并集机构集合
        return orgIdList1.Union(orgIdList2).ToList();
    }

    /// <summary>
    /// 根据数据范围获取机构Id集合
    /// </summary>
    /// <param name="dataScope"></param>
    /// <returns></returns>
    private async Task<List<long>> GetOrgIdListByDataScope(int dataScope)
    {
        var orgId = _userManager.OrgId;
        var orgIdList = new List<long>();
        // 若数据范围是全部，则获取所有机构Id集合
        if (dataScope == (int)DataScopeEnum.All)
        {
            orgIdList = await _sysOrgRep.AsQueryable().Select(u => u.Id).ToListAsync();
        }
        // 若数据范围是本部门及以下，则获取本节点和子节点集合
        else if (dataScope == (int)DataScopeEnum.DeptChild)
        {
            orgIdList = await GetChildIdListWithSelfById(orgId);
        }
        // 若数据范围是本部门不含子节点，则直接返回本部门
        else if (dataScope == (int)DataScopeEnum.Dept)
        {
            orgIdList.Add(orgId);
        }
        return orgIdList;
    }

    /// <summary>
    /// 根据节点Id获取子节点Id集合(包含自己)
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetChildIdListWithSelfById(long pid)
    {
        var orgTreeList = await _sysOrgRep.AsQueryable().ToChildListAsync(u => u.Pid, pid, true);
        return orgTreeList.Select(u => u.Id).ToList();
    }
}