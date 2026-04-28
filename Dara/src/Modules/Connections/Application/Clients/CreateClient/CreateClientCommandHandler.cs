using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Clients;
using Dara.Modules.Connections.Domain.Connections;

namespace Dara.Modules.Connections.Application.Clients.CreateClient
{
    public class CreateClientCommandHandler : IHandler<CreateClientCommand>
    {
        IConnectionRepository _connectionRepository;
        IClientRepository _clientRepository;
    
        public CreateClientCommandHandler(IConnectionRepository connectionRepository, IClientRepository clientRepository)
        {
            _connectionRepository = connectionRepository;
            _clientRepository = clientRepository;
        }
    
        public async Task HandleAsync(CreateClientCommand command)
        {
            ConnectionId id = new(command.ConnectionId);
        
            Connection connection = await _connectionRepository.GetByIdAsync(id);

            ClientName name = new ClientName(command.ClientName);
            ClientAuthToken token = new ClientAuthToken(command.ClientAuthToken);
        
            Client client = connection.CreateClient(name, token);
        
            await _clientRepository.AddAsync(client);
        }
    }
}