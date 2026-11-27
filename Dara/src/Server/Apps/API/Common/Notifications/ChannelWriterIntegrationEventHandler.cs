using System.Threading.Channels;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Apps.API.Common.Notifications;

public class ChannelWriterIntegrationEventHandler<T> : IIntegrationEventHandler<T> where T : IIntegrationEvent
{
    private readonly Channel<IIntegrationEvent> _channel;

    public ChannelWriterIntegrationEventHandler(Channel<IIntegrationEvent> channel)
    {
        _channel = channel;
    }
    
    public async Task HandleAsync(T integrationEvent)
    {
        await _channel.Writer.WriteAsync(integrationEvent);
    }
}