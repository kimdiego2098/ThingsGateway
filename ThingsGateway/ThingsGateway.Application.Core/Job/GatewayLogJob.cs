using Furion.Schedule;


namespace ThingsGateway.Application.Core;

/// <summary>
/// 清理网关日志作业任务
/// </summary>
[JobDetail("job_gatewaylog", Description = "清理网关日志", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "trigger_gatewaylog", Description = "清理网关日志", RunOnStart = true)]
public class GatewayLogJob : IJob
{
    private readonly IServiceProvider _serviceProvider;

    public GatewayLogJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        var db = serviceScope.ServiceProvider.GetService<ISqlSugarClient>().CopyNew();

        var daysAgo = 10; // 删除10天以前
        await db.Deleteable<GatewayLogOp>().Where(u => (DateTime)u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(); // 删除网关日志
    }
}