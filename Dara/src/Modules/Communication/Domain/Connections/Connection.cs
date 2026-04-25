using Dara.BuildingBlocks.Domain.Models;
using Dara.BuildingBlocks.Domain.Models.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Connections.Events;

namespace Dara.Modules.Communication.Domain.Connections;

public class Connection : Entity, IAggregateRoot<ConnectionId>
{
    public ConnectionId Id { get; private set; }
    
    private ConnectionIp _connectionIp;
    
    private Client? _connectionClient;
    
    Connection(ConnectionId id, ConnectionIp connectionIp)
    {
        Id = id;
        
        _connectionIp = connectionIp;

        _connectionClient = null;
        
        AddDomainEvent(new ConnectionCreatedDomainEvent(Id));
    }

    public static Connection Create(ConnectionId connectionId, ConnectionIp connectionIp)
    {
        return new Connection(connectionId, connectionIp);
    }

    public void Delete()
    {
        AddDomainEvent(new ConnectionDeletedDomainEvent(Id));
    }

    public Client CreateClient(ClientName clientName, ClientAuthToken token)
    {
        if(_connectionClient != null)
            throw new InvalidOperationException("Connection have client already!");
        
        Client client = Client.Create(new ClientId(Guid.NewGuid()), Id, clientName, token);
        _connectionClient = client;
        
        return _connectionClient;
    }

    public void RemoveClient()
    {
        if(_connectionClient == null)
            throw new InvalidOperationException("Connection dont have a client");
        
        _connectionClient.Delete();
        
        _connectionClient = null;
    }

    public bool TryGetClient(out Client client)
    {
        client = _connectionClient;
        
        return client != null;
    }
}