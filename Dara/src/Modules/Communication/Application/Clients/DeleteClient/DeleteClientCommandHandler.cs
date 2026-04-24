using Dara.BuildingBlocks.Application.Abstraction;
using Dara.BuildingBlocks.Application.Exceptions;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Application.Clients.DeleteClient;

public class DeleteClientCommandHandler : IApplicationCommandHandler<DeleteClientCommand, DeleteClientCommandResult>
{
    private readonly IClientsRepository _clientsRepository;
    private readonly INodesReposiotory _nodesReposiotory;
    
    public DeleteClientCommandHandler(IClientsRepository clientsRepository,  INodesReposiotory nodesReposiotory)
    {
        _clientsRepository = clientsRepository;
        _nodesReposiotory = nodesReposiotory;
    }

    public async Task<DeleteClientCommandResult> HandleAsync(DeleteClientCommand command)
    {
        Domain.Clients.Client client;
        
        if (command is DeleteClientByConnectionIdCommand idCommand)
        {
            ClientConnectionId id = new(idCommand.ConnectionId);
            client = await _clientsRepository.GetByClientConnectionIdAsync(id);
        }
        else if (command is DeleteClientByClientGuid guidCommand)
        {
            client = await _clientsRepository.GetByGuidAsync(guidCommand.ClientGuid);
        }
        else if(command is DeleteClientByNodeGuid nodeGuidCommand)
        {
            var node = await _nodesReposiotory.GetByGuidAsync(nodeGuidCommand.NodeGuid);
            
            client = await _clientsRepository.GetByClientNode(node);
        }else
        {
            throw new CommandTypeIsUnhandledException(this.GetType(),  command.GetType());
        }
        
        await _clientsRepository.RemoveAsync(client);
        
        return new(client.Id);
    }
}