using System.Collections.Concurrent;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.SystemTests.ClientServer;

// every event of one type the bus of a test host published, in order
public class RecordingIntegrationEventHandler<TEvent> : IIntegrationEventHandler<TEvent> where TEvent : IIntegrationEvent
{
    private readonly ConcurrentQueue<TEvent> _events = new();

    public IReadOnlyList<TEvent> Events => _events.ToList();

    public static RecordingIntegrationEventHandler<TEvent> SubscribeTo(AppHubTestHost host)
    {
        var handler = new RecordingIntegrationEventHandler<TEvent>();
        host.Service<IEventBus>().Subscribe(handler);
        return handler;
    }

    public Task HandleAsync(TEvent integrationEvent)
    {
        _events.Enqueue(integrationEvent);
        return Task.CompletedTask;
    }
}
