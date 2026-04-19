using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.BuildingBlocks.Domain.Exceptions;
using Dara.Modules.RpcGateway.Domain;
using Dara.Shared.Common.Logging;

namespace Dara.Modules.RpcGateway.Infrastructure;

public class ConnectionRepository : IConnectionRepository
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly List<Connection> _connections;
    private readonly ConsoleLogger _logger;

    public ConnectionRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _logger = new ConsoleLogger(this);

        _connections = new();
        _domainEventDispatcher = domainEventDispatcher;
    }
    
    public async Task<Connection> GetByGuidAsync(Guid guid)
    {
        var connection = _connections.First(x => x.Id == guid);
        
        if(connection == null)
            throw new EntityNotFoundInRepositoryException<ConnectionRepository,Connection>(this, guid);
        
        return connection;
    }

    public async Task<Connection> GetByConnectionIdAsync(ConnectionId connectionId)
    {
        var connection = _connections.First(x => x.ConnectionId == connectionId);
        
        if(connection == null)
            throw new EntityNotFoundInRepositoryException<ConnectionRepository,Connection>(this, connectionId);
        
        return connection;
    }

    public async Task<IEnumerable<Connection>> GetAllAsync()
    {
        return _connections.AsEnumerable();
    }

    public async Task<IEnumerable<Connection>> GetAllWithIpAsync(ConnectionIp connectionIp)
    {
        return _connections.Where(e => e.ConnectionIp == connectionIp);
    }

    public async Task AddAsync(Connection connection)
    {
        foreach (var domainEvent in connection.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic) domainEvent);
        
        _connections.Add(connection);
    }

    public async Task RemoveAsync(Connection connection)
    {
        foreach (var domainEvent in connection.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic) domainEvent);
        
        _connections.Remove(connection);
    }
}