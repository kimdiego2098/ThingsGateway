namespace ThingsGateway.Core;

/// <summary>
/// 清理日志作业任务
/// </summary>
[JobDetail("job_log", Description = "清理操作日志", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "trigger_log", Description = "清理操作日志")]
public class LogJob : IJob
{
    private readonly IServiceProvider _serviceProvider;

    public LogJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        var db = serviceScope.ServiceProvider.GetService<ISqlSugarClient>();

        var daysAgo = 30; // 删除30天以前
        await db.Deleteable<SysLogVis>().Where(u => (DateTime)u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(); // 删除访问日志
        await db.Deleteable<SysLogOp>().Where(u => (DateTime)u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(); // 删除操作日志
        await db.Deleteable<SysLogEx>().Where(u => (DateTime)u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(); // 删除异常日志
        await db.Deleteable<SysLogDiff>().Where(u => (DateTime)u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(); // 删除差异日志
    }
}