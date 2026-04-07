using Dara.Core.Domain.Commands;

namespace Dara.Modules.AccessManagment.Application.Auth;

public record LoginUserCommand(string email, string password) : IApplicationCommand;

class LoginUserCommandHandler : IApplicationCommandHandler<LoginUserCommand>
{
    public Task HandleAsync(LoginUserCommand command)
    {
        throw new NotImplementedException();
    }
}