namespace Dara.Shared.Contracts;

public interface IAppHubClient
{
    public Task OnProfileNameChanged(string newName);
    
    public Task OnGroupCreated(Guid groupId, string groupName, string joinCode, List<Guid> memberIds);

    public Task OnGroupMemberUpdated(Guid memberId);
    
    public Task OnGroupJoined(Guid groupId, string groupName, string joinCode, List<Guid> memberIds);
    
    public Task OnGroupLeft(Guid groupId);
    
    public Task OnGroupMemberJoined(Guid groupId, Guid memberId);
    
    public Task OnGroupMemberLeft(Guid groupId, Guid memberId);
}