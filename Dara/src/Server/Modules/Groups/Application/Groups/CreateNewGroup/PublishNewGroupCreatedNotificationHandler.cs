using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;

public class PublishNewGroupCreatedNotificationHandler : IDomainEventNotificationHandler<NewGroupCreatedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishNewGroupCreatedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(NewGroupCreatedNotification notification)
    {
        await _eventBus.PublishAsync(new NewGroupCreatedIntegrationEvent(notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.GroupId,
            notification.DomainEvent.CreatorId));
    }
}