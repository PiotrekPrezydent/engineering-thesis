using Dara.BuildingBlocks.Domain;
using Dara.Modules.RpcGateway.Domain.Clients.Events;

namespace Dara.Modules.RpcGateway.Domain.Clients;

//represent register client that is working on server
public class Client : Entity
{
    public ClientName Name { get; private set; }
    
    public ClientAuthToken AuthToken { get; private set; }
    

    public Client(ClientName name, ClientAuthToken authToken)
    {
        Id = Guid.NewGuid();
        
        Name = name;
        AuthToken = authToken;
        
        _events.Add(new ClientCreatedEvent(this));
    }

    public void ChangeClientAuthToken(ClientAuthToken clientAuthToken)
    {
        AuthToken = clientAuthToken;
        _events.Add(new ClientAuthTokenChangedEvent(this));
    }
    
    public void ChangeClientName(ClientName clientName)
    {
        Name = clientName;
        _events.Add(new ClientNameChangedEvent(this));
    }
}