using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Domain.Events;
using Dara.Shared.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        var cl = new ConsoleLogger("HANDLING DOMAIN EVENT");
        var handler = _serviceProvider.GetRequiredService<IDomainEventHandler<TEvent>>();
        
        cl.Start();
        cl.Element("HANDLER", handler);
        cl.Element("EVENT", domainEvent);
        
        await handler.HandleAsync((dynamic)domainEvent);
        
        cl.End();

    }
}