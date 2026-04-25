using Dara.BuildingBlocks.Application.Abstraction;
using Dara.BuildingBlocks.Domain.Events.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        var handler = _serviceProvider.GetRequiredService<IDomainEventHandler<TEvent>>();
        
        Guid id = Guid.NewGuid();
        Console.WriteLine($"\n\t{id} - Called event Handler: {handler.GetType().Name} with event: {typeof(TEvent).Name}");
        Console.WriteLine($"\n\t{id} - Event value: {domainEvent}");

        try
        {
            await handler.HandleAsync((dynamic)domainEvent);
            Console.WriteLine($"\n\t{id} - handling finished, dynamic event: {(dynamic)domainEvent.GetType().Name})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n\t{id} - Got Exception: {ex.GetType()} : {ex.Message}");
        }
    }
}