namespace Dara.Shared.Contracts;

public interface IAppHub
{
    public Task ChangeName(string name);

    public Task CreateGroup(string groupName);
    
    public Task JoinGroup(Guid groupId, string joinCode);
    
    public Task LeaveGroup(Guid groupId);
    
    public Task SendGroupMessage(Guid groupId, string message);
    
    public Task RegisterPlugin(PluginData pluginData);
}

