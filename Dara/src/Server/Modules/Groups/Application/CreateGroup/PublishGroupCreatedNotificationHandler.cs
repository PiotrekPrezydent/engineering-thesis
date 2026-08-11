using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Domain.Groups.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Groups.Application.CreateGroup;

public class PublishGroupCreatedNotificationHandler : IDomainEventNotificationHandler<GroupCreatedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishGroupCreatedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(GroupCreatedNotification notification)
    {
        await _eventBus.PublishAsync(new GroupCreatedIntegrationEvent(notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.GroupId,
            notification.DomainEvent.OwnerId));
    }
}