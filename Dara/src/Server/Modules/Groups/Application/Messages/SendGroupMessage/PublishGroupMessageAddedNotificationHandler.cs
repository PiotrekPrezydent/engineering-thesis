using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Groups.Application.Messages.SendGroupMessage;

public class PublishGroupMessageAddedNotificationHandler : IDomainEventNotificationHandler<GroupMessageAddedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishGroupMessageAddedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(GroupMessageAddedNotification notification)
    {
        await _eventBus.PublishAsync(new GroupMessageAddedIntegrationEvent(notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.MessageId,
            notification.DomainEvent.GroupId,
            notification.DomainEvent.AuthorId,
            notification.DomainEvent.Content));
    }
}