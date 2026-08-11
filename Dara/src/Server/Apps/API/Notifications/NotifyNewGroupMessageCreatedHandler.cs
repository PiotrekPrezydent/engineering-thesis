using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Processing;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.GetGroupDetails;
using Dara.Server.Modules.Groups.Integration;
using Dara.Server.Modules.Profiles.Application;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Notifications;

public class NotifyNewGroupMessageCreatedHandler : IHubNotificationHandler<NewGroupMessageCreatedIntegrationEvent>
{
    private readonly IGroupsModule _groupModule;
    private readonly IProfilesModule _profilesModule;
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyNewGroupMessageCreatedHandler(IGroupsModule groupModule, IProfilesModule profilesModule, IHubContext<AppHub, IAppHubClient> context)
    {
        _groupModule = groupModule;
        _profilesModule = profilesModule;
        _context = context;
    }

    public async Task HandleAsync(NewGroupMessageCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var group = await _groupModule.ExecuteQueryAsync<GetGroupDetailsQuery,GroupDetailsDto>(new GetGroupDetailsQuery(notification.GroupId));
        foreach (var member in group.Members)
        {
            Console.WriteLine($"SEND MESSAGE {notification.Content} TO {member}");
        }
        Console.WriteLine("");
    }
}