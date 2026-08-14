using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Groups.Application.Groups.JoinToGroup;

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
            new MemberJoinedGroupIntegrationEvent(notification.NotificationId,
                notification.DomainEvent.OccuredOn,
                notification.DomainEvent.GroupId,
                notification.DomainEvent.MemberId));
    }
}