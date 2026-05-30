using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Clients;
using Dara.Modules.Connections.Domain.Connections;
using Dara.Modules.Connections.Domain.Connections.Events;

namespace Dara.Modules.Connections.Application.Domain
{
    public class ConnectionDeletedEventHandler : IHandler<ConnectionDeletedDomainEvent>
    {
        private IConnectionRepository _connectionRepository;
        private IClientRepository _clientRepository;
    
        public ConnectionDeletedEventHandler(IConnectionRepository connectionRepository, IClientRepository clientRepository)
        {
            _connectionRepository = connectionRepository;
            _clientRepository = clientRepository;
        }
    
        public async Task HandleAsync(ConnectionDeletedDomainEvent domainEvent)
        {
            Connection connection = await _connectionRepository.GetByIdAsync(domainEvent.ConnectionId);

            if (connection.TryGetClient(out Client client))
            {
                connection.RemoveClient();
                await _clientRepository.RemoveAsync(client);
            }
        
            await _connectionRepository.RemoveAsync(connection);
        }
    }
}