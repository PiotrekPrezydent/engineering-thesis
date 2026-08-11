using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Processing;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Profiles.Integration;

namespace Dara.Server.Apps.API.Notifications;

public class NotifyProfileNameChangedHandler : IHubNotificationHandler<ProfileNameChangedIntegrationEvent>
{
    public async Task HandleAsync(ProfileNameChangedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        return;
    }
}