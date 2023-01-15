using AngleSharp.Text;

using CodingSeb.ExpressionEvaluator;

using Furion.DataEncryption;
using Furion.Logging.Extensions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MQTTnet;
using MQTTnet.Server;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Application.Core;

/// <summary>
/// 设备采集报警后台服务
/// </summary>
public class AlarmHostService : BackgroundService, ISingleton
{

    private readonly ILogger<AlarmHostService> _logger;
    /// <summary>
    /// 全局设备信息
    /// </summary>
    private AllDeviceData _allDeviceData;
    public static SqlSugarScope _SqlSugarScope;
    public int IsAlarmConfigChange = 1;
    private IntelligentConcurrentQueue<DeviceVariable> CollectDeviceVariables { get; set; } = new(10000);
    public ConcurrentList<DeviceVariable> RealAlarmDeviceVariables { get; set; } = new(10000);
    private IntelligentConcurrentQueue<DeviceVariable> HisAlarmDeviceVariables { get; set; } = new(10000);


    private ISqlSugarClient _alarmConfigRep;

    public event DelegateOnAlarmChanged OnAlarmChanged;
    public event DelegateOnDeviceStatusChanged OnDeviceStatusChanged;

    private MqttServer _mqttServer;
    private IServiceProvider _serviceProvider;
    public AlarmHostService(ILogger<AlarmHostService> logger, MqttServer mqttServer,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider.CreateScope().ServiceProvider;
        _logger = logger;
        _mqttServer = mqttServer;
        _allDeviceData = _serviceProvider.GetService<AllDeviceData>();

    }
    private SqlSugarScope AlarmConfig(AlarmConfig alarmConfig)
    {
        alarmConfig.ConnStr = DESCEncryption.Decrypt(alarmConfig.ConnStr, ApplicationInfo.DESCKey);

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
            ConnectionString = alarmConfig.ConnStr,//连接字符串
            DbType = alarmConfig.DbType.ToString().ToEnum<DbType>(DbType.SqlServer),//数据库类型
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
            _logger?.LogError("【" + DateTime.Now + "报警服务——错误SQL】\r\n" + UtilMethods.GetSqlString(alarmConfig.DbType.Adapt<DbType>(), ex.Sql, (SugarParameter[])ex.Parametres) + Environment.NewLine);
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
        _logger?.LogInformation("报警服务启动");
        await base.StartAsync(cancellationToken);
    }

    private void DeviceChange(Device device)
    {
        device.DeviceVariables?.ForEach(v => { v.VariableCollectIntelnalChange += DeviceVariableChange; });
        device.DeviceStatus.DeviceStatusCahnge += DeviceStatus_DeviceStatusCahnge;
    }

    private void DeviceStatus_DeviceStatusCahnge(Device device)
    {
        OnDeviceStatusChanged?.Invoke(device);
    }

    private void DeviceVariableChange(DeviceVariable variable)
    {
        CollectDeviceVariables.Enqueue(variable);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation("报警服务停止");
        return base.StopAsync(cancellationToken);
    }
    private ExpressionEvaluator expressionEvaluator;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);

        //MQTT推送报警
        new Task(async () =>
       {
           _logger?.LogInformation($"开始推送报警");
           //驱动插件执行循环前方法
           while (!stoppingToken.IsCancellationRequested)
           {
               try
               {
                   await Task.Delay(1000, stoppingToken);
                   JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
                   {
                       ContractResolver = new CamelCasePropertyNamesContractResolver()
                   };
                   var list = RealAlarmDeviceVariables.ToList();
                   ////变化推送
                   var variableMessage = new MqttApplicationMessageBuilder()
                   .WithTopic($"device/alarmstatus")
                   .WithPayload(list.ToJsonStringWithSettings(serializerSettings)).Build();
                   await _mqttServer.InjectApplicationMessage(
                           new InjectedMqttApplicationMessage(variableMessage)
                           {
                               SenderClientId = "thingsgateway"
                           });
               }
               catch (TaskCanceledException)
               {

               }
               catch (Exception ex)
               {
                   _logger?.LogError(ex, $"报警推送线程循环异常");
               }
           }
       }
, stoppingToken, TaskCreationOptions.LongRunning
).Start();
        //历史报警保存，后面单独做成worker
        new Task(async () =>
       {
           _alarmConfigRep = _serviceProvider.GetService<ISqlSugarClient>();
           _logger?.LogInformation($"历史报警线程启动");
           AlarmConfig config = new();
           //驱动插件执行循环前方法
           while (!stoppingToken.IsCancellationRequested)
           {
               try
               {
                   await Task.Delay(1000, stoppingToken);
                   try
                   {
                       if (Interlocked.CompareExchange(ref IsAlarmConfigChange, 0, 1) == 1)
                       {
                           config = await _alarmConfigRep.AsTenant()?.GetConnectionWithAttr<AlarmConfig>().CopyNew().Queryable<AlarmConfig>().FirstAsync();
                           if (config?.Enable == true)
                           {
                               _SqlSugarScope = AlarmConfig(config);
                               /***创建/更新单个表***/
                               if (!_SqlSugarScope.Ado.IsValidConnection()) throw new("数据库测试连接失败");
                               _SqlSugarScope.CodeFirst.InitTables(typeof(AlarmHis));
                           }

                       }
                   }
                   catch (Exception ex)
                   {
                       _logger?.LogError(ex, $"历史报警Sql链接对象初始化异常");
                       Interlocked.CompareExchange(ref IsAlarmConfigChange, 1, 0);
                   }

                   var list = HisAlarmDeviceVariables.ToListWithDequeue(500);
                   if (list.Count == 0) continue;
                   if (!config?.Enable == true || _SqlSugarScope == null) continue;
                   if (!_SqlSugarScope.Ado.IsValidConnection()) throw new("数据库测试连接失败");
                   ////Sql保存
                   var hisalarm = list.Adapt<List<AlarmHis>>();
                   hisalarm.ForEach(it =>
                   {
                       it.Id = Yitter.IdGenerator.YitIdHelper.NextId();
                       it.VariableAlarmsMachineIP = ApplicationInfo.AppIp;
                   }
                       );

                   //插入
                   await _SqlSugarScope.Insertable(hisalarm).ExecuteCommandAsync();

               }
               catch (TaskCanceledException)
               {

               }
               catch (Exception ex)
               {
                   _logger?.LogError(ex, $"历史报警线程循环异常");
               }
           }
       }
, stoppingToken, TaskCreationOptions.LongRunning
).Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            //报警服务,这里不单独做报警服务，因为在线组态会导致设备线程经常启停
            try
            {
                await Task.Delay(1000, stoppingToken);
                if (expressionEvaluator == null)
                {
                    expressionEvaluator = new();
                    expressionEvaluator.PreEvaluateVariable += Evaluator_PreEvaluateVariable;
                }
                var list = CollectDeviceVariables.ToListWithDequeue(1000);
                foreach (var item in list)
                {
                    if (item.VariableAlarms == null) continue;
                    AlarmAnalysis(item);
                }

            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"报警线程循环异常");
            }
        }
    }

    private void AlarmAnalysis(DeviceVariable item)
    {
        var varAlarm = item.VariableAlarms;
        string limit = string.Empty;
        string ex = string.Empty;

        AlarmEnum alarmEnum = AlarmEnum.None;

        if (item.Value.GetType() == typeof(bool))
        {
            alarmEnum = GetBoolAlarmCode(item.VariableAlarms, item.Value, out limit, out ex);
        }
        else
        {
            alarmEnum = GetDecimalAlarmDegree(item.VariableAlarms, item.Value, out limit, out ex);
        }
        if (alarmEnum == AlarmEnum.None)
        {
            //这里需恢复报警，如果存在的话
            AlarmChange(item, null, EventEnum.Finish, alarmEnum);

        }
        else
        {
            //这里需更新报警，不管是否存在
            if (!ex.IsNullOrEmpty())
            {
                var data = expressionEvaluator.GetExpressionsResult(ex, item.Value);
                if (data is bool result)
                {
                    if (result)
                    {
                        AlarmChange(item, limit, EventEnum.Alarm, alarmEnum);
                    }
                }
            }
            else
            {
                AlarmChange(item, limit, EventEnum.Alarm, alarmEnum);
            }

        }
    }

    /// <summary>
    /// 获取value报警类型
    /// </summary>
    private AlarmEnum GetDecimalAlarmDegree(VariableAlarm tag, object value, out string limit, out string expressions)
    {
        limit = string.Empty;
        expressions = string.Empty;

        if (tag.HHAlarmEnable && value.ToDecimal() > tag.HHAlarmCode.ToDecimal())
        {
            limit = tag.HHAlarmCode.ToString();
            expressions = tag.HHRestrainExpressions;
            return AlarmEnum.HH;
        }

        if (tag.HAlarmEnable && value.ToDecimal() > tag.HAlarmCode.ToDecimal())
        {
            limit = tag.HAlarmCode.ToString();
            expressions = tag.HRestrainExpressions;
            return AlarmEnum.H;
        }

        if (tag.LAlarmEnable && value.ToDecimal() < tag.LAlarmCode.ToDecimal())
        {
            limit = tag.LAlarmCode.ToString();
            expressions = tag.LRestrainExpressions;
            return AlarmEnum.L;
        }
        if (tag.LLAlarmEnable && value.ToDecimal() < tag.LLAlarmCode.ToDecimal())
        {
            limit = tag.LLAlarmCode.ToString();
            expressions = tag.LLRestrainExpressions;
            return AlarmEnum.LL;
        }
        return AlarmEnum.None;
    }

    /// <summary>
    /// 获取bool报警类型
    /// </summary>
    private AlarmEnum GetBoolAlarmCode(VariableAlarm tag, object value, out string limit, out string expressions)
    {
        limit = string.Empty;
        expressions = string.Empty;
        if (tag.BoolCloseAlarmEnable && value.ToBoolean() == false)
        {
            limit = false.ToString();
            expressions = tag.BoolCloseRestrainExpressions;
            return AlarmEnum.Close;
        }
        if (tag.BoolOpenAlarmEnable && value.ToBoolean() == true)
        {
            limit = true.ToString();
            expressions = tag.BoolOpenRestrainExpressions;
            return AlarmEnum.Open;
        }
        return AlarmEnum.None;
    }


    private void AlarmChange(DeviceVariable item, object limit, EventEnum eventEnum, AlarmEnum alarmEnum)
    {
        if (eventEnum == EventEnum.Finish)
        {
            //实时报警没有找到的话直接返回
            if (!RealAlarmDeviceVariables.Any(it => it.Id == item.Id))
            {
                return;
            }
        }
        else if (eventEnum == EventEnum.Alarm)
        {
            var variable = RealAlarmDeviceVariables.FirstOrDefault(it => it.Id == item.Id);
            if (variable != null)
            {
                if (item.VariableAlarms.AlarmType == alarmEnum)
                    return;
            }
        }

        if (eventEnum == EventEnum.Alarm)
        {
            item.VariableAlarms.AlarmType = alarmEnum;
            item.VariableAlarms.EventType = eventEnum;
            item.VariableAlarms.AlarmLimit = limit;
            item.VariableAlarms.AlarmCode = item.Value;
            item.VariableAlarms.AlarmTime = DateTime.Now;
            item.VariableAlarms.EventTime = DateTime.Now;
        }
        else if (eventEnum == EventEnum.Finish)
        {
            var oldAlarm = RealAlarmDeviceVariables.FirstOrDefault(it => it.Id == item.Id);
            item.VariableAlarms.AlarmType = oldAlarm.VariableAlarms.AlarmType;
            item.VariableAlarms.EventType = eventEnum;
            item.VariableAlarms.AlarmLimit = oldAlarm.VariableAlarms.AlarmLimit;
            item.VariableAlarms.AlarmCode = item.Value;
            item.VariableAlarms.EventTime = DateTime.Now;
        }
        else if (eventEnum == EventEnum.Check)
        {
            item.VariableAlarms.AlarmType = alarmEnum;
            item.VariableAlarms.EventType = eventEnum;
            item.VariableAlarms.AlarmLimit = limit;
            item.VariableAlarms.AlarmCode = item.Value;
            item.VariableAlarms.EventTime = DateTime.Now;
        }
        OnAlarmChanged?.Invoke(item);

        HisAlarmDeviceVariables.Enqueue(item);

        if (eventEnum == EventEnum.Alarm)
        {
            RealAlarmDeviceVariables.RemoveWhere(it => it.Id == item.Id);
            RealAlarmDeviceVariables.Add(item);
        }
        else
        {
            RealAlarmDeviceVariables.RemoveWhere(it => it.Id == item.Id);
        }

    }

    private void Evaluator_PreEvaluateVariable(object sender, VariablePreEvaluationEventArg e)
    {
        var data = App.GetService<AllDeviceData>();
        var obj = data.DeviceVariables.FirstOrDefault(it => it.Name == e.Name);
        if (obj == null)
        {
            return;
        }
        if (obj.Value != null)
            e.Value = Convert.ChangeType(obj.Value, obj.DataType);
        else
            e.Value = null;
    }
    #endregion


}

public class ChangeIntelligentConcurrentList<T> : Foundation.Core.ConcurrentList<T> where T : Device
{
    public DeviceCahngeEventHandler DeviceChange { get; set; }

    public void AddChange(T change)
    {
        Add(change);
        DeviceChange?.Invoke(change);
    }

    public void RemoveWhereChange(Func<T, bool> where)
    {
        foreach (T item in this.Where(where).ToList())
        {
            this.Remove(item);
            DeviceChange?.Invoke(item);
        }
    }

}

