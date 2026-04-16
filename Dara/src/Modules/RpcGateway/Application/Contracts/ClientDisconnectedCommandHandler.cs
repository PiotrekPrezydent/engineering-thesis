using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.RpcGateway.Contracts;

namespace Dara.Modules.RpcGateway.Application.Contracts;

public class ClientDisconnectedCommandHandler : IApplicationCommandHandler<ClientDisconnectedCommand, ClientConnectedCommandResult>
{
    public Task<ClientConnectedCommandResult> HandleAsync(ClientDisconnectedCommand command)
    {
        throw new NotImplementedException();
    }
}