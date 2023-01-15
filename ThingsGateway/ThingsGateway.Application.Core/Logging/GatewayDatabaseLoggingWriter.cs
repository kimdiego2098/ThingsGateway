using Furion.Logging;

namespace ThingsGateway.Application.Core;

/// <summary>
/// 数据库日志写入器
/// </summary>
public class GatewayDatabaseLoggingWriter : IDatabaseLoggingWriter
{
    private readonly SqlSugarRepository<GatewayLogOp> _gatewayLogOp; // 网关日志

    public GatewayDatabaseLoggingWriter(
        SqlSugarRepository<GatewayLogOp> gatewayLogOp
        )
    {
        _gatewayLogOp = gatewayLogOp;
    }

    public void Write(LogMessage logMsg, bool flush)
    {
        if (logMsg.LogName.Contains("ThingsGateway.Application.Core"))
        {
            _gatewayLogOp.Insert(new GatewayLogOp
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
            });
        }
    }
}