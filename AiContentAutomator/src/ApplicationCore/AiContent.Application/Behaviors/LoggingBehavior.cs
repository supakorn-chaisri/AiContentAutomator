using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AiContent.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("🚀 [START] Handling {RequestName}", requestName);

        var timer = Stopwatch.StartNew();
        try
        {
            var response = await next(); // วิ่งต่อหรือเข้า Handler 

            timer.Stop();
            logger.LogInformation("✅ [END] Handled {RequestName} in {ElapsedMs}ms", requestName, timer.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            timer.Stop();
            logger.LogError(ex, "❌ [ERROR] Handled {RequestName} failed after {ElapsedMs}ms", requestName, timer.ElapsedMilliseconds);
            throw;
        }
    }
}