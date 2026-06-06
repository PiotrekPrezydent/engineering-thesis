using Microsoft.Extensions.Logging;

namespace Dara.BuildingBlocks.Infrastructure.Logging;

public static partial class DaraLogMessages
{
    [LoggerMessage(
        LogLevel.Information,
        EventName = "MethodCall",
        EventId = LogsIdsRanges.CoreStart + 1,
        Message = "Called method: {methodName} with parameters: {parameters}")]
    public static partial void LogMethodCalled(this ILogger logger, string methodName, object[] parameters);
}