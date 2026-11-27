using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Domain.Clients;

namespace Dara.Server.Modules.Identity.Application.RenameClient;

// a client of another account breaks ClientMustBelongToUserRule
public class RenameClientCommandHandler : ICommandHandler<RenameClientCommand>
{
    private readonly IClientRepository _clients;

    public RenameClientCommandHandler(IClientRepository clients)
    {
        _clients = clients;
    }

    public async Task HandleAsync(RenameClientCommand command)
    {
        var client = await _clients.GetAsync(command.UserId, command.ClientId);
        client.Rename(command.Name);
    }
}
