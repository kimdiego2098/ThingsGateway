export const JobScriptCode = `namespace ThingsGateway.Core;

/// <summary>
/// XXX作业任务
/// </summary>
[JobDetail("job_XXX", Description = "XXX", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "trigger_XXX", Description = "XXX")]
public class XXXJob : IJob
{
    private readonly IServiceProvider _serviceProvider;

    public XXXJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        var db = serviceScope.ServiceProvider.GetService<ISqlSugarClient>();

        自己的逻辑代码。。。

    }
}`;
