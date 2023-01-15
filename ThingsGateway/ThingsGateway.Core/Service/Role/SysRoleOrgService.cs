namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统角色机构服务
/// </summary>
public class SysRoleOrgService : ITransient
{
    private readonly SqlSugarRepository<SysRoleOrg> _sysRoleOrgRep;

    public SysRoleOrgService(SqlSugarRepository<SysRoleOrg> sysRoleOrgRep)
    {
        _sysRoleOrgRep = sysRoleOrgRep;
    }

    /// <summary>
    /// 授权角色机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task GrantRoleOrg(RoleOrgInput input)
    {
        await _sysRoleOrgRep.DeleteAsync(u => u.RoleId == input.Id);
        if (input.DataScope == (int)DataScopeEnum.Define)
        {
            var roleOrgs = input.OrgIdList.Select(u => new SysRoleOrg
            {
                RoleId = input.Id,
                OrgId = u
            }).ToList();
            await _sysRoleOrgRep.InsertRangeAsync(roleOrgs);
        }
    }

    /// <summary>
    /// 根据角色Id集合获取角色机构Id集合
    /// </summary>
    /// <param name="roleIdList"></param>
    /// <returns></returns>
    public async Task<List<long>> GetRoleOrgIdList(List<long> roleIdList)
    {
        return await _sysRoleOrgRep.AsQueryable()
            .Where(u => roleIdList.Contains(u.RoleId))
            .Select(u => u.OrgId).ToListAsync();
    }

    /// <summary>
    /// 根据机构Id集合删除角色机构
    /// </summary>
    /// <param name="orgIdList"></param>
    /// <returns></returns>
    public async Task DeleteRoleOrgByOrgIdList(List<long> orgIdList)
    {
        await _sysRoleOrgRep.DeleteAsync(u => orgIdList.Contains(u.OrgId));
    }

    /// <summary>
    /// 根据角色Id删除角色机构
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task DeleteRoleOrgByRoleId(long roleId)
    {
        await _sysRoleOrgRep.DeleteAsync(u => u.RoleId == roleId);
    }
}