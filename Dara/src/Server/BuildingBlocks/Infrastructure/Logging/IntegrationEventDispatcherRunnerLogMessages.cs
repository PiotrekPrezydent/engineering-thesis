using Dara.BuildingBlocks.Infrastructure.IntegrationEvents;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.Extensions.Logging;

namespace Dara.BuildingBlocks.Infrastructure.Logging;

public static partial class IntegrationEventDispatcherRunnerLogMessages
{
    [LoggerMessage(
        LogLevel.Information,
        EventName = "CallingIntegrationEventHandler",
        EventId = LogsIdsRanges.CoreStart + 200,
        Message = "Called integration event handler {integrationEventHandlerName} for integration event {integrationEventName} used object: {integrationEventObject}.")]
    public static partial void LogIntegrationEventHandlerCalled(this ILogger<IIntegrationEventDispatcher> logger, string integrationEventHandlerName, string integrationEventName, IIntegrationEvent integrationEventObject);
    
    [LoggerMessage(
        LogLevel.Error,
        EventName = "IntegrationEventHandlerException",
        EventId = LogsIdsRanges.CoreStart + 201,
        Message = "Integration event handler {integrationEventHandlerName} for integration event {integrationEventName} used object: {integrationEventObject} throw exception of type {exceptionType}, exception message: {exceptionMessage}.")]
    public static partial void LogIntegrationEventHandlerException(this ILogger<IIntegrationEventDispatcher> logger, string integrationEventHandlerName, string integrationEventName, IIntegrationEvent integrationEventObject, string exceptionType, string exceptionMessage);
    
    [LoggerMessage(
        LogLevel.Error,
        EventName = "IntegrationEventDispatcherException",
        EventId = LogsIdsRanges.CoreStart + 202,
        Message = "Integration event dispatcher throw exception of type {exceptionType}, exception message: {exceptionMessage}.")]
    public static partial void LogIntegrationEventDispatcherException(this ILogger<IIntegrationEventDispatcher> logger, string exceptionType, string exceptionMessage);

}