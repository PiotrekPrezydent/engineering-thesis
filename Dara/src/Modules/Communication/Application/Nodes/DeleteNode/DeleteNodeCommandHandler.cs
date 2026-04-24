using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Application.Nodes.DeleteNode;

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
        Client client = await _clientsReposiotory.GetByGuidAsync(command.ClientId);
        
        Domain.Nodes.Node node = await _nodesReposiotory.GetByNodeClientOwnerAsync(client);

        await _nodesReposiotory.RemoveAsync(node);
        
        client.DisableNode(); // this should be in domain event i think

        return new(node.Id);
    }
}