using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.Clients;

namespace Dara.Server.Modules.Identity.Application.RenameClient;

// a client of another account breaks ClientMustBelongToUserRule
public class RenameClientCommandHandler : ICommandHandler<RenameClientCommand>
{
    private readonly IUserRepository _users;

    public RenameClientCommandHandler(IUserRepository users)
    {
        _users = users;
    }

    public async Task HandleAsync(RenameClientCommand command)
    {
        var user = await _users.GetByIdAsync(new UserId(command.UserId));
        user.RenameClient(new UserClientId(command.ClientId), command.Name);
    }
}
