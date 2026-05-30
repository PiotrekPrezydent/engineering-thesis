using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.BuildingBlocks.Infrastructure.Configuration;
using Dara.BuildingBlocks.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    readonly IServiceProvider _serviceProvider;
    readonly BuildingBlocksLogger _logger;

    public IntegrationEventDispatcher(IServiceProvider serviceProvider, BuildingBlocksLogger logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchIntegrationEventAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent) where TIntegrationEvent : IIntegrationEvent
    {
        var handlers = _serviceProvider.GetServices<IHandler<TIntegrationEvent>>();
        foreach (var handler in handlers)
        {
            try
            {
                _logger.IntegrationEventHandlerCalled(handler, integrationEvent);
                await  handler.HandleAsync(integrationEvent);
            }catch(Exception ex)
            {
                _logger.IntegrationEventHandlerException(handler, integrationEvent, ex);
            }
        }
    }
}