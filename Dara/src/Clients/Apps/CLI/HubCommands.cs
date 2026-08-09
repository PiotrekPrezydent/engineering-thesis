using Dara.Shared.Common.CLI;
using Dara.Shared.Contracts;

namespace Dara.Clients.Apps.CLI;

public class HubCommands : IAppHub
{
    public static IAppHub Proxy { get; set; }
    
    public HubCommands()
    {
    }
    
    [CLICommand("cn")]
    public async Task ChangeName(string name)
    {
        await Proxy.ChangeName(name);
    }

    [CLICommand("cg")]
    public async Task CreateGroup(string groupName)
    {
        await Proxy.CreateGroup(groupName);
    }

    [CLICommand("jg")]
    public async Task JoinGroup(Guid groupId, string joinCode)
    {
        await Proxy.JoinGroup(groupId, joinCode);
    }

    [CLICommand("lg")]
    public async Task LeaveGroup(Guid groupId)
    {
        await Proxy.LeaveGroup(groupId);
    }
}