using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Domain.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application;

public class UserCreatedNotificationPublisher : IDomainEventNotificationHandler<NewUserCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public UserCreatedNotificationPublisher(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }


    public async Task HandleAsync(NewUserCreatedDomainEvent notification)
    {
        Console.WriteLine("PUBLISHING");
        await _eventBus.PublishAsync(new UserCreatedIntegrationEvent(notification.UserId.Value));
    }
}