using Dara.BuildingBlocks.Domain.Models;
using Dara.BuildingBlocks.Domain.Models.Abstraction;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Connections.Events;

namespace Dara.Modules.Communication.Domain.Connections;

public class Connection : Entity, IAggregateRoot<ConnectionId>
{
    public ConnectionId Id { get; private set; }
    
    private ConnectionIp _connectionIp;
    
    public Client? ConnectionClient;
    
    private Connection(ConnectionId id, ConnectionIp connectionIp)
    {
        Id = id;
        
        _connectionIp = connectionIp;

        ConnectionClient = null;
        
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

    public Client StartClient(ClientName clientName, ClientAuthToken token)
    {
        if(ConnectionClient != null)
            throw new InvalidOperationException("Connection client is already started");
        
        Client client = Client.Create(new ClientId(Guid.NewGuid()), Id, clientName, token);
        ConnectionClient = client;
        
        return ConnectionClient;
    }

    public void StopClient()
    {
        if(ConnectionClient == null)
            throw new InvalidOperationException("Connection client is not started");
        
        ConnectionClient.Delete();
        ConnectionClient = null;
    }
}