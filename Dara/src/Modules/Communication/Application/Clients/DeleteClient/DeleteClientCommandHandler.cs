using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Connections;

namespace Dara.Modules.Communication.Application.Clients.DeleteClient;

public class DeleteClientCommandHandler : IApplicationCommandHandler<DeleteClientCommand, DeleteClientCommandResult>
{
    IClientRepository _clientRepository;
    IConnectionRepository _connectionRepository;
    
    public DeleteClientCommandHandler(IClientRepository clientRepository, IConnectionRepository connectionRepository)
    {
        _clientRepository = clientRepository;
        _connectionRepository = connectionRepository;
    }
    
    public async Task<DeleteClientCommandResult> HandleAsync(DeleteClientCommand command)
    {
        ConnectionId id = new(command.ConnectionId);
        Connection connection = await _connectionRepository.GetByIdAsync(id);

        connection.TryGetClient(out Client client);
        connection.RemoveClient();
        
        await _clientRepository.SaveAsync(client);
        
        return new();
    }
}