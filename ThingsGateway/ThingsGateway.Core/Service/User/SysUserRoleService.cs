namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统用户角色服务
/// </summary>
public class SysUserRoleService : ITransient
{
    private readonly SqlSugarRepository<SysUserRole> _sysUserRoleRep;

    public SysUserRoleService(SqlSugarRepository<SysUserRole> sysUserRoleRep)
    {
        _sysUserRoleRep = sysUserRoleRep;
    }

    /// <summary>
    /// 授权用户角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task GrantUserRole(UserRoleInput input)
    {
        await _sysUserRoleRep.DeleteAsync(u => u.UserId == input.UserId);

        if (input.RoleIdList == null || input.RoleIdList.Count < 1) return;
        var roles = input.RoleIdList.Select(u => new SysUserRole
        {
            UserId = input.UserId,
            RoleId = u
        }).ToList();
        await _sysUserRoleRep.InsertRangeAsync(roles);
    }

    /// <summary>
    /// 根据角色Id删除用户角色
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task DeleteUserRoleByRoleId(long roleId)
    {
        await _sysUserRoleRep.DeleteAsync(u => u.RoleId == roleId);
    }

    /// <summary>
    /// 根据用户Id删除用户角色
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task DeleteUserRoleByUserId(long userId)
    {
        await _sysUserRoleRep.DeleteAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// 根据用户Id获取角色集合
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<SysRole>> GetUserRoleList(long userId)
    {
        var sysUserRoleList = await _sysUserRoleRep.AsQueryable()
            .Mapper(u => u.SysRole, u => u.RoleId)
            .Where(u => u.UserId == userId).ToListAsync();
        return sysUserRoleList.Select(u => u.SysRole).ToList();
    }

    /// <summary>
    /// 根据用户Id获取角色Id集合
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<long>> GetUserRoleIdList(long userId)
    {
        return await _sysUserRoleRep.AsQueryable()
            .Where(u => u.UserId == userId).Select(u => u.RoleId).ToListAsync();
    }
}