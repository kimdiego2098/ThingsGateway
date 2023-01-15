using Nest;

namespace ThingsGateway.Core;


/// <summary>
/// ES日志写入器
/// </summary>
public class ElasticSearchLoggingWriter : IDatabaseLoggingWriter
{
    private readonly ElasticClient _esClient;

    public ElasticSearchLoggingWriter(ElasticClient esClient)
    {
        _esClient = esClient;
    }

    public void Write(LogMessage logMsg, bool flush)
    {
        var document = new SysLogOp
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
        };
        _esClient.IndexDocument(document);
    }
}