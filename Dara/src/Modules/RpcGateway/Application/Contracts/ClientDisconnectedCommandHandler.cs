using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Modules.RpcGateway.Domain;

namespace Dara.Modules.RpcGateway.Application.Contracts;

public class ClientDisconnectedCommandHandler : IApplicationCommandHandler<ClientDisconnectedCommand, ClientDisconnectedCommandResult>
{
    private readonly IClientConnectionRepository _clientConnectionRepository;

    public ClientDisconnectedCommandHandler(IClientConnectionRepository clientConnectionRepository)
    {
        _clientConnectionRepository = clientConnectionRepository;
    }

    public async Task<ClientDisconnectedCommandResult> HandleAsync(ClientDisconnectedCommand command)
    {
        ConnectionId conId = new(command.ConnectionId);
        
        var client = await _clientConnectionRepository.FindByConnectionId(conId);
        
        await _clientConnectionRepository.RemoveAsync(client);

        return new(command.ConnectionId);
    }
}