namespace ThingsGateway.Core.Service;

/// <summary>
/// 微信账号服务
/// </summary>
[ApiDescriptionSettings(Order = 100)]
public class SysWechatUserService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysWechatUser> _sysWechatUserRep;

    public SysWechatUserService(SqlSugarRepository<SysWechatUser> sysWechatUserRep)
    {
        _sysWechatUserRep = sysWechatUserRep;
    }

    /// <summary>
    /// 获取微信用户列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<SysWechatUser>> GetWechatUserPage([FromQuery] WechatUserInput input)
    {
        return await _sysWechatUserRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.NickName), u => u.NickName.Contains(input.NickName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Mobile), u => u.Mobile.Contains(input.Mobile))
            .OrderBy(u => u.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加微信用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AddWechatUser(SysWechatUser input)
    {
        await _sysWechatUserRep.InsertAsync(input.Adapt<SysWechatUser>());
    }

    /// <summary>
    /// 更新微信用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateWechatUser(SysWechatUser input)
    {
        var weChatUser = input.Adapt<SysWechatUser>();
        await _sysWechatUserRep.AsUpdateable(weChatUser).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除微信用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteWechatUser(DeleteWechatUserInput input)
    {
        await _sysWechatUserRep.DeleteAsync(u => u.Id == input.Id);
    }
}