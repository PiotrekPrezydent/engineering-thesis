using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Processing;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.GetGroupDetails;
using Dara.Server.Modules.Groups.Integration;
using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Application.GetProfile;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Notifications;

public class NotifyNewMemberJoinedGroupHandler : IHubNotificationHandler<NewMemberJoinedGroupIntegrationEvent>
{
    private readonly IGroupsModule _groupModule;
    private readonly IProfilesModule _profilesModule;
    private readonly IHubContext<AppHub, IAppHubClient> _context;

    public NotifyNewMemberJoinedGroupHandler(IGroupsModule groupModule, IProfilesModule profilesModule, IHubContext<AppHub, IAppHubClient> context)
    {
        _groupModule = groupModule;
        _profilesModule = profilesModule;
        _context = context;
    }

    public async Task HandleAsync(NewMemberJoinedGroupIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = _context.Clients.User(notification.MemberId.ToString());
        var group = await _groupModule.ExecuteQueryAsync<GetGroupDetailsQuery,GroupDetailsDto>(new GetGroupDetailsQuery(notification.GroupId));
        
        var clientProfile = await _profilesModule.ExecuteQueryAsync<GetProfileQuery,ProfileDto>(new GetProfileQuery(notification.MemberId));
        var membersProfiles = await _profilesModule.ExecuteQueryAsync<GetProfilesQuery, List<ProfileDto>>(new GetProfilesQuery(group.Members.ToArray()));

        await client.OnGroupJoined(
            new GroupSnapshot(
                group.GroupId, 
                group.OwnerId, 
                group.GroupName, 
                group.JoinCode,
            membersProfiles.Select(e => 
                new GroupMemberSnapshot(e.ProfileId, e.Name))
                .ToList()));
        
        group.Members.Remove(notification.MemberId);

        var groupMemberJoinedNotification = new GroupMemberJoinedGroupNotification(notification.GroupId,
            new GroupMemberSnapshot(clientProfile.ProfileId, clientProfile.Name));
        
        foreach (var member in group.Members)
        {
            var memberClient = _context.Clients.User(member.ToString());
            await memberClient.OnGroupMemberJoined(groupMemberJoinedNotification);
        }
    }
}