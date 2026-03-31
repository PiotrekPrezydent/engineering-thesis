using LADR.SharedKernel.Domain.Models;
using LADR.SharedKernel.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LADR.SharedKernel.Infrastructure.Dispatching;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : DomainEvent
    {
        var handlers = _serviceProvider.GetServices<IDomainEventHandler<TEvent>>();
        
        foreach (var handler in handlers)
            await handler.HandleAsync(domainEvent);
    }
}