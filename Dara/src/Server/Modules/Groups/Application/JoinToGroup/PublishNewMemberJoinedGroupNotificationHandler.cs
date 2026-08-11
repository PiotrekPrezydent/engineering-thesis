using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Domain.Groups.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Groups.Application.JoinToGroup;

public class PublishNewMemberJoinedGroupNotificationHandler : IDomainEventNotificationHandler<NewMemberJoinedGroupNotification>
{
    private readonly IEventBus _eventBus;

    public PublishNewMemberJoinedGroupNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(NewMemberJoinedGroupNotification notification)
    {
        await _eventBus.PublishAsync(
            new NewMemberJoinedGroupIntegrationEvent(notification.NotificationId,
                notification.DomainEvent.OccuredOn,
                notification.DomainEvent.GroupId,
                notification.DomainEvent.MemberId));
    }
}