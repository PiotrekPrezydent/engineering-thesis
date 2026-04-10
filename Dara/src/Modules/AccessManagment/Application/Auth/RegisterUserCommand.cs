using Dara.Core.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Auth;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Application.Auth;

public record RegisterUserCommand(string email, string password, string nickname) : IApplicationCommand;

public record RegisterUserCommandResult(Guid userId) : IApplicationCommandResult;

public class RegisterUserCommandHandler : IApplicationCommandHandler<RegisterUserCommand, RegisterUserCommandResult>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserCommandResult> HandleAsync(RegisterUserCommand command)
    {
        UserEmail email = new UserEmail(command.email);
        UserPassword password = new UserPassword(_passwordHasher.HashPassword(command.password));
        UserNickname nickname = new UserNickname(command.nickname);

        await _users.Add(new(email, password, nickname));
        var usr = _users.GetUserByEmail(email).Result;
        Console.WriteLine("user added " + usr.Id);

        return new RegisterUserCommandResult(usr.Id);
    }
}