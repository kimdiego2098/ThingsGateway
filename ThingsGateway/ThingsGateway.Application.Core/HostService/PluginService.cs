

using Masuit.Tools;

using Microsoft.Extensions.Logging;

namespace ThingsGateway.Application.Core;
public class PluginAssemble
{

    public string AssembleName { get; set; }
    public Type Type { get; set; }

}
/// <summary>
/// 驱动插件信息
/// </summary>
public class PluginInfo
{

    /// <summary>
    /// 驱动文件全路径
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 插件程序集名与Type的字典
    /// </summary>
    public List<PluginAssemble> PluginAssemble { get; set; }
    /// <summary>
    /// 驱动文件名称
    /// </summary>
    public string PluginName { get; set; }

}

/// <summary>
/// 驱动插件服务
/// </summary>
public class PluginService : ISingleton
{

    private readonly SqlSugarRepository<DriverPlugin> _driverPluginRep;
    private readonly ILogger<PluginService> _logger;
    private readonly SqlSugarRepository<UploadPlugin> _upLoadPluginRep;
    private IServiceProvider _serviceProvider;
    public PluginService(ILogger<PluginService> logger,
        SysCacheService sysCacheService, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider.CreateScope().ServiceProvider;
        _logger = logger;
        _driverPluginRep = _serviceProvider.GetService<SqlSugarRepository<DriverPlugin>>();
        _upLoadPluginRep = _serviceProvider.GetService<SqlSugarRepository<UploadPlugin>>();

        LoadAllDrivers();
        LoadAllUploads();
    }

    public List<PluginInfo> DriverInfos { get; private set; } = new List<PluginInfo>();
    public List<PluginInfo> UploadInfos { get; private set; } = new List<PluginInfo>();

    public bool AddDriver(DriverPlugin plugin)
    {
        var driverFiles = plugin.FileName;
        PluginInfo driverInfo = new PluginInfo
        {
            FileName = driverFiles,
            PluginAssemble = new(),
            PluginName = plugin.PluginName,
        };
        _logger?.LogInformation($"添加驱动插件文件：{driverFiles}");
        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), plugin.FileName);
        var dll = Assembly.LoadFrom(path);
        if (dll != null)
        {

            foreach (var type in dll.GetTypes().Where(x => typeof(DriverBase).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract))
            {
                driverInfo.PluginAssemble.Add(new() { AssembleName = type.Name, Type = type });
                _logger?.LogInformation($"加载驱动插件 {driverInfo.FileName}-{type.Name} 成功");
            }
            DriverInfos.Add(driverInfo);
        }
        var num = driverInfo.PluginAssemble.Count();
        _logger?.LogInformation($"加载驱动插件完成 ,数量 {num}");
        if (num > 0)
            return true;
        else
            return false;
    }
    public bool AddUpload(UploadPlugin plugin)
    {
        var uploadFiles = plugin.FileName;
        PluginInfo uploadInfo = new PluginInfo
        {
            FileName = uploadFiles,
            PluginAssemble = new(),
            PluginName = plugin.PluginName,
        };
        _logger?.LogInformation($"添加上传插件文件：{uploadFiles}");
        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), plugin.FileName);
        var dll = Assembly.LoadFrom(path);
        if (dll != null)
        {

            foreach (var type in dll.GetTypes().Where(x => typeof(UpLoadBase).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract))
            {
                uploadInfo.PluginAssemble.Add(new() { AssembleName = type.Name, Type = type });
                _logger?.LogInformation($"加载上传插件 {uploadInfo.FileName}-{type.Name} 成功");
            }
            UploadInfos.Add(uploadInfo);
        }
        var num = uploadInfo.PluginAssemble.Count();
        _logger?.LogInformation($"加载上传插件完成 ,数量 {num}");
        if (num > 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 反射构造驱动插件
    /// </summary>
    public DriverBase CreateDriver(Type type, ILogger logger)
    {
        if (logger == null)
        {
            logger = _logger;
        }
        object[] param = new object[] { logger };
        //反射构造驱动插件
        DriverBase driver = Activator.CreateInstance(type, param) as DriverBase;
        return driver;
    }

    /// <summary>
    /// 反射构造上传插件
    /// </summary>
    public UpLoadBase CreateUpload(Type type, ILogger logger)
    {
        if (logger == null)
        {
            logger = _logger;
        }
        object[] param = new object[] { logger, _serviceProvider.CreateScope().ServiceProvider };
        //反射构造上传插件
        UpLoadBase driver = Activator.CreateInstance(type, param) as UpLoadBase;
        return driver;
    }

    public void DeleteDriver(DriverPlugin plugin)
    {
        var driverFiles = plugin.FileName;
        DriverInfos.RemoveWhere(it => it.FileName == driverFiles);
    }
    public void DeleteUpload(UploadPlugin plugin)
    {
        var uploadFiles = plugin.FileName;
        UploadInfos.RemoveWhere(it => it.FileName == uploadFiles);
    }

    public PluginAssemble GetDriverAssemble(string driverAssembleName)
    {
        var driver = DriverInfos.
        SelectMany(it => it.PluginAssemble).
        FirstOrDefault(it =>
            it.AssembleName == driverAssembleName
                    );

        if (driver == null)
        {
            throw new($"找不到设备驱动:[{driverAssembleName}]");
        }
        return driver;
    }

    /// <summary>
    /// 获取驱动插件的属性值
    /// </summary>
    public List<DeviceProperty> GetDriverProperties(DriverBase driver)
    {
        var data = driver.GetType().GetProperties().SelectMany(it =>
            new[]
            {
                new {property= it, devicePropertyAttribute= it.GetCustomAttribute<DevicePropertyAttribute>() },
        })
            .Where(x => x.devicePropertyAttribute != null)
        .ToList()
              .SelectMany(it => new[]
              {
                  new DeviceProperty(){
                      DevicePropertyName=it.property.Name,
                      Description=it.devicePropertyAttribute.Name,
                      Remark=it.devicePropertyAttribute.Remark,
                      Value=it.property.GetValue(driver)?.ToString(),
                  }
              });
        return data.ToList();
    }

    /// <summary>
    /// 获取驱动插件的属性值
    /// </summary>
    public List<UploadDeviceProperty> GetUploadProperties(UpLoadBase driver)
    {
        var data = driver.GetType().GetProperties().SelectMany(it =>
            new[]
            {
                new {property= it, devicePropertyAttribute= it.GetCustomAttribute<DevicePropertyAttribute>() },
        })
            .Where(x => x.devicePropertyAttribute != null)
        .ToList()
              .SelectMany(it => new[]
              {
                  new UploadDeviceProperty(){
                      UploadDevicePropertyName=it.property.Name,
                      Description=it.devicePropertyAttribute.Name,
                      Remark=it.devicePropertyAttribute.Remark,
                      Value=it.property.GetValue(driver)?.ToString(),
                  }
              });
        return data.ToList();
    }


    public List<MethodInfo> GetMethod(DriverBase driver)
    {
        return driver.GetType().GetMethods().Where(
               x => x.GetCustomAttribute(typeof(MethodAttribute)) != null).ToList();
    }
    public PluginAssemble GetUploadAssemble(string uploadAssembleName)
    {
        var driver = UploadInfos.
        SelectMany(it => it.PluginAssemble).
        FirstOrDefault(it =>
            it.AssembleName == uploadAssembleName
                    );

        if (driver == null)
        {
            throw new($"找不到设备驱动:[{uploadAssembleName}]");
        }
        return driver;
    }

    private void LoadAllDrivers()
    {
        _logger?.LogInformation("正在加载驱动插件");
        var driverFiles = _driverPluginRep.AsQueryable().ToList();
        if (driverFiles.Count > 0)
        {
            _logger?.LogInformation($"发现驱动插件文件，数量{driverFiles.Count}");
            foreach (var plugin in driverFiles)
            {
                try
                {
                    var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), plugin.FileName);
                    var dll = Assembly.LoadFrom(path);
                    if (dll != null)
                    {
                        PluginInfo driverInfo = new PluginInfo
                        {
                            FileName = plugin.FileName,
                            PluginAssemble = new(),
                            PluginName = plugin.PluginName,
                        };
                        foreach (var type in dll.GetTypes().Where(x => typeof(DriverBase).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract))
                        {
                            driverInfo.PluginAssemble.Add(new() { AssembleName = type.Name, Type = type });
                            _logger?.LogInformation($"加载驱动插件 {driverInfo.FileName}-{type.Name} 成功");
                        }
                        DriverInfos.Add(driverInfo);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"加载驱动插件失败 {ex}");
                }
            }
        }
        _logger?.LogInformation($"加载驱动插件完成 ,数量 {DriverInfos.SelectMany(it => it.PluginAssemble).Count()}");
    }
    private void LoadAllUploads()
    {
        _logger?.LogInformation("正在加载上传插件");
        var upLoadFiles = _upLoadPluginRep.GetList();
        if (upLoadFiles.Count > 0)
        {
            _logger?.LogInformation($"发现上传插件文件，数量{upLoadFiles.Count}");
            foreach (var plugin in upLoadFiles)
            {
                try
                {
                    var path = Path.Combine(App.HostEnvironment.ContentRootPath, plugin.FileName);
                    var dll = Assembly.LoadFrom(path);
                    if (dll != null)
                    {
                        PluginInfo upLoadInfo = new PluginInfo
                        {
                            FileName = plugin.FileName,
                            PluginAssemble = new(),
                            PluginName = plugin.PluginName,
                        };
                        foreach (var type in dll.GetTypes().Where(x => typeof(UpLoadBase).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract))
                        {
                            upLoadInfo.PluginAssemble.Add(new() { AssembleName = type.Name, Type = type });
                            _logger?.LogInformation($"加载上传插件 {upLoadInfo.FileName}-{type.Name} 成功");
                        }
                        UploadInfos.Add(upLoadInfo);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"加载上传插件失败 {ex}");
                }
            }
        }

        _logger?.LogInformation($"加载上传插件完成 ,数量 {UploadInfos.SelectMany(it => it.PluginAssemble).Count()}");
    }

}