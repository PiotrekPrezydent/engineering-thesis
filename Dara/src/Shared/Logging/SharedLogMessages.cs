using Microsoft.Extensions.Logging;

namespace Dara.Shared.Logging;

public static partial class SharedLogMessages
{
    [LoggerMessage(
        LogLevel.Debug,
        EventName = "MethodCall",
        EventId = SharedLogsIdsRanges.SharedStart + 1,
        Message = "Called method: {methodName} with parameters: {parameters}")]
    public static partial void LogMethodCalled(this ILogger logger, string methodName, object[] parameters);
    
    [LoggerMessage(
        LogLevel.Debug,
        EventName = "MethodEnded",
        EventId = SharedLogsIdsRanges.SharedStart + 2,
        Message = "Ended running method: {methodName} with parameters: {parameters}")]
    public static partial void LogMethodEnded(this ILogger logger, string methodName, object[] parameters);
}