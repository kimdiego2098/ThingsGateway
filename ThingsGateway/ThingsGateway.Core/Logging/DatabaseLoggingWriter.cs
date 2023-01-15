namespace ThingsGateway.Core;

/// <summary>
/// 数据库日志写入器
/// </summary>
public class DatabaseLoggingWriter : IDatabaseLoggingWriter
{
    private readonly SqlSugarRepository<SysLogOp> _sysLogOpRep; // 操作日志
    private readonly SqlSugarRepository<SysLogEx> _sysLogExRep; // 异常日志

    public DatabaseLoggingWriter(SqlSugarRepository<SysLogOp> sysLogOpRep,
        SqlSugarRepository<SysLogEx> sysLogExRep)
    {
        _sysLogOpRep = sysLogOpRep;
        _sysLogExRep = sysLogExRep;
    }

    public void Write(LogMessage logMsg, bool flush)
    {
        if (logMsg.LogName.Contains("ThingsGateway.Application.Core"))
        {

        }
        else if (logMsg.LogLevel == Microsoft.Extensions.Logging.LogLevel.Information)
        {
            _sysLogOpRep.Insert(new SysLogOp
            {
                LogName = logMsg.LogName,
                LogLevel = logMsg.LogLevel.ToString(),
                EventId = logMsg.EventId.Id.ToString(),
                Message = logMsg.Message,
                Exception = logMsg.Exception?.ToString(),
                State = logMsg.State?.ToString(),
                LogDateTime = logMsg.LogDateTime,
                ThreadId = logMsg.ThreadId,
                TraceId = logMsg.TraceId,
                UseUtcTimestamp = logMsg.UseUtcTimestamp,
            });
        }
        else if (logMsg.LogLevel > Microsoft.Extensions.Logging.LogLevel.Debug)
        {
            _sysLogExRep.Insert(new SysLogEx
            {
                LogName = logMsg.LogName,
                LogLevel = logMsg.LogLevel.ToString(),
                EventId = logMsg.EventId.Id.ToString(),
                Message = logMsg.Message,
                Exception = logMsg.Exception?.ToString(),
                State = logMsg.State?.ToString(),
                LogDateTime = logMsg.LogDateTime,
                ThreadId = logMsg.ThreadId,
                TraceId = logMsg.TraceId,
                UseUtcTimestamp = logMsg.UseUtcTimestamp,
            });
        }
    }
}