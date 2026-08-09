using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.CreateGroup;
using Dara.Server.Modules.Groups.Application.GetValidGroup;
using Dara.Server.Modules.Groups.Application.JoinToGroup;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Application.ChangeProfileName;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Hubs;

public partial class AppHub : Hub<IAppHubClient>, IAppHub
{
    private readonly IIdentityModule _identityModule;
    private readonly IGroupModule _groupModule;
    private readonly IProfilesModule _profilesModule;

    public static IHubContext<AppHub, IAppHubClient> InstanceContext;

    public AppHub(IIdentityModule identityModule, IProfilesModule profilesModule, IGroupModule groupModule)
    {
        _identityModule = identityModule;
        _profilesModule = profilesModule;
        _groupModule = groupModule;
    }
    
    public static IAppHubClient GetClientByGuid(Guid guid) => InstanceContext.Clients.User(guid.ToString());

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected - GUID: {Context.GuidIdentifier()}");
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
        await _groupModule.ExecuteCommandAsync<CreateGroupCommand>(command);
    }

    public async Task JoinGroup(string joinCode)
    {
        var groupId = await _groupModule.ExecuteQueryAsync<GetValidGroupQuery, Guid?>(new GetValidGroupQuery(joinCode));
        if(groupId == null)
            throw new HubException("Group not found");
        
        await _groupModule.ExecuteCommandAsync(new JoinToGroupCommand(groupId.Value,Context.GuidIdentifier()));
    }

    public async Task LeaveGroup(string groupName)
    {
        //var groupId = _groupModule.ExecuteQueryAsync<>()
        throw new NotImplementedException();
    }
}
