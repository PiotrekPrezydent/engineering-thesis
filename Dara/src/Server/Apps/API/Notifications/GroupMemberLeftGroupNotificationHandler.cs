// using Dara.Server.Apps.API.Hubs;
// using Dara.Server.Apps.API.Processing;
// using Dara.Server.Modules.Groups.Application;
// using Dara.Server.Modules.Groups.Application.GetGroupDetails;
// using Dara.Server.Modules.Groups.Integration;
// using Dara.Server.Modules.Profiles.Application;
// using Dara.Server.Modules.Profiles.Application.GetProfile;
// using Dara.Shared.Contracts;
// using Dara.Shared.Contracts.Notifications;
// using Microsoft.AspNetCore.SignalR;
//
// namespace Dara.Server.Apps.API.Notifications;
//
// public class GroupMemberLeftGroupNotificationHandler : IHubNotificationHandler<MemberLeftGroupIntegrationEvent>
// {
//     private readonly IGroupModule _groupModule;
//     private readonly IProfilesModule _profilesModule;
//     private readonly IHubContext<AppHub, IAppHubClient> _context;
//     
//     public GroupMemberLeftGroupNotificationHandler(IGroupModule groupModule, IProfilesModule profilesModule, IHubContext<AppHub, IAppHubClient> context)
//     {
//         _groupModule = groupModule;
//         _profilesModule = profilesModule;
//         _context = context;
//     }
//
//     public async Task HandleAsync(MemberLeftGroupIntegrationEvent notification, CancellationToken cancellationToken)
//     {
//         var client = _context.Clients.User(notification.MemberId.ToString());
//         var group = await _groupModule.ExecuteQueryAsync<GetGroupDetailsQuery, GroupDetailsDto>(new GetGroupDetailsQuery(notification.GroupId));
//         var clientProfile = await _profilesModule.ExecuteQueryAsync<GetProfileQuery, ProfileDto>(new GetProfileQuery(notification.MemberId));
//         var memberLeftNotification =
//             new GroupMemberLeftGroupNotification(notification.GroupId, new(clientProfile.Id, clientProfile.Name));
//         
//         await client.OnGroupLeft(notification.GroupId);
//         foreach (var member in group.Members)
//         {
//             var memberClient = _context.Clients.User(member.ToString());
//             await memberClient.OnGroupMemberLeft(memberLeftNotification);
//         }
//         
//     }
// }