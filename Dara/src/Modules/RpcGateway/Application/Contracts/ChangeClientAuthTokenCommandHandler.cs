using Dara.BuildingBlocks.Application;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Modules.RpcGateway.Domain;

namespace Dara.Modules.RpcGateway.Application.Contracts;

public class ChangeClientAuthTokenCommandHandler : IApplicationCommandHandler<ChangeClientAuthTokenCommand, ChangeClientAuthTokenCommandResult>
{
    private readonly IClientConnectionRepository _clientConnectionRepository;

    public ChangeClientAuthTokenCommandHandler(IClientConnectionRepository clientConnectionRepository)
    {
        _clientConnectionRepository = clientConnectionRepository;
    }

    public async Task<ChangeClientAuthTokenCommandResult> HandleAsync(ChangeClientAuthTokenCommand command)
    {
        return new();
    }
}
