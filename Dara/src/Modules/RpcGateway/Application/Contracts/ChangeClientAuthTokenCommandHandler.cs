using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.RpcGateway.Contracts;

namespace Dara.Modules.RpcGateway.Application.Contracts;

public class ChangeClientAuthTokenCommandHandler : IApplicationCommandHandler<ChangeClientAuthTokenCommand, ChangeClientAuthTokenCommandResult>
{
    public Task<ChangeClientAuthTokenCommandResult> HandleAsync(ChangeClientAuthTokenCommand command)
    {
        throw new NotImplementedException();
    }
}
