using Dara.Server.Apps.API.Hubs;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Profiles.Integration;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Events;

public class ProfileNameChangedEventHandler : IIntegrationEventHandler<ProfileNameChangedIntegrationEvent>
{
    private readonly IServiceProvider _serviceProvider;

    public ProfileNameChangedEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(ProfileNameChangedIntegrationEvent integrationEvent)
    {
        var client = AppHub.GetClientByGuid(integrationEvent.ProfileId);
        await client.OnProfileNameChanged(integrationEvent.NewName);
    }
}