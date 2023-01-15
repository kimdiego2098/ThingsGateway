namespace ThingsGateway.Core.Service;

public class CommonService : ICommonService, IScoped
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CommonService(IHttpContextAccessor httpContextAccessor
       )
    {
        _httpContextAccessor = httpContextAccessor;
    }


    /// <summary>
    /// 获取Host
    /// </summary>
    /// <returns></returns>
    public string GetHost()
    {
        return $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}";
    }

    /// <summary>
    /// 获取文件URL
    /// </summary>
    /// <param name="sysFile"></param>
    /// <returns></returns>
    public string GetFileUrl(SysFile sysFile)
    {
        return $"{GetHost()}/{sysFile.FilePath}/{sysFile.Id + sysFile.Suffix}";
    }
}