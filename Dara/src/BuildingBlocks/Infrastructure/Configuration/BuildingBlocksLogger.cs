using Microsoft.Extensions.Logging;

namespace Dara.BuildingBlocks.Infrastructure.Configuration;

public partial class BuildingBlocksLogger
{
    readonly ILogger<BuildingBlocksLogger> _logger;

    public BuildingBlocksLogger(ILogger<BuildingBlocksLogger> logger)
    {
        _logger = logger;
    }

    public void DomainEventHandlerCalled(object handler, object domainEvent)
    {
        LogDomainEventHandlerCalled(_logger, handler, domainEvent);
    }

    public void DomainEventHandlerException(object handler, object domainEvent, Exception exception)
    {
        LogDomainEventHandlerException(_logger, handler, domainEvent, exception);
    }

    public void IntegrationEventHandlerCalled(object handler, object integrationEvent)
    {
        LogIntegrationEventHandlerCalled(_logger, handler, integrationEvent);
    }

    public void IntegrationEventHandlerException(object handler, object integrationEvent, Exception exception)
    {
        LogIntegrationEventHandlerException(_logger, handler, integrationEvent, exception);
    }

    public void ModuleCommandHandlerCalled(object handler, object moduleCommand)
    {
        LogModuleCommandHandlerCalled(_logger, handler, moduleCommand);   
    }

    public void ModuleCommandHandlerException(object handler, object moduleCommand, Exception exception)
    {
        LogModuleCommandHandlerException(_logger, handler, moduleCommand, exception);
    }
    
    public void ModuleCommandHandlerResult(object handler, object moduleCommand, object result)
    {
        LogModuleCommandHandlerResult(_logger, handler, moduleCommand, result);
    }
    
    //domain event dispatcher
    [LoggerMessage(LogLevel.Information, "Called domain event handler: {handler} for domain event: {domainEvent}")]
    public static partial void LogDomainEventHandlerCalled(ILogger logger, object handler, object domainEvent);
    
    [LoggerMessage(LogLevel.Information, "Exception in domain event handler: {handler} for domain event: {domainEvent}, Exception: {exception}")]
    public static partial void LogDomainEventHandlerException(ILogger logger, object handler, object domainEvent, object exception);
    
    
    //integration event dispatcher
    [LoggerMessage(LogLevel.Information, "Called integration event handler: {handler} for integration event: {integrationEvent}")]
    public static partial void LogIntegrationEventHandlerCalled(ILogger logger, object handler, object integrationEvent);
    
    [LoggerMessage(LogLevel.Information, "Exception in integration event handler: {handler} for integration event: {integrationEvent}, Exception: {exception}")]
    public static partial void LogIntegrationEventHandlerException(ILogger logger, object handler, object integrationEvent, object exception);
    
    
    //module command runner
    [LoggerMessage(LogLevel.Information, "Called module command handler: {handler} for module command: {moduleCommand}")]
    public static partial void LogModuleCommandHandlerCalled(ILogger logger, object handler, object moduleCommand);
    
    [LoggerMessage(LogLevel.Information, "Exception in module command handler: {handler} for module command: {moduleCommand}, Exception: {exception}")]
    public static partial void LogModuleCommandHandlerException(ILogger logger, object handler, object moduleCommand, object exception);
    
    [LoggerMessage(LogLevel.Information, "Result in module command handler: {handler} for module command: {moduleCommand}, Result: {result}")]
    public static partial void LogModuleCommandHandlerResult(ILogger logger, object handler, object moduleCommand, object result);
}