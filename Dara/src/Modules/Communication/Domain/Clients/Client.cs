using Dara.BuildingBlocks.Domain.Models;
using Dara.BuildingBlocks.Domain.Models.Abstraction;
using Dara.Modules.Communication.Domain.Clients.Events;
using Dara.Modules.Communication.Domain.Connections;

namespace Dara.Modules.Communication.Domain.Clients;

public class Client : Entity, IAggregateRoot<ClientId>
{
    public ClientId Id { get; private set; }
    
    private ConnectionId _connectionId;
    private ClientName _name;
    private ClientAuthToken _authToken;
    
    Client(ClientId id, ConnectionId connectionId, ClientName name, ClientAuthToken authToken)
    {
        Id = id;
        
        _connectionId = connectionId;
        _name = name;
        _authToken = authToken;
        
        AddDomainEvent(new ClientCreatedDomainEvent(Id));
    }

    internal static Client Create(ClientId clientId, ConnectionId connectionId, ClientName name, ClientAuthToken authToken)
    {
        return new Client(clientId, connectionId, name, authToken);
    }

    public void Delete()
    {
        AddDomainEvent(new ClientDeletedDomainEvent(Id));
    }

    public void ChangeName(ClientName newName)
    {
        _name = newName;
        
        AddDomainEvent(new ClientNameChangedDomainEvent(Id, newName));
    }

    public void ChangeAuthToken(ClientAuthToken newAuthToken)
    {
        _authToken = newAuthToken;
        
        AddDomainEvent(new ClientAuthTokenChangedDomainEvent(Id, newAuthToken));
    }
}