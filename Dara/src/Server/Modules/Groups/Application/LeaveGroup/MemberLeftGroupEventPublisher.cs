using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Domain.Groups.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Groups.Application.LeaveGroup;

public class MemberLeftGroupEventPublisher : IDomainEventNotificationHandler<GroupMemberLeftDomainEvent>
{
    private readonly IEventBus _eventBus;

    public MemberLeftGroupEventPublisher(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(GroupMemberLeftDomainEvent notification)
    {
        await _eventBus.PublishAsync(new MemberLeftGroupIntegrationEvent(notification.GroupId, notification.MemberId));
    }
}