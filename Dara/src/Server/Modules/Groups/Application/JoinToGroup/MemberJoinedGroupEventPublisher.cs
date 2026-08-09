using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Domain.Groups.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Groups.Application.JoinToGroup;

public class MemberJoinedGroupEventPublisher : IDomainEventNotificationHandler<GroupMemberJoinedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public MemberJoinedGroupEventPublisher(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(GroupMemberJoinedDomainEvent notification)
    {
        await _eventBus.PublishAsync(
            new NewMemberJoinedGroupIntegrationEvent(notification.GroupId, notification.MemberId));
    }
}