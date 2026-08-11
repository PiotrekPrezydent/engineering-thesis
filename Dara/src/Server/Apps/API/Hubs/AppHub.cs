using Dara.Server.Apps.API.Extensions;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.CreateGroup;
using Dara.Server.Modules.Groups.Application.JoinToGroup;
using Dara.Server.Modules.Groups.Application.LeaveGroup;
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
        var command = new CreateGroupCommand(Context.GuidIdentifier(), groupName);
        await _groupsModule.ExecuteCommandAsync<CreateGroupCommand,Guid>(command);
    }

    public async Task JoinGroup(Guid groupId, string joinCode)
    {
        await _groupsModule.ExecuteCommandAsync(new JoinToGroupCommand(groupId,Context.GuidIdentifier(),joinCode));
    }

    public async Task LeaveGroup(Guid groupId)
    {
        await _groupsModule.ExecuteCommandAsync(new LeaveGroupCommand(groupId, Context.GuidIdentifier()));
    }
}
