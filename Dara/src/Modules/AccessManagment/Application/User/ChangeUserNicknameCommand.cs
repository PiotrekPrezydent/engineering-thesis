using Dara.Core.Domain.Commands;

namespace Dara.Modules.AccessManagment.Application.User;

public record ChangeUserNicknameCommand(Guid userId, string newNickname) : IApplicationCommand;

class ChangeUserNicknameCommandHandler : IApplicationCommandHandler<ChangeUserNicknameCommand>
{
    public Task HandleAsync(ChangeUserNicknameCommand command)
    {
        throw new NotImplementedException();
    }
}