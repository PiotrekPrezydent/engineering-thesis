using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Shared.Contracts;

public interface IAppHubClient
{
    public Task OnProfileNameChanged(ProfileNameChangedNotification notification);
    
    public Task OnGroupCreated(GroupCreatedNotification notification);

    public Task OnGroupMemberUpdated(GroupMemberSnapshot groupMemberSnapshot);
    
    public Task OnGroupJoined(GroupSnapshot groupSnapshot);
    
    public Task OnGroupLeft(Guid groupId);
    
    public Task OnGroupMemberJoined(GroupMemberJoinedGroupNotification notification);
    
    public Task OnGroupMemberLeft(GroupMemberLeftGroupNotification notification);
}