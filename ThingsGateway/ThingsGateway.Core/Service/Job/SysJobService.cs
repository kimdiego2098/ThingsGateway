namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统作业任务服务
/// </summary>
[ApiDescriptionSettings(Order = 188)]
public class SysJobService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysJobDetail> _sysJobDetailRep;
    private readonly SqlSugarRepository<SysJobTrigger> _sysJobTriggerRep;
    private readonly SqlSugarRepository<SysJobCluster> _sysJobClusterRep;
    private readonly ISchedulerFactory _schedulerFactory;

    public SysJobService(SqlSugarRepository<SysJobDetail> sysJobDetailRep,
        SqlSugarRepository<SysJobTrigger> sysJobTriggerRep,
        SqlSugarRepository<SysJobCluster> sysJobClusterRep,
        ISchedulerFactory schedulerFactory)
    {
        _sysJobDetailRep = sysJobDetailRep;
        _sysJobTriggerRep = sysJobTriggerRep;
        _sysJobClusterRep = sysJobClusterRep;
        _schedulerFactory = schedulerFactory;
    }

    /// <summary>
    /// 获取作业分页列表
    /// </summary>
    [HttpGet]
    public async Task<SqlSugarPagedList<JobOutput>> GetJobPage([FromQuery] PageJobInput input)
    {
        var jobDetails = await _sysJobDetailRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.JobId), u => u.JobId.Contains(input.JobId))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Description), u => u.Description.Contains(input.Description))
            .Select(d => new JobOutput
            {
                JobDetail = d,
            }).ToPagedListAsync(input.Page, input.PageSize);
        await _sysJobDetailRep.AsSugarClient().ThenMapperAsync(jobDetails.Items, async u =>
        {
            u.JobTriggers = await _sysJobTriggerRep.GetListAsync(t => t.JobId == u.JobDetail.JobId);
        });


        // 提取中括号里面的参数值
        var rgx = new Regex(@"(?i)(?<=\[)(.*)(?=\])");
        foreach (var job in jobDetails.Items)
        {
            foreach (var jobTrigger in job.JobTriggers)
            {
                jobTrigger.Args = rgx.Match(jobTrigger.Args).Value;
            }
        }

        return jobDetails;
    }

    /// <summary>
    /// 添加作业
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task AddJobDetail(AddJobDetailInput input)
    {
        var isExist = await _sysJobDetailRep.IsAnyAsync(u => u.JobId == input.JobId && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D1006);

        // 动态创建作业
        NatashaInitializer.Preheating();
        var oop = new AssemblyCSharpBuilder("ThingsGateway.Core");
        oop.Domain = DomainManagement.Random();
        oop.Add(input.ScriptCode);
        var jobType = oop.GetTypeFromShortName(input.JobId);
        _schedulerFactory.AddJob(JobBuilder.Create(jobType).SetIncludeAnnotations(input.IncludeAnnotations));

        await _sysJobDetailRep.InsertAsync(input.Adapt<SysJobDetail>());
    }

    /// <summary>
    /// 更新作业
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateJobDetail(UpdateJobDetailInput input)
    {
        var isExist = await _sysJobDetailRep.IsAnyAsync(u => u.JobId == input.JobId && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D1006);

        await _sysJobDetailRep.UpdateAsync(input.Adapt<SysJobDetail>());
    }

    /// <summary>
    /// 删除作业
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteJobDetail(DeleteJobDetailInput input)
    {
        _schedulerFactory.RemoveJob(input.JobId);

        await _sysJobDetailRep.DeleteAsync(u => u.JobId == input.JobId);
        await _sysJobTriggerRep.DeleteAsync(u => u.JobId == input.JobId);
    }

    /// <summary>
    /// 获取触发器列表
    /// </summary>
    [HttpGet]
    public async Task<List<SysJobTrigger>> GetJobTriggerList([FromQuery] JobDetailInput input)
    {
        return await _sysJobTriggerRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.JobId), u => u.JobId.Contains(input.JobId))
            .ToListAsync();
    }

    /// <summary>
    /// 添加触发器
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task AddJobTrigger(AddJobTriggerInput input)
    {
        var isExist = await _sysJobTriggerRep.IsAnyAsync(u => u.TriggerId == input.TriggerId && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D1006);

        var jobTrigger = input.Adapt<SysJobTrigger>();
        jobTrigger.Args = "[" + jobTrigger.Args + "]";
        await _sysJobTriggerRep.InsertAsync(jobTrigger);
    }

    /// <summary>
    /// 更新触发器
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateJobTrigger(UpdateJobTriggerInput input)
    {
        var isExist = await _sysJobTriggerRep.IsAnyAsync(u => u.TriggerId == input.TriggerId && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D1006);

        var jobTrigger = input.Adapt<SysJobTrigger>();
        jobTrigger.Args = "[" + jobTrigger.Args + "]";
        await _sysJobTriggerRep.UpdateAsync(jobTrigger);
    }

    /// <summary>
    /// 删除触发器
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteJobTrigger(DeleteJobTriggerInput input)
    {
        await _sysJobTriggerRep.DeleteAsync(u => u.TriggerId == input.TriggerId);
    }

    /// <summary>
    /// 暂停所有作业
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public void PauseAllJob()
    {
        _schedulerFactory.PauseAll();
    }

    /// <summary>
    /// 启动所有作业
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public void StartAllJob()
    {
        _schedulerFactory.StartAll();
    }

    /// <summary>
    /// 暂停作业
    /// </summary>
    [HttpPost]
    public void PauseJob(JobDetailInput input)
    {
        _ = _schedulerFactory.TryGetJob(input.JobId, out var _scheduler);
        _scheduler?.Pause();
    }

    /// <summary>
    /// 启动作业
    /// </summary>
    [HttpPost]
    public void StartJob(JobDetailInput input)
    {
        _ = _schedulerFactory.TryGetJob(input.JobId, out var _scheduler);
        _scheduler?.Start();
    }

    /// <summary>
    /// 暂停触发器
    /// </summary>
    [HttpPost]
    public void PauseTrigger(JobTriggerInput input)
    {
        _ = _schedulerFactory.TryGetJob(input.JobId, out var _scheduler);
        _scheduler?.PauseTrigger(input.TriggerId);
    }

    /// <summary>
    /// 启动触发器
    /// </summary>
    [HttpPost]
    public void StartTrigger(JobTriggerInput input)
    {
        _ = _schedulerFactory.TryGetJob(input.JobId, out var _scheduler);
        _scheduler?.StartTrigger(input.TriggerId);
    }

    /// <summary>
    /// 强制唤醒作业调度器
    /// </summary>
    [HttpPost]
    public void CancelSleep()
    {
        _schedulerFactory.CancelSleep();
    }

    /// <summary>
    /// 强制触发所有作业持久化
    /// </summary>
    [HttpPost]
    public void PersistAll()
    {
        _schedulerFactory.PersistAll();
    }

    /// <summary>
    /// 获取集群列表
    /// </summary>
    [HttpGet]
    public async Task<List<SysJobCluster>> GetJobClusterList()
    {
        return await _sysJobClusterRep.GetListAsync();
    }
}