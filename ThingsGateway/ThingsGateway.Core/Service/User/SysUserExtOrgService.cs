namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统用户扩展机构服务
/// </summary>
public class SysUserExtOrgService : ITransient
{
    private readonly SqlSugarRepository<SysUserExtOrg> _sysUserExtOrgRep;

    public SysUserExtOrgService(SqlSugarRepository<SysUserExtOrg> sysUserExtOrgRep)
    {
        _sysUserExtOrgRep = sysUserExtOrgRep;
    }

    /// <summary>
    /// 获取用户扩展机构集合
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<SysUserExtOrg>> GetUserExtOrgList(long userId)
    {
        return await _sysUserExtOrgRep.GetListAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// 更新用户扩展机构
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="extOrgList"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task UpdateUserExtOrg(long userId, List<SysUserExtOrg> extOrgList)
    {
        await _sysUserExtOrgRep.DeleteAsync(u => u.UserId == userId);

        if (extOrgList == null || extOrgList.Count < 1) return;
        extOrgList.ForEach(u =>
        {
            u.UserId = userId;
        });
        await _sysUserExtOrgRep.InsertRangeAsync(extOrgList);
    }

    /// <summary>
    /// 根据机构Id集合删除扩展机构
    /// </summary>
    /// <param name="orgIdList"></param>
    /// <returns></returns>
    public async Task DeleteUserExtOrgByOrgIdList(List<long> orgIdList)
    {
        await _sysUserExtOrgRep.DeleteAsync(u => orgIdList.Contains(u.OrgId));
    }

    /// <summary>
    /// 根据用户Id删除扩展机构
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task DeleteUserExtOrgByUserId(long userId)
    {
        await _sysUserExtOrgRep.DeleteAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// 根据机构Id判断是否有用户
    /// </summary>
    /// <param name="orgId"></param>
    /// <returns></returns>
    public async Task<bool> HasUserOrg(long orgId)
    {
        return await _sysUserExtOrgRep.IsAnyAsync(u => u.OrgId == orgId);
    }

    /// <summary>
    /// 根据职位Id判断是否有用户
    /// </summary>
    /// <param name="posId"></param>
    /// <returns></returns>
    public async Task<bool> HasUserPos(long posId)
    {
        return await _sysUserExtOrgRep.IsAnyAsync(u => u.PosId == posId);
    }
}