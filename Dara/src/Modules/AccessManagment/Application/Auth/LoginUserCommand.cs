using Dara.Core.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Auth;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Application.Auth;

public record LoginUserCommand(string email, string password) : IApplicationCommand;

public class LoginUserCommandHandler : IApplicationCommandHandler<LoginUserCommand>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserCommandHandler(IUserRepository users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }
    
    public Task HandleAsync(LoginUserCommand command)
    {
        Console.WriteLine("login: " + command);
        //UserEmail email = new UserEmail(command.email);
        // var user = _users.GetUserByEmail(email).Result;
        //
        // //try catch
        //
        // if (!_passwordHasher.VerifyHashedPassword(user.Password.HashPassword, command.password))
        // {
        //     Console.WriteLine("Wrong password");
        //     return Task.CompletedTask;
        // }
        
        //Console.WriteLine($"Logged in: {user.Email}");
        return Task.CompletedTask;
    }
}