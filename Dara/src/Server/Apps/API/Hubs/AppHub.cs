using Dara.Server.Apps.API.Extensions;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;
using Dara.Server.Modules.Groups.Application.Groups.JoinToGroup;
using Dara.Server.Modules.Groups.Application.Groups.LeaveGroup;
using Dara.Server.Modules.Groups.Application.Messages.SendGroupMessage;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Application.ChangeProfileName;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Hubs;

public partial class AppHub : Hub<IAppHubClient>, IAppHub
{
    private readonly IIdentityModule _identityModule;
    private readonly IGroupsModule _groupsModule;
    private readonly IProfilesModule _profilesModule;

    public AppHub(IIdentityModule identityModule, IProfilesModule profilesModule, IGroupsModule groupsModule)
    {
        _identityModule = identityModule;
        _profilesModule = profilesModule;
        _groupsModule = groupsModule;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected - GUID: {Context.GuidIdentifier()}");
        await Clients.Caller.OnGroupLeft(Context.GuidIdentifier());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {

        Console.WriteLine($"{Context.ConnectionId} DISCONNECTED - GUID: {Context.GuidIdentifier()}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task ChangeName(string name)
    {
        var command = new ChangeProfileNameCommand(Context.GuidIdentifier(), name);
        await _profilesModule.ExecuteCommandAsync<ChangeProfileNameCommand>(command);
    }

    public async Task CreateGroup(string groupName)
    {
        var command = new CreateNewGroupCommand(Context.GuidIdentifier(), groupName,"DEFAULT-CODE"+Random.Shared.Next());
        await _groupsModule.ExecuteCommandAsync<CreateNewGroupCommand,Guid>(command);
    }

    public async Task JoinGroup(Guid groupId, string joinCode)
    {
        await _groupsModule.ExecuteCommandAsync(new JoinToGroupCommand(groupId,Context.GuidIdentifier(),joinCode));
    }

    public async Task LeaveGroup(Guid groupId)
    {
        await _groupsModule.ExecuteCommandAsync(new LeaveGroupCommand(groupId, Context.GuidIdentifier()));
    }

    public async Task SendGroupMessage(Guid groupId, string message)
    {
        await _groupsModule.ExecuteCommandAsync(new SendGroupMessageCommand(groupId, Context.GuidIdentifier(), message));
    }

    public async Task RegisterPlugin(PluginData pluginData)
    {
        throw new NotImplementedException();
    }
}
