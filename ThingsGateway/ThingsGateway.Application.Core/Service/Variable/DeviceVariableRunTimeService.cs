using Microsoft.Extensions.Options;

using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Application.Core;


/// <summary>
/// 设备变量API服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.RunTimeGroupName, Order = 301)]
public class DeviceVariableRunTimeService : IDynamicApiController, IScoped
{
    private readonly DeviceCollectService _deviceCollectService;
    private readonly SqlSugarRepository<DeviceVariable> _deviceVariableRep;
    private readonly SqlSugarRepository<DeviceProperty> _devicePropertyRep;
    private readonly SqlSugarRepository<Device> _deviceRep;
    private readonly UploadOptions _uploadOptions;
    private readonly SysCacheService _sysCacheService;
    /// <summary>
    /// 网关数据都做了数据权限，所以RunTime数据也需要判断数据权限，这里直接使用数据库返回的标识再筛选
    /// </summary>
    public DeviceVariableRunTimeService(
        SqlSugarRepository<DeviceVariable> deviceVariableRep,
        SqlSugarRepository<Device> deviceRep, IOptions<UploadOptions> uploadOptions,
        SqlSugarRepository<DeviceProperty> devicePropertyRep,
        IServiceProvider serviceProvider,
        SysCacheService sysCacheService)
    {
        _uploadOptions = uploadOptions.Value;
        _deviceVariableRep = deviceVariableRep;
        _deviceRep = deviceRep;
        _sysCacheService = sysCacheService;
        _devicePropertyRep = devicePropertyRep;
        _deviceCollectService = serviceProvider.GetBackgroundService<DeviceCollectService>();
    }

    /// <summary>
    /// 获取设备变量分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<DeviceVariableCopy>> GetDeviceVariablePage([FromQuery] RunTimePageDeviceVariableInput input)
    {
        var list = _deviceCollectService.DeviceCollectCores.Select(a => a.DeviceVariablesCopy).ToList();
        var listdata = list.Where(it => it != null && it.Count > 0).SelectMany(a => a).ToList();
        //var listdata = _deviceCollectService.DeviceCollectCores.SelectMany(it => it.DeviceVariablesCopy).ToList();
        var runTimeData = listdata?.WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
           ?.WhereIF(!string.IsNullOrWhiteSpace(input.DeviceName?.Trim()), u => _deviceCollectService.DevIdNames.FirstOrDefault(it => it.Id == u.DeviceId).Name.Contains(input.DeviceName))
           ?.WhereIF(!string.IsNullOrWhiteSpace(input.Description?.Trim()), u => u.Description.Contains(input.Description))
           ?.WhereIF(!string.IsNullOrWhiteSpace(input.VariableAddress?.Trim()), u => u.VariableAddress.Contains(input.VariableAddress))
           ?.WhereIF(!string.IsNullOrWhiteSpace(input.Quality?.Trim()), u =>
           {
               if ("Bad".ToUpper().Contains(input.Quality.ToUpper()))
               {
                   return false;
               }
               else
               {
                   return true;
               }
           }
           )
           .ToList();
        var data = await runTimeData?.ToPagedListAsync(input.Page, input.PageSize);
        return data;

    }


    /// <summary>
    /// 写入值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<string> WriteDeviceVariableValue(WriteDeviceVariableInput input)
    {
        var data = await _deviceCollectService.InvokeDeviceMed("WEB API;USER:" + ToString(), new Dictionary<string, object>()
        {
            {  input.Name,input.WriteValue }
        }
        );
        if (data.IsSuccess) return data.ToJsonString();
        else throw new Exception(data.Message);
    }
    public override string ToString()
    {
        var user = App.GetService<UserManager>();
        return user?.Account;
    }

}