using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.RpcGateway.Contracts;

namespace Dara.Modules.RpcGateway.Application.Contracts;

public class ChangeClientNameCommandHandler : IApplicationCommandHandler<ChangeClientNameCommand, ChangeClientNameCommandResult>
{
    public Task<ChangeClientNameCommandResult> HandleAsync(ChangeClientNameCommand command)
    {
        throw new NotImplementedException();
    }
}
