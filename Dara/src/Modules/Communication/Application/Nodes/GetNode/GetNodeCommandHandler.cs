using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Application.Nodes.GetNode;

public class GetNodeCommandHandler : IApplicationCommandHandler<GetNodeCommand, GetNodeCommandResult>
{
    private readonly IClientsRepository _clientsReposiotory;
    private readonly INodesReposiotory _nodesReposiotory;
    
    public GetNodeCommandHandler(IClientsRepository clientsReposiotory,  INodesReposiotory nodesReposiotory)
    {
        _clientsReposiotory = clientsReposiotory;
        _nodesReposiotory = nodesReposiotory;
    }

    public async Task<GetNodeCommandResult> HandleAsync(GetNodeCommand commandHandler)
    {
        Client client = await _clientsReposiotory.GetByGuidAsync(commandHandler.ClientId);

        Domain.Nodes.Node node = await _nodesReposiotory.GetByNodeClientOwnerAsync(client);
        
        return new(node.Id, node.Name.Value, node.AuthToken.Token);
    }
}