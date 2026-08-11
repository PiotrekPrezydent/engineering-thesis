using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;

public class InMemoryEventBus : IEventBus
{
    public static InMemoryEventBus Instance { get; } = new InMemoryEventBus();
    
    readonly IDictionary<Type, List<IIntegrationEventHandler>> _eventHandlers = new Dictionary<Type, List<IIntegrationEventHandler>>();
    
    public void Subscribe<TEvent>(IIntegrationEventHandler<TEvent> handler) where TEvent : IIntegrationEvent
    {
        var eventType = typeof(TEvent);
        
        if(_eventHandlers.TryGetValue(eventType, out var eventHandler))
            eventHandler.Add(handler);
        else
            _eventHandlers.Add(eventType,[handler]);
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IIntegrationEvent
    {
        var eventType = typeof(TEvent);
        if (_eventHandlers.TryGetValue(eventType, out var eventHandlers))
        {
            foreach (var handler  in eventHandlers)
            {
                if (handler is IIntegrationEventHandler<TEvent> eventHandler)
                    await eventHandler.HandleAsync(@event);
            }
        }
    }
}