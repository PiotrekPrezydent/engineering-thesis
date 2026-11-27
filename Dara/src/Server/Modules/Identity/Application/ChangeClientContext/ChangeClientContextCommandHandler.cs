using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Domain.Clients;

namespace Dara.Server.Modules.Identity.Application.ChangeClientContext;

// a client of another account breaks ClientMustBelongToUserRule
public class ChangeClientContextCommandHandler : ICommandHandler<ChangeClientContextCommand>
{
    private readonly IClientRepository _clients;

    public ChangeClientContextCommandHandler(IClientRepository clients)
    {
        _clients = clients;
    }

    public async Task HandleAsync(ChangeClientContextCommand command)
    {
        var client = await _clients.GetAsync(command.UserId, command.ClientId);
        client.ChangeContext(command.Entries);
    }
}
