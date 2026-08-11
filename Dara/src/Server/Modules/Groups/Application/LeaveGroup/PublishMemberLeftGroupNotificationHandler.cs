using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Domain.Groups.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Groups.Application.LeaveGroup;

public class PublishMemberLeftGroupNotificationHandler : IDomainEventNotificationHandler<MemberLeftGroupNotification>
{
    private readonly IEventBus _eventBus;

    public PublishMemberLeftGroupNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(MemberLeftGroupNotification notification)
    {
        await _eventBus.PublishAsync(new MemberLeftGroupIntegrationEvent(notification.NotificationId,
            notification.DomainEvent.OccuredOn,
            notification.DomainEvent.GroupId,
            notification.DomainEvent.MemberId));
    }
}