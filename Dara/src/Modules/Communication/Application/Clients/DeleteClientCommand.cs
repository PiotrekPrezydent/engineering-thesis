using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Nodes;

namespace Dara.Modules.Communication.Application.Clients;

public abstract record DeleteClientCommand() : IApplicationCommand;

public record DeleteClientByConnectionIdCommand(string ConnectionId) : DeleteClientCommand;

public record DeleteClientByClientGuid(Guid ClientGuid) : DeleteClientCommand;

public record DeleteClientByNodeGuid(Guid NodeGuid) : DeleteClientCommand;

public record DeleteClientCommandResult(Guid DeletedClientId) : IApplicationCommandResult;

public class DeleteClientCommandHandler : IApplicationCommandHandler<DeleteClientCommand, DeleteClientCommandResult>
{
    private readonly IClientsRepository _clientsRepository;
    //private readonly INodesReposiotory _nodesReposiotory;
    
    public DeleteClientCommandHandler(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<DeleteClientCommandResult> HandleAsync(DeleteClientCommand command)
    {
        if (command is DeleteClientByConnectionIdCommand)
        {
            var com = command as DeleteClientByConnectionIdCommand;
            ClientConnectionId id = new(com.ConnectionId);
            Client client = await _clientsRepository.GetByClientConnectionIdAsync(id);
        
            await _clientsRepository.RemoveAsync(client);
            return new(client.Id);
        }
        
        return null;
    }
}