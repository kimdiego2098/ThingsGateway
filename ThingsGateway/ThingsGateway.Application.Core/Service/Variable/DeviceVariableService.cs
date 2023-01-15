using Magicodes.ExporterAndImporter.Excel;



using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Application.Core;


/// <summary>
/// 设备变量API服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 200)]
public class DeviceVariableService : IDynamicApiController, IScoped
{
    private readonly DeviceCollectService _deviceCollectService;
    private readonly SqlSugarRepository<DeviceVariable> _deviceVariableRep;
    private readonly SqlSugarRepository<DeviceProperty> _devicePropertyRep;
    private readonly SqlSugarRepository<Device> _deviceRep;
    private readonly UploadOptions _uploadOptions;
    private readonly SysCacheService _sysCacheService;
    private DeviceService _deviceService;
    public DeviceVariableService(
        SqlSugarRepository<DeviceVariable> deviceVariableRep,
        SqlSugarRepository<Device> deviceRep, IOptions<UploadOptions> uploadOptions,
        SqlSugarRepository<DeviceProperty> devicePropertyRep,
        IServiceProvider serviceProvider, DeviceService deviceService,
        SysCacheService sysCacheService)
    {
        _uploadOptions = uploadOptions.Value;
        _deviceVariableRep = deviceVariableRep;
        _deviceRep = deviceRep; _deviceService = deviceService;
        _sysCacheService = sysCacheService; _devicePropertyRep = devicePropertyRep;
        _deviceCollectService = serviceProvider.GetBackgroundService<DeviceCollectService>();

    }

    /// <summary>
    /// 获取设备变量分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<DeviceVariable>> GetDeviceVariablePage([FromQuery] PageDeviceVariableInput input)
    {
        var devIdName = await _deviceService.GetDeviceNames();
        long devId = 0;
        if (!string.IsNullOrWhiteSpace(input.DeviceName?.Trim()))
        {
            devId = devIdName.FirstOrDefault(it => it.Name.Contains(input.DeviceName))?.Id ?? -1;
        }
        var data = await _deviceVariableRep.AsQueryable()
           .Includes(a => a.VariablePropertys, t => t.Variable)
           .Includes(a => a.VariableAlarms, t => t.Variable)
           .Includes(a => a.VariableHiss, t => t.Variable)


            .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))

            .WhereIF(devId != 0,
            u => u.DeviceId == devId)

            .OrderBy(u => u.CreateTime).ToPagedListAsync(input.Page, input.PageSize);
        return data;
    }
    /// <summary>
    /// 增加报警属性
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public VariableAlarm AddVariableAlarm(UpdateDeviceVariableInput input)
    {
        return new();
    }
    /// <summary>
    /// 增加历史属性
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public VariableHis AddVariableHis(UpdateDeviceVariableInput input)
    {
        return new();
    }
    /// <summary>
    /// 增加扩展属性
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public VariableProperty AddVariableEx(UpdateDeviceVariableInput input)
    {
        return new();
    }
    /// <summary>
    /// 增加设备变量
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AddDeviceVariable(AddDeviceVariableInput input)
    {
        var isExist = await _deviceVariableRep.IsAnyAsync(u => u.Name == input.Name);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D9000);
        if (input.DeviceId <= 0)
            throw Oops.Oh(ErrorCodeEnum.DEV1000);

        var result = await _deviceVariableRep.Context
.InsertNav((DeviceVariable)input)
.Include(z1 => z1.VariablePropertys)// 插入第一层
.Include(a => a.VariableAlarms)
.Include(a => a.VariableHiss)
.ExecuteCommandAsync();

        var devIdName = await _deviceService.GetDeviceNames();

        var dev = await _deviceRep.AsQueryable()
.Where(it => it.Name == devIdName.FirstOrDefault(a => a.Id == input.DeviceId).Name)
.Includes(b => b.DeviceVariables, c => c.VariableAlarms, d => d.Variable)
.Includes(b => b.DeviceVariables, c => c.VariablePropertys, d => d.Variable)
.Includes(b => b.DeviceVariables, c => c.VariableHiss, d => d.Variable)
.Includes(a => a.DeviceVariables, b => b.Device)
 .Includes(b => b.DeviceProperties)
.FirstAsync();

        //这里执行动态添加设备变量
        //_deviceCollectService.UpDeviceThread(dev, false, true);
    }

    /// <summary>
    /// 更新设备变量
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateVariableConfig(UpdateDeviceVariableInput input)
    {
        var isExist = await _deviceVariableRep.IsAnyAsync(u => u.Name == input.Name && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D9000);
        if (input.DeviceId <= 0)
            throw Oops.Oh(ErrorCodeEnum.DEV1000);

        var result = await _deviceVariableRep.Context
       .UpdateNav((DeviceVariable)input)
       .Include(z1 => z1.VariablePropertys, new UpdateNavOptions() { OneToManyDeleteAll = true })// 删除脏数据
           .Include(a => a.VariableAlarms, new UpdateNavOptions() { OneToManyDeleteAll = true })
           .Include(a => a.VariableHiss, new UpdateNavOptions() { OneToManyDeleteAll = true })

       .ExecuteCommandAsync();
        if (result)
        {
            var devIdName = await _deviceService.GetDeviceNames();

            var dev = await _deviceRep.AsQueryable()
    .Where(it => it.Name == devIdName.FirstOrDefault(a => a.Id == input.DeviceId).Name)
   .Includes(b => b.DeviceVariables, a => a.VariableAlarms, c => c.Variable)
   .Includes(b => b.DeviceVariables, a => a.VariableHiss, c => c.Variable)
.Includes(b => b.DeviceVariables, c => c.VariablePropertys, d => d.Variable)
           .Includes(a => a.DeviceVariables, b => b.Device)
   .Includes(b => b.DeviceProperties)
    .FirstAsync();
            //_deviceCollectService.UpDeviceThread(dev, false, true);

        }
        else
        {
            throw Oops.Oh(ErrorCodeEnum.Z5000);
        }
    }


    /// <summary>
    /// 删除设备变量
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteVariableConfig(DeleteDeviceVariableInput input)
    {
        var deviceVariable = await _deviceVariableRep.GetFirstAsync(u => u.Id == input.Id);
        var config = await _deviceVariableRep.Context.DeleteNav<DeviceVariable>(
it => it.Id == deviceVariable.Id)
.Include(z1 => z1.VariablePropertys)
           .Include(a => a.VariableAlarms)
           .Include(a => a.VariableHiss)
.ExecuteCommandAsync();
        if (config)
        {
            var devIdName = await _deviceService.GetDeviceNames();

            var dev = await _deviceRep.AsQueryable()
    .Where(it => it.Name == devIdName.FirstOrDefault(a => a.Id == deviceVariable.DeviceId).Name)
   .Includes(b => b.DeviceProperties)
   .Includes(b => b.DeviceVariables)
    .FirstAsync();

            //_deviceCollectService.UpDeviceThread(dev, false, true);

        }
        else
        {
            throw Oops.Oh(ErrorCodeEnum.Z5000);

        }


    }



    /// <summary>
    /// 导出设备变量表
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ExportFile(DeviceVariableExportInput input)
    {
        var devIdName = await _deviceService.GetDeviceNames();
        long devId = 0;
        if (!string.IsNullOrWhiteSpace(input.DeviceName?.Trim()))
        {
            devId = devIdName.FirstOrDefault(it => it.Name.Contains(input.DeviceName))?.Id ?? -1;
        }
        var list = await _deviceVariableRep.AsQueryable()
           .Includes(a => a.VariablePropertys, t => t.Variable)
           .Includes(a => a.VariableAlarms, t => t.Variable)
           .Includes(a => a.VariableHiss, t => t.Variable)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            .WhereIF(devId != 0,
            u => u.DeviceId == devId).ToListAsync();

        var devices = list.Adapt<List<DeviceVariableExcel>>();
        var devicePropertys = list.Select(it => it.VariableAlarms).Where(it => it != null).Adapt<List<DeviceVariableAlarmExcel>>();
        var deviceVarHiss = list.Select(it => it.VariableHiss).Where(it => it != null).Adapt<List<DeviceVariableHisExcel>>();
        var deviceVarPropertys = list.Select(it => it.VariablePropertys).Where(it => it != null).Adapt<List<DeviceVariablePropertyExcel>>();

        devices.ForEach(it =>
        {
            it.DeviceName = devIdName.FirstOrDefault(a => a.Id == it.DeviceId)?.Name;
        }
        );
        var variableIdName = list.Select(it =>
          {
              return new { Id = it.Id, Name = it.Name };
          });
        devicePropertys.ForEach(it =>
        {
            if (it != null)
            {
                var data = variableIdName.FirstOrDefault(a => a.Id == it.VariableId);
                if (data != null)
                    it.VariableName = data.Name;
            }

        }

);
        deviceVarHiss.ForEach(it =>
        {
            if (it != null)
            {
                var data = variableIdName.FirstOrDefault(a => a.Id == it.VariableId);
                if (data != null)
                    it.VariableName = data.Name;
            }
        }

);
        deviceVarPropertys.ForEach(it =>
        {
            if (it != null)
            {
                var data = variableIdName.FirstOrDefault(a => a.Id == it.VariableId);
                if (data != null)
                    it.VariableName = data.Name;
            }
        }

);
        var exporter = new ExcelExporter();
        var result = await exporter
        .Append(devices)
        .SeparateBySheet()
        .Append(devicePropertys)
        .SeparateBySheet()
        .Append(deviceVarHiss)
        .SeparateBySheet()
        .Append(deviceVarPropertys)
    .ExportAppendDataAsByteArray();
        var fs = new MemoryStream(result);
        return new FileStreamResult(fs, "application/octet-stream") { FileDownloadName = DateTime.Now.ToString("yyyyMMddHHmmss") + "设备变量.xlsx" };
    }

    /// <summary>
    /// 导入设备(文件流)
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task ImprotFile([Required] IFormFile file)
    {
        if (file == null) throw Oops.Oh(ErrorCodeEnum.D8000);

        if (!_uploadOptions.ContentType.Contains(file.ContentType))
            throw Oops.Oh(ErrorCodeEnum.D8001);

        var sizeKb = (long)(file.Length / 1024.0); // 大小KB
        if (sizeKb > _uploadOptions.MaxSize)
            throw Oops.Oh(ErrorCodeEnum.D8002);

        //var suffix = Path.GetExtension(file.FileName).ToLower(); // 后缀
        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        ExcelImporter Importer = new ExcelImporter();
        //获取到的导入结果为一个字典类型，Key为Sheet名，Value为Sheet对应的数据
        var importDic = await Importer.ImportMultipleSheet<ImportDeviceVariableExcelExcel>(stream);
        //遍历字典，获取每个Sheet的数据
        List<DeviceVariable> devvars = new();
        List<DeviceVariable> adddevvars = new();
        List<DeviceVariable> updevvars = new();
        List<DeviceVariableAlarmExcel> devpros = new();
        List<DeviceVariableHisExcel> devhiss = new();
        List<DeviceVariablePropertyExcel> devvarpros = new();
        foreach (var item in importDic)
        {
            if (item.Value.HasError)
            {
                throw new(item.Value.TemplateErrors.ToJsonString() + item.Value.RowErrors.ToJsonString());
            }
            var import = item.Value;
            //导入的Sheet数据，
            if (item.Key == "设备变量表")
            {
                //多个不同类型的Sheet返回的值为object，需要进行类型转换
                devvars = import.Data.Adapt<List<DeviceVariable>>();

            }
            if (item.Key == "设备变量报警表")
            {
                devpros = import.Data.Adapt<List<DeviceVariableAlarmExcel>>();
            }
            if (item.Key == "设备变量历史表")
            {
                devhiss = import.Data.Adapt<List<DeviceVariableHisExcel>>();
            }
            if (item.Key == "设备变量扩展域表")
            {
                devvarpros = import.Data.Adapt<List<DeviceVariablePropertyExcel>>();
            }

        }
        var devpropertys = devpros;
        var devvarhiss = devhiss;

        var devIdName = await _deviceService.GetDeviceNames();

        var names = devvars.Select(a => a.Name);
        var olddatas = await _deviceVariableRep.AsQueryable()
            .Includes(it => it.VariableAlarms)
            .Includes(it => it.VariableHiss)
            .Where(it => names.Contains(it.Name))
           .ToListAsync();


        devvars.ForEach(its =>
        {
            its.DeviceId = devIdName.FirstOrDefault(a => a.Name == its.DeviceName)?.Id ?? 0;
            var olddev = olddatas.Where(it => it.Name == its.Name).FirstOrDefault();
            if (olddev == null)
            {
                adddevvars.Add(its);
            }
            else
            {
                its.Id = olddev.Id;
                updevvars.Add(its);
            }
            devpropertys.ForEach(it =>
            {
                if (its.Name == it.VariableName)
                {
                    var data = it.Adapt<VariableAlarm>();
                    data.VariableId = its.Id;
                    if (olddev?.VariableAlarms != null)
                    {
                        data.Id = olddev.VariableAlarms.Id;
                    }
                    else
                    {
                        var codeId = Yitter.IdGenerator.YitIdHelper.NextId();
                        data.Id = codeId;
                    }
                    its.VariableAlarms = data;
                }
            });

            devvarhiss.ForEach(it =>
            {
                if (its.Name == it.VariableName)
                {
                    var data = it.Adapt<VariableHis>();
                    data.VariableId = its.Id;
                    if (olddev?.VariableHiss != null)
                    {
                        data.Id = olddev.VariableHiss.Id;
                    }
                    else
                    {
                        var codeId = Yitter.IdGenerator.YitIdHelper.NextId();
                        data.Id = codeId;
                    }
                    its.VariableHiss = data;
                }
            });
            devvarpros.ForEach(it =>
            {
                if (its.Name == it.VariableName)
                {
                    var data = it.Adapt<VariableProperty>();
                    data.VariableId = its.Id;
                    if (olddev?.VariablePropertys != null)
                    {
                        data.Id = olddev.VariablePropertys.Id;
                    }
                    else
                    {
                        var codeId = Yitter.IdGenerator.YitIdHelper.NextId();
                        data.Id = codeId;
                    }
                    its.VariablePropertys = data;
                }
            });

        });

        var re1 = await _deviceVariableRep.Context
                       .UpdateNav(updevvars)
           .Include(z1 => z1.VariablePropertys)// 删除脏数据
           .Include(z1 => z1.VariableAlarms)// 删除脏数据
           .Include(z1 => z1.VariableHiss)// 删除脏数据
            .ExecuteCommandAsync();
        var re2 = await _deviceVariableRep.Context
           .InsertNav(adddevvars)
        .Include(z1 => z1.VariablePropertys)
        .Include(z1 => z1.VariableAlarms)
        .Include(z1 => z1.VariableHiss)
        .ExecuteCommandAsync();

        var devs = await _deviceRep.AsQueryable()
            .Where(it => devvars.Select(a => a.DeviceId).Contains(it.Id))
               .Includes(b => b.DeviceVariables, c => c.VariablePropertys, d => d.Variable)
               .Includes(b => b.DeviceVariables, c => c.VariableAlarms, d => d.Variable)
               .Includes(b => b.DeviceVariables, c => c.VariableHiss, d => d.Variable)
               .Includes(b => b.DeviceProperties)
               .ToListAsync()
               ;
        foreach (Device item in devs)
        {
            //_deviceCollectService.UpDeviceThread(item, false, true);

        }
    }


}