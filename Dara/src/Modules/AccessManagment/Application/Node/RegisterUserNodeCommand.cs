using Dara.Core.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Application.Node;

public record RegisterUserNodeCommand(Guid userId, string deviceName) : IApplicationCommand; // device hash will be getted from name

class RegisterUserDeviceCommandHandler : IApplicationCommandHandler<RegisterUserNodeCommand>
{
    private readonly IUserRepository _users;

    public RegisterUserDeviceCommandHandler(IUserRepository users)
    {
        _users = users;
    }

    public Task HandleAsync(RegisterUserNodeCommand command)
    {
        var user = _users.GetUserById(command.userId).Result;
        
        user.AddNode(command.deviceName, new(command.deviceName + "DEVICETOKEN"));
        
        Console.WriteLine("device added");
        return _users.Save(user);
    }
}