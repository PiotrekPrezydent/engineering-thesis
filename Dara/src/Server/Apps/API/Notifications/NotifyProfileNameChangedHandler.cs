using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Processing;
using Dara.Server.Modules.Profiles.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Notifications;

public class NotifyProfileNameChangedHandler : IHubNotificationHandler<ProfileNameChangedIntegrationEvent>
{
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyProfileNameChangedHandler(IHubContext<AppHub, IAppHubClient> context)
    {
        _context = context;
    }

    // only the connections of the user, groups has no query for the groups of a member yet
    public async Task HandleAsync(ProfileNameChangedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.User(notification.ProfileId.ToString());

        await client.OnProfileNameChanged(new ProfileNameChangedNotification(notification.ProfileId, notification.NewName));
    }
}