using System.Threading.Channels;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Apps.API.Common.Notifications;

public class ChannelWriterIntegrationEventHandler<T> : IIntegrationEventHandler<T> where T : IIntegrationEvent
{
    private readonly Channel<IIntegrationEvent> _channel;
    private readonly HubNotificationsDeduplicator _deduplicator;

    public ChannelWriterIntegrationEventHandler(Channel<IIntegrationEvent> channel, HubNotificationsDeduplicator deduplicator)
    {
        _channel = channel;
        _deduplicator = deduplicator;
    }
    
    public async Task HandleAsync(T integrationEvent)
    {
        // a repeat of a retried outbox message is a success for the bus, the client got the notification already
        if (!_deduplicator.TryAccept(integrationEvent))
            return;

        await _channel.Writer.WriteAsync(integrationEvent);
    }
}