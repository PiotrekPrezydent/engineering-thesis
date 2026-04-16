using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.RpcGateway.Contracts;

namespace Dara.Modules.RpcGateway.Application.Contracts;

public class ClientConnectedCommandHandler : IApplicationCommandHandler<ClientConnectedCommand, ClientConnectedCommandResult>
{
    public Task<ClientConnectedCommandResult> HandleAsync(ClientConnectedCommand command)
    {
        throw new NotImplementedException();
    }
}
