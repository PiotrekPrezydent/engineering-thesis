using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Clients;
using Dara.Modules.Connections.Domain.Connections;

namespace Dara.Modules.Connections.Application.Clients.DeleteClient
{
    public class DeleteClientCommandHandler : IHandler<DeleteClientCommand>
    {
        IClientRepository _clientRepository;
        IConnectionRepository _connectionRepository;
    
        public DeleteClientCommandHandler(IClientRepository clientRepository, IConnectionRepository connectionRepository)
        {
            _clientRepository = clientRepository;
            _connectionRepository = connectionRepository;
        }
    
        public async Task HandleAsync(DeleteClientCommand command)
        {
            ConnectionId id = new(command.ConnectionId);
            Connection connection = await _connectionRepository.GetByIdAsync(id);

            connection.TryGetClient(out Client client);
            connection.RemoveClient();
        
            await _clientRepository.SaveAsync(client);
        }
    }
}