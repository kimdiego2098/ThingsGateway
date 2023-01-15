namespace ThingsGateway.Application.Core;
/// <summary>
/// 上传设备运行态API服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.RunTimeGroupName, Order = 300)]
public class UploadDeviceRunTimeService : IDynamicApiController, IScoped
{
    private readonly UploadService _uploadService;
    private readonly SqlSugarRepository<UploadDevice> _deviceRep;

    public UploadDeviceRunTimeService(
        SqlSugarRepository<UploadDevice> deviceRep,
        IServiceProvider serviceProvider
        )
    {
        _uploadService = serviceProvider.GetBackgroundService<UploadService>();
        _deviceRep = deviceRep;
    }

    /// <summary>
    /// 获取设备状态分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<UploadDevice>> GetDevicePage([FromQuery] PageUploadDeviceInput input)
    {
        //    //先获取数据库中存在的设备
        //    var data = await _deviceRep.AsQueryable()
        //.WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
        //.WhereIF(!string.IsNullOrWhiteSpace(input.PluginName?.Trim()), u => u.DriverAssembleName.Contains(input.PluginName))
        //.OrderBy(u => u.CreateTime).ToPagedListAsync(input.Page, input.PageSize);

        var runTimeData = await _uploadService.UploadCores
            //.Where(it => data.Items.Any(a => a.Id == it.DeviceId))
            .Select(it => it.DeviceCopy)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PluginName?.Trim()), u => u.DriverAssembleName.Contains(input.PluginName))
            .OrderBy(u => u.CreateTime)
            .ToPagedListAsync(input.Page, input.PageSize);
        return runTimeData;
    }


}