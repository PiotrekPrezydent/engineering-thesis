using LADR.SharedKernel.Infrastructure.Models;
using LADR.SharedKernel.Integration.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LADR.SharedKernel.Infrastructure.Dispatching;

public class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public IntegrationEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IntegrationEvent
    {
        var handlers = _serviceProvider.GetServices<IIntegrationEventHandler<TEvent>>();
        
        foreach (var handler in handlers)
            await handler.HandleAsync(domainEvent);
    }
}