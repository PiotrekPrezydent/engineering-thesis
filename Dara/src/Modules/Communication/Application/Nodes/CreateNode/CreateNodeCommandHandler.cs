using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Application.Nodes.CreateNode;

public class CreateNodeCommandHandler : IApplicationCommandHandler<CreateNodeCommand, CreateNodeCommandResult>
{
    private readonly INodesReposiotory _nodesReposiotory;
    private readonly IClientsRepository _clientsReposiotory;
    public CreateNodeCommandHandler(INodesReposiotory nodesReposiotory,  IClientsRepository clientsReposiotory)
    {
        _nodesReposiotory = nodesReposiotory;
        _clientsReposiotory = clientsReposiotory;
    }

    public async Task<CreateNodeCommandResult> HandleAsync(CreateNodeCommand command)
    {
        Client client = await _clientsReposiotory.GetByGuidAsync(command.ClientId);
        
        NodeName name = new(command.NodeName);
        NodeAuthToken token = new(command.NodeAuthToken);

        Domain.Nodes.Node node = new Domain.Nodes.Node(name, token, client);
        client.EnableNode(node); // this should be in domain event i think

        await _nodesReposiotory.AddAsync(node);
        
        return new(node.Id);
    }
}