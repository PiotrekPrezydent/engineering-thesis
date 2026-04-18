using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.Communication.Domain.Clients.Events;
using Dara.Modules.Communication.Domain.Groups;

namespace Dara.Modules.Communication.Domain.Clients;

public class Client : Entity, IAggregateRoot
{
    public ClientName Name { get; private set; }
    
    public ClientAuthToken AuthToken { get; private set; }
    
    public ClientConnection Connection { get; }

    public Group CurrentGroup { get; private set; }

    public Client(ClientName name, ClientAuthToken authToken, ClientConnection connection)
    {
        Id = Guid.NewGuid();
        
        Name = name;
        AuthToken = authToken;
        Connection = connection;

        CurrentGroup = null;
    }

    public void ChangeName(ClientName newName)
    {
        Name = newName;
        
        _events.Add(new EntityValueObjectChangedEvent<Client,ClientName>(this, Name));
    }

    public void ChangeAuthToken(ClientAuthToken newAuthToken)
    {
        AuthToken = newAuthToken;
        
        _events.Add(new EntityValueObjectChangedEvent<Client,ClientAuthToken>(this, AuthToken));
    }
    
    public void JoinGroup(Group group)
    {
        if(CurrentGroup != null)
            throw new InvalidOperationException("Cannot join a group more than once");
        
        CurrentGroup = group;
        
        _events.Add(new ClientJoinedGroupEvent(this, CurrentGroup));
    }

    public void LeaveGroup(Group group)
    {
        if(CurrentGroup == null)
            throw new InvalidOperationException("Client dont have a group");
        
        CurrentGroup = null;
        
        _events.Add(new ClientLeftGroupEvent(this, group));
    }
    
    
}