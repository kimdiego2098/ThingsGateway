namespace ThingsGateway.Core;

/// <summary>
/// 清理在线用户作业任务
/// </summary>
[JobDetail("job_onlineUser", Description = "清理在线用户", GroupName = "default", Concurrent = false)]
[PeriodSeconds(1, TriggerId = "trigger_onlineUser", Description = "清理在线用户", MaxNumberOfRuns = 1, RunOnStart = true)]
public class OnlineUserJob : IJob
{
    private readonly IServiceProvider _serviceProvider;

    public OnlineUserJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _serviceProvider.CreateScope();

        var rep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<SysOnlineUser>>();
        await rep.AsDeleteable().ExecuteCommandAsync();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("【" + DateTime.Now + "】服务重启清空在线用户");

        // 缓存多租户
        await serviceScope.ServiceProvider.GetService<SysTenantService>().UpdateTenantCache();
    }
}