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
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Modules.Groups.Notifications;

public class NotifyMemberLeftGroupHandler : IHubNotificationHandler<MemberLeftGroupIntegrationEvent>
{
    private readonly IGroupsModule _groupModule;
    private readonly IIdentityModule _identityModule;
    private readonly IHubContext<AppHub, IAppHubClient> _context;
    
    public NotifyMemberLeftGroupHandler(IGroupsModule groupModule, IIdentityModule identityModule, IHubContext<AppHub, IAppHubClient> context)
    {
        _groupModule = groupModule;
        _identityModule = identityModule;
        _context = context;
    }

    public async Task HandleAsync(MemberLeftGroupIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.User(notification.MemberId.ToString());
        var group = await _groupModule.ExecuteQueryAsync<GetGroupDetailsQuery, GroupDetailsDto>(new GetGroupDetailsQuery(notification.GroupId));
        var leaving = await _identityModule.ExecuteQueryAsync<GetClientQuery, ClientDto?>(new GetClientQuery(notification.MemberId));
        if (leaving is null)
            return;
        var memberLeftNotification =
            new GroupMemberLeftGroupNotification(notification.GroupId, new(leaving.ClientId, leaving.Name));
        
        await client.OnGroupLeft(notification.GroupId);
        foreach (var member in group.Members)
        {
            var memberClient = _context.Clients.User(member.ToString());
            await memberClient.OnGroupMemberLeft(memberLeftNotification);
        }
        
    }
}