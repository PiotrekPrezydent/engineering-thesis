using Dara.Server.Apps.API.Common.Hubs;
using Dara.Server.Apps.API.Common.Notifications;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.GetGroupDetails;
using Dara.Server.Modules.Groups.Integration;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Application.GetClients;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Modules.Groups.Notifications;

public class NotifyNewMemberJoinedGroupHandler : IHubNotificationHandler<MemberJoinedGroupIntegrationEvent>
{
    private readonly IGroupsModule _groupModule;
    private readonly IIdentityModule _identityModule;
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyNewMemberJoinedGroupHandler(IGroupsModule groupModule, IIdentityModule identityModule, IHubContext<AppHub, IAppHubClient> context)
    {
        _groupModule = groupModule;
        _identityModule = identityModule;
        _context = context;
    }

    public async Task HandleAsync(MemberJoinedGroupIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.Group(HubGroups.Client(notification.MemberId));
        var group = await _groupModule.ExecuteQueryAsync<GetGroupDetailsQuery,GroupDetailsDto>(new GetGroupDetailsQuery(notification.GroupId));
        
        var joining = await _identityModule.ExecuteQueryAsync<GetClientQuery, ClientDto?>(new GetClientQuery(notification.MemberId));
        if (joining is null)
            return;
        var membersClients = await _identityModule.ExecuteQueryAsync<GetClientsQuery, List<ClientDto>>(new GetClientsQuery(group.Members.ToArray()));

        await client.OnGroupJoined(
            new GroupSnapshot(
                group.GroupId, 
                group.OwnerId, 
                group.GroupName, 
                group.JoinCode,
            membersClients.Select(e => 
                new GroupMemberSnapshot(e.ClientId, e.Name))
                .ToList()));
        
        group.Members.Remove(notification.MemberId);

        var groupMemberJoinedNotification = new GroupMemberJoinedGroupNotification(notification.GroupId,
            new GroupMemberSnapshot(joining.ClientId, joining.Name));
        
        // one call for every member
        await _context.Clients.Groups(HubGroups.Clients(group.Members)).OnGroupMemberJoined(groupMemberJoinedNotification);
    }
}