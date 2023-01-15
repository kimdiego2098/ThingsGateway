namespace ThingsGateway.Core;

/// <summary>
/// 日志事件订阅
/// </summary>
public class LogEventSubscriber : IEventSubscriber, ISingleton
{
    private readonly IServiceProvider _serviceProvider;

    public LogEventSubscriber(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 增加操作日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe("Add:OpLog")]
    public async Task CreateOpLog(EventHandlerExecutingContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var _rep = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogOp>>();
        await _rep.InsertAsync((SysLogOp)context.Source.Payload);
    }

    /// <summary>
    /// 增加异常日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe("Add:ExLog")]
    public async Task CreateExLog(EventHandlerExecutingContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var _rep = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogEx>>();
        await _rep.InsertAsync((SysLogEx)context.Source.Payload);

        // 发送邮件
        await scope.ServiceProvider.GetRequiredService<SysMessageService>().SendEmail(JSON.Serialize(context.Source.Payload));
    }

    /// <summary>
    /// 增加访问日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe("Add:VisLog")]
    public async Task CreateVisLog(EventHandlerExecutingContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var _rep = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogVis>>();
        await _rep.InsertAsync((SysLogVis)context.Source.Payload);
    }
}