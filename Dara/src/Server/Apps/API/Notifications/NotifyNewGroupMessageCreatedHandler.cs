using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Processing;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.GetGroupDetails;
using Dara.Server.Modules.Groups.Integration;
using Dara.Server.Modules.Profiles.Application;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Notifications;

public class NotifyNewGroupMessageCreatedHandler : IHubNotificationHandler<GroupMessageAddedIntegrationEvent>
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

    public async Task HandleAsync(GroupMessageAddedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var group = await _groupModule.ExecuteQueryAsync<GetGroupDetailsQuery,GroupDetailsDto>(new GetGroupDetailsQuery(notification.GroupId));
        var messageNotification = new GroupMessageReceivedNotification(notification.GroupId, notification.MessageId, notification.AuthorId, notification.Content);

        // the author too, so its other connections see the message
        foreach (var member in group.Members)
        {
            var memberClient = _context.Clients.User(member.ToString());
            await memberClient.OnGroupMessageReceived(messageNotification);
        }
    }
}