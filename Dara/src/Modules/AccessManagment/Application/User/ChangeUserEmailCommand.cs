using Dara.Core.Domain.Commands;

namespace Dara.Modules.AccessManagment.Application.User;

public record ChangeUserEmailCommand(Guid userId, string newEmail) : IApplicationCommand;

class ChangeUserEmailCommandHandler : IApplicationCommandHandler<ChangeUserEmailCommand>
{
    public Task HandleAsync(ChangeUserEmailCommand command)
    {
        throw new NotImplementedException();
    }
}