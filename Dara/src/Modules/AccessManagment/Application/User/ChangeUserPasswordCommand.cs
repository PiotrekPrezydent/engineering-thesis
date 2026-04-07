using Dara.Core.Domain.Commands;

namespace Dara.Modules.AccessManagment.Application.User;

public record ChangeUserPasswordCommand(Guid userId, string newPassword) : IApplicationCommand;

public class ChangeUserPasswordCommandHandler : IApplicationCommandHandler<ChangeUserPasswordCommand>
{
    public Task HandleAsync(ChangeUserPasswordCommand command)
    {
        throw new NotImplementedException();
    }
}