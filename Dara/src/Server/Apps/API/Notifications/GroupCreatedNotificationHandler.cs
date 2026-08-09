using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Processing;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.GetGroupDetails;
using Dara.Server.Modules.Groups.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Notifications;

public class GroupCreatedNotificationHandler : IHubNotificationHandler<GroupCreatedIntegrationEvent>
{ 
    private readonly IGroupModule _groupModule;
    private readonly IHubContext<AppHub, IAppHubClient> _context;
    
    public GroupCreatedNotificationHandler(IGroupModule groupModule, IHubContext<AppHub, IAppHubClient> context)
    {
        _groupModule = groupModule;
        _context = context;
    }

    public async Task HandleAsync(GroupCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.User(notification.GroupOwnerId.ToString());
        var details = await _groupModule.ExecuteQueryAsync<GetGroupDetailsQuery,GroupDetailsDto>(new GetGroupDetailsQuery(notification.GroupId));
        
        await client.OnGroupCreated(new GroupCreatedNotification(details.GroupId, details.GroupName, details.JoinCode));
    }
}