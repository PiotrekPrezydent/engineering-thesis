using Dara.Server.BuildingBlocks.Application;
using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Models;
using Dara.BuildingBlocks.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dara.BuildingBlocks.Infrastructure.DomainEvents;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    readonly protected IServiceProvider _serviceProvider;
    readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchSingleEventAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        _logger.LogMethodCalled(nameof(DispatchEntityEventsAsync), new object[]{domainEvent});
        try
        {
            var handler = _serviceProvider.GetRequiredService<IHandler<TDomainEvent>>();

            try
            {
                var task = handler.HandleAsync(domainEvent);
                Logging.DomainEventDispatcherLogMessages.LogDomainEventHandlerCalled(_logger, typeof(IHandler<TDomainEvent>).Name, domainEvent.GetType().Name, domainEvent);

                await task;
            }
            catch (Exception handlerException)
            {
                Logging.DomainEventDispatcherLogMessages.LogDomainEventHandlerException(_logger, typeof(IHandler<TDomainEvent>).Name, domainEvent.GetType().Name, domainEvent, handlerException.GetType().Name, handlerException.Message);
            }
        }
        catch (Exception dispatcherException)
        {
            Logging.DomainEventDispatcherLogMessages.LogDomainEventDispatcherException(_logger, dispatcherException.GetType().Name, dispatcherException.Message);
        }

        
    }

    public async Task DispatchEntityEventsAsync(Entity entity)
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
           await DispatchSingleEventAsync((dynamic)domainEvent);
        }
        
        entity.ClearDomainEvents();
    }
}