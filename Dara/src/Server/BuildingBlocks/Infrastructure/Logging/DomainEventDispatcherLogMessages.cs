using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Infrastructure.DomainEvents;
using Microsoft.Extensions.Logging;

namespace Dara.BuildingBlocks.Infrastructure.Logging;

public static partial class DomainEventDispatcherLogMessages
{
    [LoggerMessage(
        LogLevel.Information,
        EventName = "CallingDomainEventHandler",
        EventId = LogsIdsRanges.CoreStart + 100,
        Message = "Called domain event handler {domainEventHandlerName} for domain event {domainEventName} used object: {domainEventObject}.")]
    public static partial void LogDomainEventHandlerCalled(this ILogger<IDomainEventDispatcher> logger, string domainEventHandlerName, string domainEventName, IDomainEvent domainEventObject);
    
    [LoggerMessage(
        LogLevel.Error,
        EventName = "DomainEventHandlerException",
        EventId = LogsIdsRanges.CoreStart + 101,
        Message = "Domain event handler {domainEventHandlerName} for domain event {domainEventName} using object: {domainEventObject} throw exception of type {exceptionType}, exception message: {exceptionMessage}.")]
    public static partial void LogDomainEventHandlerException(this ILogger<IDomainEventDispatcher> logger, string domainEventHandlerName, string domainEventName, IDomainEvent domainEventObject, string exceptionType, string exceptionMessage);
    
    [LoggerMessage(
        LogLevel.Error,
        EventName = "DomainEventDispatcherException",
        EventId = LogsIdsRanges.CoreStart + 102,
        Message = "Domain event dispatcher throw exception of type {exceptionType}, exception message: {exceptionMessage}.")]
    public static partial void LogDomainEventDispatcherException(this ILogger<IDomainEventDispatcher> logger, string exceptionType, string exceptionMessage);

}