namespace ThingsGateway.Core.Service;

/// <summary>
/// 作业持久化（数据库）
/// </summary>
public class DbJobPersistence : IJobPersistence
{
    private readonly IServiceProvider _serviceProvider;

    public DbJobPersistence(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 作业调度服务启动时
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SchedulerBuilder> Preload()
    {
        using var serviceScope = _serviceProvider.CreateScope();
        var jobDetailRep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<SysJobDetail>>();
        var jobTriggerRep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<SysJobTrigger>>();

        // 获取内存的作业
        IEnumerable<SchedulerBuilder> memoryJobs = App.EffectiveTypes.ScanToBuilders();

        // 若数据库不存在任何作业，则返回内存作业
        if (!jobDetailRep.IsAny(u => true)) return memoryJobs;

        var schedulerBuilders = new List<SchedulerBuilder>();

        // 获取数据库所有作业
        var dbJobs = jobDetailRep.GetList();
        foreach (var dbJob in dbJobs)
        {
            var jobDetail = JobBuilder.Create(dbJob.AssemblyName, dbJob.JobType).LoadFrom(dbJob);

            // 加载数据库的触发器
            var triggerBuilders = new List<TriggerBuilder>();
            var dbTriggers = jobTriggerRep.GetList(u => u.JobId == dbJob.JobId)
                .Select(u => Triggers.Create(u.AssemblyName, u.TriggerType).LoadFrom(u)).ToArray();
            triggerBuilders.AddRange(dbTriggers);

            var memoryTriggers = memoryJobs.Where(u => u.GetJobBuilder().JobId == dbJob.JobId).SelectMany(u => u.GetTriggerBuilders());
            foreach (var memTrigger in memoryTriggers)
            {
                var triggerId = memTrigger.TriggerId;
                // 若数据库中已包含这个触发器
                if (!string.IsNullOrWhiteSpace(triggerId) && dbTriggers.Any(u => u.TriggerId == triggerId))
                    continue;
                triggerBuilders.Add(memTrigger);
            }
            schedulerBuilders.Add(SchedulerBuilder.Create(jobDetail, triggerBuilders.ToArray()).Updated());
        }

        // 合并作业
        foreach (var job in memoryJobs)
        {
            var jobId = job.GetJobBuilder().JobId;
            // 若数据库中已包含这个作业
            if (!string.IsNullOrWhiteSpace(jobId) && dbJobs.Any(u => u.JobId == jobId))
                continue;
            schedulerBuilders.Add(job);
        }

        return schedulerBuilders;
    }

    /// <summary>
    /// 作业计划初始化通知
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public SchedulerBuilder OnLoading(SchedulerBuilder builder)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        var jobDetailRep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<SysJobDetail>>();
        var jobTriggerRep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<SysJobTrigger>>();
        //return rep.IsAny(u => u.JobId != builder.GetJobBuilder().JobId) ? builder.Appended() : builder.Updated();

        foreach (var (jobBuilder, triggerBuilder) in builder.GetEnumerable())
        {
            if (jobTriggerRep.IsAny(u => u.JobId == jobBuilder.JobId && u.TriggerId == triggerBuilder.TriggerId))
                triggerBuilder.Updated();
            else
                triggerBuilder.Appended();
        }
        return builder;
    }

    /// <summary>
    /// 作业计划Scheduler的JobDetail变化时
    /// </summary>
    /// <param name="context"></param>
    public void OnChanged(PersistenceContext context)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        var db = serviceScope.ServiceProvider.GetService<ISqlSugarClient>();

        var jobDetail = context.JobDetail.Adapt<SysJobDetail>();
        if (context.Behavior == PersistenceBehavior.Appended)
        {
            db.Insertable(jobDetail).ExecuteCommand();
        }
        else if (context.Behavior == PersistenceBehavior.Updated)
        {
            db.Updateable(jobDetail).WhereColumns(u => new { u.JobId }).IgnoreColumns(u => new { u.Id }).ExecuteCommand();
        }
        else if (context.Behavior == PersistenceBehavior.Removed)
        {
            db.Deleteable<SysJobDetail>().Where(u => u.JobId == jobDetail.JobId).ExecuteCommand();
        }
    }

    /// <summary>
    /// 作业计划Scheduler的触发器Trigger变化时
    /// </summary>
    /// <param name="context"></param>
    public void OnTriggerChanged(PersistenceTriggerContext context)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        var db = serviceScope.ServiceProvider.GetService<ISqlSugarClient>();

        var jobTrigger = context.Trigger.Adapt<SysJobTrigger>();
        if (context.Behavior == PersistenceBehavior.Appended)
        {
            db.Insertable(jobTrigger).ExecuteCommand();
        }
        else if (context.Behavior == PersistenceBehavior.Updated)
        {
            db.Updateable(jobTrigger).WhereColumns(u => new { u.TriggerId, u.JobId }).IgnoreColumns(u => new { u.Id }).ExecuteCommand();
        }
        else if (context.Behavior == PersistenceBehavior.Removed)
        {
            db.Deleteable<SysJobTrigger>().Where(u => u.TriggerId == jobTrigger.TriggerId && u.JobId == jobTrigger.JobId).ExecuteCommand();
        }
    }
}