using Dara.BuildingBlocks.Domain.Commands;
using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.RpcGateway.Domain;
using Dara.Modules.RpcGateway.Domain.Events;
using Dara.Shared.Common.Logging;

namespace Dara.Modules.RpcGateway.Infrastructure;

public class ClientConnectionRepository : IClientConnectionRepository
{
    private readonly List<ClientConnection> _clientConnections;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly ConsoleLogger _consoleLogger;

    public ClientConnectionRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _consoleLogger = new ConsoleLogger(this);
        _consoleLogger.Start("CREATE");
        
        _clientConnections = new();
        _domainEventDispatcher = domainEventDispatcher;
        
        _consoleLogger.End();
    }
    
    public async Task<ClientConnection> FindByIdAsync(Guid clientId)
    {
        _consoleLogger.Start("FindById");
        _consoleLogger.Element("ID", clientId);
        
        var client = _clientConnections.Find(x => x.Id == clientId);
        
        _consoleLogger.Element("FINDED", client);
        _consoleLogger.End();
        
        return client;
    }

    public async Task<ClientConnection> FindByConnectionId(ConnectionId connectionId)
    {
        _consoleLogger.Start("FindByConnectionId");
        _consoleLogger.Element("ID", connectionId);
        
        var client = _clientConnections.Find(x => x.ConnectionId == connectionId);
        
        _consoleLogger.Element("FINDED", client);
        _consoleLogger.End();
        
        return client;
    }

    public async Task AddAsync(ClientConnection clientConnection)
    {
        _consoleLogger.Start("Add");
        _consoleLogger.Element("client", clientConnection);
        
        foreach(var domainEvent in clientConnection.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
        clientConnection.ClearDomainEvents();
        
        _clientConnections.Add(clientConnection);
        
        _consoleLogger.Element("LIST:", _clientConnections);
        _consoleLogger.End();
    }

    public async Task RemoveAsync(ClientConnection clientConnection)
    {
        _consoleLogger.Start("remove");
        _consoleLogger.Element("client", clientConnection);
        
        foreach(var domainEvent in clientConnection.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
        clientConnection.ClearDomainEvents();
        
        _clientConnections.Remove(clientConnection);
        
        await _domainEventDispatcher.DispatchAsync(new ClientConnectionRemovedEvent(clientConnection));
        
        _consoleLogger.Element("LIST:", _clientConnections);
        _consoleLogger.End();
    }

    public async Task SaveAsync(ClientConnection clientConnection)
    {
        _consoleLogger.Start("save");
        _consoleLogger.Element("client", clientConnection);
        
        foreach(var domainEvent in clientConnection.DomainEvents)
            await _domainEventDispatcher.DispatchAsync((dynamic)domainEvent);
        
        clientConnection.ClearDomainEvents();
        
        var removed = await FindByIdAsync(clientConnection.Id);
        
        _clientConnections.Remove(removed);
        _clientConnections.Add(clientConnection);
        
        _consoleLogger.Element("LIST:", _clientConnections);
        _consoleLogger.End();
    }
}