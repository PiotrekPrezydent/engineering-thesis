using Dara.BuildingBlocks.Application;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Modules.RpcGateway.Domain;
using Dara.Modules.RpcGateway.Domain.Clients;
using Dara.Modules.RpcGateway.Domain.Connections;

namespace Dara.Modules.RpcGateway.Application.Contracts;

public class ClientConnectedCommandHandler : IApplicationCommandHandler<ClientConnectedCommand, ClientConnectedCommandResult>
{
    private readonly IClientConnectionRepository _clientConnectionRepository;

    public ClientConnectedCommandHandler(IClientConnectionRepository clientConnectionRepository)
    {
        _clientConnectionRepository = clientConnectionRepository;
    }

    public async Task<ClientConnectedCommandResult> HandleAsync(ClientConnectedCommand command)
    {
        ConnectionId conId = new(command.ConnectionId);
        ConnectionIp conIp = new(command.ConnectionIp);
        ClientName name = new("SAMPEL NAME");
        ClientAuthToken token = new("SAMPLE TOKEN");
        
        var client = new Client(conId, conIp, name, token);
        
        await _clientConnectionRepository.AddAsync(client);

        return new();
    }
}
