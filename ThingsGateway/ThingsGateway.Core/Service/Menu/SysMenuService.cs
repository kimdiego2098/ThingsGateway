namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统菜单服务
/// </summary>
[ApiDescriptionSettings(Order = 195)]
public class SysMenuService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysMenu> _sysMenuRep;
    private readonly SysRoleMenuService _sysRoleMenuService;
    private readonly SysUserRoleService _sysUserRoleService;
    private readonly SysCacheService _sysCacheService;

    public SysMenuService(UserManager userManager,
        SqlSugarRepository<SysMenu> sysMenuRep,
        SysRoleMenuService sysRoleMenuService,
        SysUserRoleService sysUserRoleService,
        SysCacheService sysCacheService)
    {
        _userManager = userManager;
        _sysMenuRep = sysMenuRep;
        _sysRoleMenuService = sysRoleMenuService;
        _sysUserRoleService = sysUserRoleService;
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// 获取登录菜单树
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<MenuOutput>> GetLoginMenuTree()
    {
        if (_userManager.SuperAdmin)
        {
            var menuList = await _sysMenuRep.AsQueryable()
    .Where(u => u.Type != MenuTypeEnum.Btn && u.Status == StatusEnum.Enable)
                .OrderBy(u => u.OrderNo).ToTreeAsync(u => u.Children, u => u.Pid, 0);
            return menuList.Adapt<List<MenuOutput>>();
        }
        else
        {
            var menuIdList = await GetMenuIdList();
            var menuTree = await _sysMenuRep.AsQueryable()
                  .Where(u => u.Status == StatusEnum.Enable)
                .OrderBy(u => u.OrderNo).ToTreeAsync(u => u.Children, u => u.Pid, 0, menuIdList.Select(d => (object)d).ToArray());
            DeleteBtnFromMenuTree(menuTree);
            return menuTree.Adapt<List<MenuOutput>>();
        }
    }

    /// <summary>
    /// 删除登录菜单树里面的按钮
    /// </summary>
    private void DeleteBtnFromMenuTree(List<SysMenu> menuList)
    {
        for (var i = menuList.Count - 1; i >= 0; i--)
        {
            var menu = menuList[i];
            if (menu.Type == MenuTypeEnum.Btn)
                menuList.Remove(menu);
            else if (menu.Children.Count > 0)
                DeleteBtnFromMenuTree(menu.Children);
        }
    }

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<List<SysMenu>> GetMenuList([FromQuery] MenuInput input)
    {
        var menuIdList = _userManager.SuperAdmin ? new List<long>() : await GetMenuIdList();

        // 有筛选条件时返回list列表（防止构造不出树）
        if (!string.IsNullOrWhiteSpace(input.Title) || input.Type > 0)
        {
            return await _sysMenuRep.AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(input.Title), u => u.Title.Contains(input.Title))
                .WhereIF(input.Type > 0, u => u.Type == input.Type)
                .WhereIF(menuIdList.Count > 1, u => menuIdList.Contains(u.Id))
                .OrderBy(u => u.OrderNo).ToListAsync();
        }

        return _userManager.SuperAdmin ?
            await _sysMenuRep.AsQueryable().OrderBy(u => u.OrderNo).ToTreeAsync(u => u.Children, u => u.Pid, 0) :
            await _sysMenuRep.AsQueryable()
                .OrderBy(u => u.OrderNo).ToTreeAsync(u => u.Children, u => u.Pid, 0, menuIdList.Select(d => (object)d).ToArray()); // 角色菜单授权时
    }

    /// <summary>
    /// 增加菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AddMenu(AddMenuInput input)
    {
        var isExist = input.Type != MenuTypeEnum.Btn
            ? await _sysMenuRep.IsAnyAsync(u => u.Title == input.Title)
            : await _sysMenuRep.IsAnyAsync(u => u.Permission == input.Permission);

        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D4000);

        // 校验菜单参数
        CheckMenuParam(input.Adapt<SysMenu>());

        await _sysMenuRep.InsertAsync(input.Adapt<SysMenu>());

        // 清除缓存
        DeleteMenuCache();
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateMenu(UpdateMenuInput input)
    {
        var isExist = input.Type != MenuTypeEnum.Btn
            ? await _sysMenuRep.IsAnyAsync(u => u.Title == input.Title && u.Type == input.Type && u.Id != input.Id)
            : await _sysMenuRep.IsAnyAsync(u => u.Permission == input.Permission && u.Type == input.Type && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D4000);

        // 校验菜单参数
        CheckMenuParam(input.Adapt<SysMenu>());

        await _sysMenuRep.AsUpdateable(input.Adapt<SysMenu>()).IgnoreColumns(true).ExecuteCommandAsync();

        // 清除缓存
        DeleteMenuCache();
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [UnitOfWork]
    public async Task DeleteMenu(DeleteMenuInput input)
    {
        var menuTreeList = await _sysMenuRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
        var menuIdList = menuTreeList.Select(u => u.Id).ToList();

        await _sysMenuRep.DeleteAsync(u => menuIdList.Contains(u.Id));

        // 级联删除角色菜单数据
        await _sysRoleMenuService.DeleteRoleMenuByMenuIdList(menuIdList);

        // 清除缓存
        DeleteMenuCache();
    }

    /// <summary>
    /// 增加和编辑时检查菜单数据
    /// </summary>
    /// <param name="menu"></param>
    private static void CheckMenuParam(SysMenu menu)
    {
        var permission = menu.Permission;
        if (menu.Type == MenuTypeEnum.Btn)
        {
            if (string.IsNullOrEmpty(permission))
                throw Oops.Oh(ErrorCodeEnum.D4003);
            if (!permission.Contains(':'))
                throw Oops.Oh(ErrorCodeEnum.D4004);
        }
    }

    /// <summary>
    /// 获取按钮权限列表(登录)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<string>> GetPermCodeList()
    {
        var userId = _userManager.UserId;
        var permissions = _sysCacheService.Get<List<string>>(CacheConst.KeyPermission + userId); // 取缓存
        if (permissions == null || permissions.Count == 0)
        {
            var menuIdList = _userManager.SuperAdmin ? new List<long>() : await GetMenuIdList();
            permissions = await _sysMenuRep.AsQueryable()
                .Where(u => u.Type == MenuTypeEnum.Btn)
                .WhereIF(menuIdList.Count > 0, u => menuIdList.Contains(u.Id))
                .Select(u => u.Permission).ToListAsync();
            _sysCacheService.Set(CacheConst.KeyPermission + userId, permissions); // 缓存结果
        }
        return permissions;
    }

    /// <summary>
    /// 获取所有按钮权限集合
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<string>> GetAllPermCodeList()
    {
        var permissions = _sysCacheService.Get<List<string>>(CacheConst.KeyPermission + 0); // 先从缓存里面读取
        if (permissions == null || permissions.Count == 0)
        {
            permissions = await _sysMenuRep.AsQueryable()
                .Where(u => u.Type == MenuTypeEnum.Btn)
                .Select(u => u.Permission).ToListAsync();
            _sysCacheService.Set(CacheConst.KeyPermission + 0, permissions); // 缓存结果
        }
        return permissions;
    }

    /// <summary>
    /// 清除菜单和按钮缓存
    /// </summary>
    private void DeleteMenuCache()
    {
        _sysCacheService.RemoveByPrefixKey(CacheConst.KeyMenu);
        _sysCacheService.RemoveByPrefixKey(CacheConst.KeyPermission);
    }

    /// <summary>
    /// 获取当前用户菜单Id集合
    /// </summary>
    /// <returns></returns>
    private async Task<List<long>> GetMenuIdList()
    {
        var roleIdList = await _sysUserRoleService.GetUserRoleIdList(_userManager.UserId);
        return await _sysRoleMenuService.GetRoleMenuIdList(roleIdList);
    }
}