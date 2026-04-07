using Dara.Core.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Auth;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Application.Auth;

public record RegisterUserCommand(string email, string password, string nickname) : IApplicationCommand;

public class RegisterUserCommandHandler : IApplicationCommandHandler<RegisterUserCommand>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public Task HandleAsync(RegisterUserCommand command)
    {
        UserEmail email = new UserEmail(command.email);
        UserPassword password = new UserPassword(_passwordHasher.HashPassword(command.password));
        UserNickname nickname = new UserNickname(command.nickname);

        _users.Add(new(email, password, nickname));
        Console.WriteLine("user added");
        
        return Task.CompletedTask;
    }
}