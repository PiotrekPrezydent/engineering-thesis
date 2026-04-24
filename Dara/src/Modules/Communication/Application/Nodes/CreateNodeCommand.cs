using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Application.Nodes;

public record CreateNodeCommand(string ConnectionId, string NodeName, string NodeAuthToken) : IApplicationCommand;

public record CreateNodeCommandResult(Guid NodeId) : IApplicationCommandResult;

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
        ClientConnectionId id = new(command.ConnectionId);

        Client client = await _clientsReposiotory.GetByClientConnectionIdAsync(id);
        
        NodeName name = new(command.NodeName);
        NodeAuthToken token = new(command.NodeAuthToken);

        Node node = new Node(name, token, client);
        client.EnableNode(node);

        await _nodesReposiotory.AddAsync(node);
        
        return new(node.Id);
    }
}