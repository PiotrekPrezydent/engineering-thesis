using Dara.Core.Domain.Commands;

namespace Dara.Modules.AccessManagment.Application.Auth;

public record RegisterUserCommand(string email, string password, string nickname) : IApplicationCommand;

class RegisterUserCommandHandler : IApplicationCommandHandler<RegisterUserCommand>
{
    public Task HandleAsync(RegisterUserCommand command)
    {
        throw new NotImplementedException();
    }
}