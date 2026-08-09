using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Domain.Events;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Identity.Application.CreateClient;

public class NewClientCreatedEventPublisher : IDomainEventNotificationHandler<NewClientCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public NewClientCreatedEventPublisher(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    public async Task HandleAsync(NewClientCreatedDomainEvent notification)
    {
        await _eventBus.PublishAsync(new NewClientCreatedIntegrationEvent(notification.ClientId.Value));
    }
}