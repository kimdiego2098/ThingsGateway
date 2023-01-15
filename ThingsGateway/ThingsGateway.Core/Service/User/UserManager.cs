namespace ThingsGateway.Core;

/// <summary>
/// 当前登录用户
/// </summary>
public class UserManager : IScoped
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private long _tenantId;

    public long UserId
    {
        get => long.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.UserId)?.Value);
    }

    public long TenantId
    {
        get
        {
            var tId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.TenantId);
            return tId == null ? _tenantId : long.Parse(tId.Value);
        }
        set => _tenantId = value;
    }

    public string Account
    {
        get => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.Account)?.Value;
    }

    public string RealName
    {
        get => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.RealName)?.Value;
    }

    public bool SuperAdmin
    {
        get => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.AccountType)?.Value == ((int)AccountTypeEnum.SuperAdmin).ToString();
    }

    public long OrgId
    {
        get => long.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.OrgId)?.Value);
    }

    public string OpenId
    {
        get => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.OpenId)?.Value;
    }

    public UserManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
}