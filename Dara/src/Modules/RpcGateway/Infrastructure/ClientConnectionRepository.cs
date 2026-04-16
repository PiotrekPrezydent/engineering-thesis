using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.RpcGateway.Domain;
using Dara.Modules.RpcGateway.Domain.Events;

namespace Dara.Modules.RpcGateway.Infrastructure;

public class ClientConnectionRepository : IClientConnectionRepository
{
    private readonly List<ClientConnection> _clientConnections;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public ClientConnectionRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _clientConnections = new();
        _domainEventDispatcher = domainEventDispatcher;
    }
    
    public async Task<ClientConnection> FindByIdAsync(Guid clientId)
    {
        return _clientConnections.Find(x => x.Id == clientId);
    }

    public async Task<ClientConnection> FindByConnectionId(ConnectionId connectionId)
    {
        return _clientConnections.Find(x => x.ConnectionId == connectionId);
    }

    public async Task AddAsync(ClientConnection clientConnection)
    {
        _clientConnections.Add(clientConnection);
        //this should be in domain i think
        await _domainEventDispatcher.DispatchAsync(new ClientConnectionCreatedEvent(clientConnection));
    }

    public async Task RemoveAsync(ClientConnection clientConnection)
    {
        _clientConnections.Remove(clientConnection);
        //this should be in domain i think
        await _domainEventDispatcher.DispatchAsync(new ClientConnectionRemovedEvent(clientConnection));
    }

    public async Task SaveAsync(ClientConnection clientConnection)
    {
        var removed = await FindByIdAsync(clientConnection.Id);
        _clientConnections.Remove(removed);
        _clientConnections.Add(clientConnection);
        //this should be in domain i think
        await _domainEventDispatcher.DispatchAsync(new ClientConnectionCreatedEvent(clientConnection));
    }
}