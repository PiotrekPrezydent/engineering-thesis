using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Auth;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Application.User;

public record ChangeUserPasswordCommand(Guid userId, string newPassword) : IApplicationCommand;

public class ChangeUserPasswordCommandHandler : IApplicationCommandHandler<ChangeUserPasswordCommand>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public ChangeUserPasswordCommandHandler(IPasswordHasher passwordHasher, IUserRepository users)
    {
        _passwordHasher = passwordHasher;
        _users = users;
    }


    public Task HandleAsync(ChangeUserPasswordCommand command)
    {
        UserPassword password = new(_passwordHasher.HashPassword(command.newPassword));

        var user = _users.GetUserById(command.userId).Result;
        
        user.ChangePassword(password);
        return _users.Save(user);
    }
}