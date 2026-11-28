using Dara.Server.Apps.API.Common.Hubs;
using Dara.Server.Apps.API.Common.Notifications;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.GetGroupDetails;
using Dara.Server.Modules.Groups.Integration;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Modules.Groups.Notifications;

public class NotifyNewGroupMessageCreatedHandler : IHubNotificationHandler<GroupMessageAddedIntegrationEvent>
{
    private readonly IGroupsModule _groupModule;
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyNewGroupMessageCreatedHandler(IGroupsModule groupModule, IHubContext<AppHub, IAppHubClient> context)
    {
        _groupModule = groupModule;
        _context = context;
    }

    public async Task HandleAsync(GroupMessageAddedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var group = await _groupModule.ExecuteQueryAsync<GetGroupDetailsQuery,GroupDetailsDto>(new GetGroupDetailsQuery(notification.GroupId));
        var messageNotification = new GroupMessageReceivedNotification(notification.GroupId, notification.MessageId, notification.AuthorId, notification.Content);

        // the author too, so its other connections see the message
        // one call for every member
        await _context.Clients.Groups(HubGroups.Clients(group.Members)).OnGroupMessageReceived(messageNotification);
    }
}