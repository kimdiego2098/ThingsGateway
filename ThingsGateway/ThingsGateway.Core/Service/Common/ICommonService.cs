namespace ThingsGateway.Core.Service;

public interface ICommonService
{

    string GetHost();

    string GetFileUrl(SysFile sysFile);
}