using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.Clients;

namespace Dara.Server.Modules.Identity.Application.ChangeClientContext;

public class ChangeClientContextCommandHandler : ICommandHandler<ChangeClientContextCommand>
{
    private readonly IUserRepository _users;

    public ChangeClientContextCommandHandler(IUserRepository users)
    {
        _users = users;
    }

    public async Task HandleAsync(ChangeClientContextCommand command)
    {
        var user = await _users.GetByIdAsync(new UserId(command.UserId));
        user.ChangeClientContext(new UserClientId(command.ClientId), command.Entries);
    }
}
