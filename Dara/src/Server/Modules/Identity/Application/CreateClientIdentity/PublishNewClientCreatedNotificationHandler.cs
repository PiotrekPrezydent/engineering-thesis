using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Domain.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application.CreateClientIdentity;

public class PublishNewClientCreatedNotificationHandler : IDomainEventNotificationHandler<NewClientIdentityCreatedNotification>
{
    private readonly IEventBus _eventBus;

    public PublishNewClientCreatedNotificationHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    public async Task HandleAsync(NewClientIdentityCreatedNotification notification)
    {
        Console.WriteLine("PUBLISH NOTIFICATION");
        
        await _eventBus.PublishAsync(new NewClientCreatedIntegrationEvent(notification.NotificationId,notification.DomainEvent.OccuredOn, notification.DomainEvent.ClientIdentityId));
    }
}


public class TestHandl : IIntegrationEventHandler<NewClientCreatedIntegrationEvent>
{
    public async Task HandleAsync(NewClientCreatedIntegrationEvent integrationEvent)
    {
        Console.WriteLine("HANDLE INTE");
    }
}

