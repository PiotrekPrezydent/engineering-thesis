using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Connections;
using Dara.Modules.Communication.Domain.Connections.Events;

namespace Dara.Modules.Communication.Application.Connections.DeleteConnection;

public class ConnectionDeletedEventHandler : IDomainEventHandler<ConnectionDeletedDomainEvent>
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
            await _clientRepository.SaveAsync(client);
        }
        
        await _connectionRepository.RemoveAsync(connection);
    }
}