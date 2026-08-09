using Dara.Server.Apps.API.Hubs;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Profiles.Integration;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Events;

public class ProfileNameChangedEventHandler : IIntegrationEventHandler<ProfileNameChangedIntegrationEvent>
{
    private readonly IHubContext<AppHub> _context;

    public ProfileNameChangedEventHandler(IHubContext<AppHub> context)
    {
        _context = context;
    }

    public async Task HandleAsync(ProfileNameChangedIntegrationEvent integrationEvent)
    {
        var client = _context.Clients.Client(integrationEvent.ProfileId.ToString());
        // inform profile, and his groups to update ui
    }
}