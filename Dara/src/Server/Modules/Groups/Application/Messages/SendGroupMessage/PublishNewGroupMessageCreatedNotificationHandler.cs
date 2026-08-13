using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Groups.Application.Messages.SendGroupMessage;

public class PublishNewGroupMessageCreatedNotificationHandler : IDomainEventNotificationHandler<NewGroupMessageCreatedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishNewGroupMessageCreatedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(NewGroupMessageCreatedNotification notification)
    {
        await _eventBus.PublishAsync(new NewGroupMessageCreatedIntegrationEvent(notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.MessageId,
            notification.DomainEvent.GroupId,
            notification.DomainEvent.AuthorId,
            notification.DomainEvent.Content));
    }
}