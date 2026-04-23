using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Application.Nodes;

public record DeleteNodeCommand(string ConnectionId) : IApplicationCommand;

public record DeleteNodeCommandResult() : IApplicationCommandResult;

public class DeleteNodeCommandHandler : IApplicationCommandHandler<DeleteNodeCommand, DeleteNodeCommandResult>
{
    private readonly IClientsRepository _clientsReposiotory;
    private readonly INodesReposiotory _nodesReposiotory;
    public DeleteNodeCommandHandler(INodesReposiotory nodesReposiotory, IClientsRepository clientsReposiotory)
    {
        _nodesReposiotory = nodesReposiotory;
        _clientsReposiotory = clientsReposiotory;
    }

    public async Task<DeleteNodeCommandResult> HandleAsync(DeleteNodeCommand command)
    {
        ClientConnectionId id = new(command.ConnectionId);

        Client client = await _clientsReposiotory.GetByClientConnectionIdAsync(id);
        Node node = await _nodesReposiotory.GetByNodeClientOwnerAsync(client);

        await _nodesReposiotory.RemoveAsync(node);
        
        client.DisableNode();

        return new();
    }
}