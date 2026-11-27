using Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;
using Dara.Server.Modules.Groups.Application.Groups.JoinToGroup;
using Dara.Server.Modules.Groups.Application.Groups.LeaveGroup;
using Dara.Server.Modules.Groups.Application.Messages.SendGroupMessage;

// the namespace of the hub, not of this folder: a partial class lives in one namespace
namespace Dara.Server.Apps.API.Common.Hubs;

public partial class AppHub
{
    public async Task CreateGroup(string groupName)
    {
        var command = new CreateNewGroupCommand(Context.ClientId(), groupName, "DEFAULT-CODE" + Random.Shared.Next());
        await _groupsModule.ExecuteCommandAsync<CreateNewGroupCommand, Guid>(command);
    }

    public async Task JoinGroup(Guid groupId, string joinCode)
    {
        await _groupsModule.ExecuteCommandAsync(new JoinToGroupCommand(groupId, Context.ClientId(), joinCode));
    }

    public async Task LeaveGroup(Guid groupId)
    {
        await _groupsModule.ExecuteCommandAsync(new LeaveGroupCommand(groupId, Context.ClientId()));
    }

    public async Task SendGroupMessage(Guid groupId, string message)
    {
        await _groupsModule.ExecuteCommandAsync(new SendGroupMessageCommand(groupId, Context.ClientId(), message));
    }
}
