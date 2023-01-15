namespace ThingsGateway.Core;

/// <summary>
/// 结果筛选器
/// </summary>
public class ResultFilter : IAsyncResultFilter
{
    // 脱敏验证处理器
    private readonly ISensitiveDetectionProvider _sensitiveDetectionProvider;

    public ResultFilter(ISensitiveDetectionProvider sensitiveDetectionProvider)
    {
        _sensitiveDetectionProvider = sensitiveDetectionProvider;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // 是否开启全局脱敏显示处理
        if (CommonConst.SysSensitiveDetection)
        {
            var resStr = context.Result?.GetType() == typeof(JsonResult) ? JSON.Serialize(context.Result) : string.Empty;
            if (!string.IsNullOrWhiteSpace(resStr))
            {
                resStr = await _sensitiveDetectionProvider.ReplaceAsync(resStr, '*');
                // 强制替换结果
                context.Result = new ContentResult
                {
                    Content = resStr
                };
            }
        }

        await next();
    }
}