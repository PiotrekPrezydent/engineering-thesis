using Dara.Core.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Auth;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Application.Device;

public record RegisterUserDeviceCommand(Guid userId, string deviceName) : IApplicationCommand; // device hash will be getted from name

class RegisterUserDeviceCommandHandler : IApplicationCommandHandler<RegisterUserDeviceCommand>
{
    private readonly IUserRepository _users;

    public RegisterUserDeviceCommandHandler(IUserRepository users)
    {
        _users = users;
    }

    public Task HandleAsync(RegisterUserDeviceCommand command)
    {
        var user = _users.GetUserById(command.userId).Result;
        
        user.AddDevice(command.deviceName, new(command.deviceName + "DEVICETOKEN"));
        
        Console.WriteLine("device added");
        return _users.Save(user);
    }
}