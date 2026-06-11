using Dara.BuildingBlocks.Domain;

namespace Dara.BuildingBlocks.Infrastructure.Processing;

public class DomainEventsDispatcher : IDomainEventsDispatcher
{
    readonly IHandlersResolver _handlersResolver;

    public DomainEventsDispatcher(IHandlersResolver handlersResolver)
    {
        _handlersResolver = handlersResolver;
    }

    public async Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        var handlers = _handlersResolver.GetDomainEventHandlers<TDomainEvent>();
        foreach (var handler  in handlers)
            await handler.HandleAsync(domainEvent);
    }
}