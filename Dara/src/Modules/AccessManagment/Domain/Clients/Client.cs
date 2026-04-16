using Dara.BuildingBlocks.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Clients.Events;
using Dara.Modules.AccessManagment.Domain.Clients.Exceptions;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Domain.Clients;

public class Client : Entity, IAggregateRoot
{
    public User? User { get; private set; }
    
    public ClientName Name { get; private set; }
    
    public ClientToken Token { get; private set; }
    
   //public bool IsActive { get; private set; }

    public Client(ClientName name, ClientToken token)
    {
        Id = Guid.NewGuid();
        Name = name;
        Token = token;
        //IsActive = true;
        
        _events.Add(new ClientCreatedEvent(this));
    }

    public void AssingUser(User user)
    {
        if (User is null)
        {
            User = user;
            _events.Add(new ClientUserAddedEvent(this, user));
        }
        else
        {
            throw new ClientAlreadyHaveUserException();
        }
    }

    public void RemoveUser(User user)
    {
        if (User is null)
        {
            throw new ClientUserIsNullException();
        }
        else
        {
            User = null;
            _events.Add(new ClientUserRemovedEvent(this, user));
        }
    }
    
    public void ChangeName(ClientName newName)
    {
        Name = newName;
    }

    public void ChangeToken(ClientToken token)
    {
        Token = token;
    }

    //when disconnected we should change client is active, do not remove him from domain
    // public void ChangeIsActive(bool isActive)
    // {
    //     IsActive = isActive;
    // }
}