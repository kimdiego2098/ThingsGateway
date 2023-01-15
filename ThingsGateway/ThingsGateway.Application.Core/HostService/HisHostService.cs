using AngleSharp.Text;

using Furion.DataEncryption;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Application.Core;

/// <summary>
/// 设备采集后台历史服务
/// </summary>
public class HisHostService : BackgroundService, ISingleton
{
    private readonly ILogger<HisHostService> _logger;
    /// <summary>
    /// 全局设备信息
    /// </summary>
    private AllDeviceData _allDeviceData;
    public static SqlSugarScope _SqlSugarScope;
    public int IsHisConfigChange = 1;
    private IntelligentConcurrentQueue<DeviceVariable> CollectDeviceVariables { get; set; } = new(50000);
    private IntelligentConcurrentQueue<DeviceVariable> ChangeDeviceVariables { get; set; } = new(50000);
    private ISqlSugarClient _hisConfigRep;
    private IServiceProvider _serviceProvider;
    public HisHostService(ILogger<HisHostService> logger, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider.CreateScope().ServiceProvider;

        _logger = logger;


        _allDeviceData = _serviceProvider.GetService<AllDeviceData>();
        _hisConfigRep = _serviceProvider.GetService<ISqlSugarClient>();

    }
    private SqlSugarScope HisConfig(HisConfig hisConfig)
    {
        hisConfig.ConnStr = DESCEncryption.Decrypt(hisConfig.ConnStr, ApplicationInfo.DESCKey);

        //多库情况下使用说明：
        //如果是固定多库可以传 new SqlSugarScope(List<ConnectionConfig>,db=>{}) 文档：多租户
        //如果是不固定多库 可以看文档Saas分库
        var configureExternalServices = new ConfigureExternalServices
        {
            EntityService = (type, column) => // 修改列可空-1、带?问号 2、String类型若没有Required
            {
                if ((type.PropertyType.IsGenericType && type.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    || (type.PropertyType == typeof(string) && type.GetCustomAttribute<RequiredAttribute>() == null))
                    column.IsNullable = true;
            },
            DataInfoCacheService = new SqlSugarCache(),
        };
        //用单例模式
        var sqlSugarScope = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = hisConfig.ConnStr,//连接字符串
            DbType = hisConfig.DbType.ToString().ToEnum<DbType>(DbType.SqlServer),//数据库类型
            IsAutoCloseConnection = true, //不设成true要手动close
            ConfigureExternalServices = configureExternalServices
        },
    db =>
    {
        db.Aop.OnError = (ex) =>
        {
            if (ex.Parametres == null) return;
            Console.ForegroundColor = ConsoleColor.Red;
            var pars = db.Utilities.SerializeObject(((SugarParameter[])ex.Parametres).ToDictionary(it => it.ParameterName, it => it.Value));
            _logger?.LogError("【" + DateTime.Now + "历史服务——错误SQL】\r\n" + UtilMethods.GetSqlString(hisConfig.DbType.Adapt<DbType>(), ex.Sql, (SugarParameter[])ex.Parametres) + Environment.NewLine);
        };
    });
        return sqlSugarScope;
    }
    #region worker服务
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_allDeviceData.Devices.DeviceChange == null)
        {
            _allDeviceData.Devices.DeviceChange = DeviceChange;
        }
        else
        {
            _allDeviceData.Devices.DeviceChange += DeviceChange;
        }
        _logger?.LogInformation("历史服务启动");
        await base.StartAsync(cancellationToken);
    }

    private void DeviceChange(Device device)
    {
        device.DeviceVariables?.ForEach(v => { v.VariableCollectChange += DeviceVariableCollectChange; });
        device.DeviceVariables?.ForEach(v => { v.VariableValueChange += DeviceVariableValueChange; });
    }

    private void DeviceVariableCollectChange(DeviceVariable variable)
    {
        if (variable.VariableHiss?.HisType == HisType.Collect)
        {
            CollectDeviceVariables.Enqueue(variable);
        }
    }
    private void DeviceVariableValueChange(DeviceVariable variable)
    {
        if (variable.VariableHiss?.HisType == HisType.Change)
        {
            ChangeDeviceVariables.Enqueue(variable);
        }
    }
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation("历史服务停止");
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);
        _logger?.LogInformation($"历史数据线程启动");
        HisConfig config = new();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1000, stoppingToken);
                try
                {

                    if (Interlocked.CompareExchange(ref IsHisConfigChange, 0, 1) == 1)
                    {
                        config = await _hisConfigRep.AsTenant()?.GetConnectionWithAttr<HisConfig>().CopyNew().Queryable<HisConfig>()?.FirstAsync();
                        if (config?.Enable == true)
                        {
                            _SqlSugarScope = HisConfig(config);
                            /***创建/更新单个表***/
                            _SqlSugarScope.CodeFirst.InitTables(typeof(HisValue));
                            await _SqlSugarScope.Queryable<HisValue>().FirstAsync();
                        }

                    }
                }
                catch (TaskCanceledException)
                {

                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"历史存储数据库链接对象初始化异常");
                    Interlocked.CompareExchange(ref IsHisConfigChange, 1, 0);

                }

                //这里直接出队，没做失败重试，后续添加
                var list = CollectDeviceVariables.ToListWithDequeue(CollectDeviceVariables.Count);
                var changelist = ChangeDeviceVariables.ToListWithDequeue(ChangeDeviceVariables.Count);
                if (!config?.Enable == true || _SqlSugarScope == null) continue;
                await _SqlSugarScope.Queryable<HisValue>().FirstAsync();
                if (list.Count != 0)
                {
                    ////Sql保存
                    var collecthis = list.Adapt<List<HisValue>>();
                    //插入
                    await _SqlSugarScope.Insertable<HisValue>(collecthis).ExecuteCommandAsync();
                }

                if (changelist.Count != 0)
                {
                    ////Sql保存
                    var changehis = changelist.Adapt<List<HisValue>>();
                    //插入
                    await _SqlSugarScope.Insertable<HisValue>(changehis).ExecuteCommandAsync();

                }

            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"历史数据线程循环异常");
            }
        }



    }


    #endregion


}
public class HisValueMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<DeviceVariable, HisValue>()
            .Map(dest => dest.Value, (src) => ValueReturn(src));
    }

    private static object ValueReturn(DeviceVariable src)
    {
        if (src.DataType == typeof(bool))
        {
            if (src.Value.ToString().ToUpper() == "FALSE" || src.Value.ToString().ToUpper() == "0")
            {
                return 0;
            }
            else { return 1; }
        }
        else
        {
            return src.Value;
        }
    }
}