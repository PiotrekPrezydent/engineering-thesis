namespace Dara.Shared.Contracts;

public interface IAppHub
{
    public Task ChangeName(string name);

    public Task CreateGroup(string groupName);
    
    public Task JoinGroup(Guid groupId, string joinCode);
    
    public Task LeaveGroup(Guid groupId);
}

