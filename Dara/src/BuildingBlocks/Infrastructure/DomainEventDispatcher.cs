using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Models;
using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.BuildingBlocks.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    readonly protected IServiceProvider _serviceProvider;
    readonly BuildingBlocksLogger _logger;

    public DomainEventDispatcher(IServiceProvider serviceProvider, BuildingBlocksLogger logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchSingleEventAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        var handler = _serviceProvider.GetRequiredService<IHandler<TDomainEvent>>();
        _logger.DomainEventHandlerCalled(handler, domainEvent);
        
        try
        {
            await handler.HandleAsync(domainEvent);
        }
        catch(Exception ex)
        {
            _logger.DomainEventHandlerException(handler, domainEvent, ex);
        }
        
    }

    public async Task DispatchEntityEventsAsync(Entity entity)
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
           await DispatchSingleEventAsync(domainEvent);
        }
        
        entity.ClearDomainEvents();
    }
}