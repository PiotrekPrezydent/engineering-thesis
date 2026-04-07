using Dara.Core.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Application.Device;

public record RevokeUserDeviceCommand(Guid userId, Guid deviceId) : IApplicationCommand;

class RevokeUserDeviceCommandHandler : IApplicationCommandHandler<RevokeUserDeviceCommand>
{
    private readonly IUserRepository _users;

    public RevokeUserDeviceCommandHandler(IUserRepository users)
    {
        _users = users;
    }

    public Task HandleAsync(RevokeUserDeviceCommand command)
    {
        var user = _users.GetUserById(command.userId).Result;
        user.RemoveDevice(command.deviceId);
        
        return _users.Save(user);
    }
}