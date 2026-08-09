namespace Dara.Shared.Contracts;

public interface IAppHub
{
    public Task ChangeName(string name);

    public Task CreateGroup(string groupName);
    
    public Task JoinGroup(string joinCode);
    
    public Task LeaveGroup(string groupName);
}

