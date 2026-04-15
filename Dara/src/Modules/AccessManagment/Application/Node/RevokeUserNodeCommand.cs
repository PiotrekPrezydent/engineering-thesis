using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Application.Node;

public record RevokeUserNodeCommand(Guid userId, Guid deviceId) : IApplicationCommand;

class RevokeUserDeviceCommandHandler : IApplicationCommandHandler<RevokeUserNodeCommand>
{
    private readonly IUserRepository _users;

    public RevokeUserDeviceCommandHandler(IUserRepository users)
    {
        _users = users;
    }

    public Task HandleAsync(RevokeUserNodeCommand command)
    {
        var user = _users.GetUserById(command.userId).Result;
        user.RemoveNode(command.deviceId);
        
        return _users.Save(user);
    }
}