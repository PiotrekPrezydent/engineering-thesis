using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Modules.RpcGateway.Domain;

namespace Dara.Modules.RpcGateway.Application.Contracts;

public class ChangeClientNameCommandHandler : IApplicationCommandHandler<ChangeClientNameCommand, ChangeClientNameCommandResult>
{
    private readonly IClientConnectionRepository _clientConnectionRepository;

    public ChangeClientNameCommandHandler(IClientConnectionRepository clientConnectionRepository)
    {
        _clientConnectionRepository = clientConnectionRepository;
    }

    public async Task<ChangeClientNameCommandResult> HandleAsync(ChangeClientNameCommand command)
    {
        Console.WriteLine("CLIENT CHANGE NAME");
        return new();
    }
}
