using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;

// one bus per host, created by the host and passed to every module
public class InMemoryEventBus : IEventBus
{
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
        if (!_eventHandlers.TryGetValue(eventType, out var eventHandlers))
            return;

        // a failing subscriber does not cut off the next ones; the outbox retries the message and the inboxes of the
        // subscribers that got it already drop the repeat
        List<Exception>? errors = null;
        foreach (var handler in eventHandlers)
        {
            if (handler is not IIntegrationEventHandler<TEvent> eventHandler)
                continue;

            try
            {
                await eventHandler.HandleAsync(@event);
            }
            catch (Exception ex)
            {
                (errors ??= []).Add(ex);
            }
        }

        if (errors is { Count: 1 })
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(errors[0]).Throw();

        if (errors != null)
            throw new AggregateException($"{errors.Count} subscribers of {eventType.Name} failed.", errors);
    }
}