using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.Shared.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsoleLogger _consoleLogger;
    
    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _consoleLogger = new ConsoleLogger(this);
        _consoleLogger.Start("CREATE");
        
        _serviceProvider = serviceProvider;
        
        _consoleLogger.Element("SERVICE", _serviceProvider);
        _consoleLogger.End();
    }
    
    public async Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        var handler = _serviceProvider.GetRequiredService<IDomainEventHandler<TEvent>>();
        
        _consoleLogger.Start("HANDLING DOMAIN EVENT");
        _consoleLogger.Element("HANDLER", handler);
        _consoleLogger.Element("EVENT", domainEvent);
        
        await handler.HandleAsync((dynamic)domainEvent);
        
        _consoleLogger.End();

    }
}