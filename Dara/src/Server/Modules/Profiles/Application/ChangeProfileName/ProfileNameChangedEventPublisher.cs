using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Profiles.Domain.Events;
using Dara.Server.Modules.Profiles.Integration;

namespace Dara.Server.Modules.Profiles.Application.ChangeProfileName;

public class ProfileNameChangedEventPublisher : IDomainEventNotificationHandler<ProfileNameChangedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public ProfileNameChangedEventPublisher(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(ProfileNameChangedDomainEvent notification)
    {
        await _eventBus.PublishAsync(
            new ProfileNameChangedIntegrationEvent(notification.ProfileId.Value, notification.NewName));
    }
}