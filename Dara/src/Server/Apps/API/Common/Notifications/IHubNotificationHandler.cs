using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Apps.API.Common.Notifications;

public interface IHubNotificationHandler<in TEvent> where TEvent : IIntegrationEvent
{
    public Task HandleAsync(TEvent notification, CancellationToken cancellationToken);
}