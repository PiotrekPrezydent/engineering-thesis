using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dara.BuildingBlocks.Infrastructure.IntegrationEvents;

public class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    readonly IServiceProvider _serviceProvider;
    readonly ILogger<IntegrationEventDispatcher> _logger;

    public IntegrationEventDispatcher(IServiceProvider serviceProvider, ILogger<IntegrationEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchIntegrationEventAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent) where TIntegrationEvent : IIntegrationEvent
    {
        try
        {
            var handlers = _serviceProvider.GetServices<IHandler<TIntegrationEvent>>();
            
            foreach (var handler in handlers)
            {
                try
                {
                    var task = handler.HandleAsync(integrationEvent);
                    Logging.IntegrationEventDispatcherRunnerLogMessages.LogIntegrationEventHandlerCalled(_logger, handler.GetType().Name, integrationEvent.GetType().Name, integrationEvent);
                    await task;
                }catch(Exception handlerException)
                {
                    Logging.IntegrationEventDispatcherRunnerLogMessages.LogIntegrationEventHandlerException(_logger, handler.GetType().Name, integrationEvent.GetType().Name, integrationEvent, handlerException.GetType().Name, handlerException.Message);
                }
            }
        }catch(Exception dispatcherException)
        {
            Logging.IntegrationEventDispatcherRunnerLogMessages.LogIntegrationEventDispatcherException(_logger, dispatcherException.GetType().Name, dispatcherException.Message);
        }

    }
}