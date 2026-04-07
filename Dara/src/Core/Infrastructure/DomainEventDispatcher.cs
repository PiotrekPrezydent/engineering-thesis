using Dara.Core.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Core.Infrastructure;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        var handlers = _serviceProvider.GetServices<IDomainEventHandler<TEvent>>();
        
        foreach (var handler in handlers)
            await handler.HandleAsync((dynamic)domainEvent);
    }
}