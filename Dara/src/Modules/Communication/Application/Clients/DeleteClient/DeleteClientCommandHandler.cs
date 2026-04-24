using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Connections;

namespace Dara.Modules.Communication.Application.Clients.DeleteClient;

public class DeleteClientCommandHandler : IApplicationCommandHandler<DeleteClientCommand, DeleteClientCommandResult>
{
    IConnectionRepository _connectionRepository;
    IClientRepository _clientRepository;
    
    public DeleteClientCommandHandler(IConnectionRepository connectionRepository, IClientRepository clientRepository)
    {
        _connectionRepository = connectionRepository;
        _clientRepository = clientRepository;
    }
    
    public async Task<DeleteClientCommandResult> HandleAsync(DeleteClientCommand command)
    {
        ConnectionId id = new ConnectionId(command.ConnectionId);
        
        Connection connection = await _connectionRepository.GetByIdAsync(id);
        Client client = connection.ConnectionClient;
        
        client.Delete();

        await _clientRepository.RemoveAsync(client);
        
        connection.StopClient(); // this provides event
        
        return new();
    }
}