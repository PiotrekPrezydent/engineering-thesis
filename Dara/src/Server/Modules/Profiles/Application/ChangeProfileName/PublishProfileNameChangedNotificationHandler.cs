using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Profiles.Integration;

namespace Dara.Server.Modules.Profiles.Application.ChangeProfileName;

public class ProfileNameChangedPublisher : IDomainEventNotificationHandler<ProfileNameChangedNotification>
{
    private readonly IEventBus _eventBus;

    public ProfileNameChangedPublisher(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task HandleAsync(ProfileNameChangedNotification notification)
    {
       await  _eventBus.PublishAsync(new ProfileNameChangedIntegrationEvent(
           notification.NotificationId,
           notification.DomainEvent.OccuredOn, 
           notification.DomainEvent.ProfileId, 
           notification.DomainEvent.NewName));
    }
}