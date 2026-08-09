using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Domain.Groups.Events;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Modules.Groups.Application.CreateGroup;

public class NewGroupCreatedEventPublisher : IDomainEventNotificationHandler<GroupCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public NewGroupCreatedEventPublisher(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(GroupCreatedDomainEvent notification)
    {
        await _eventBus.PublishAsync(new GroupCreatedIntegrationEvent(notification.GroupId, notification.OwnerId));
    }
}