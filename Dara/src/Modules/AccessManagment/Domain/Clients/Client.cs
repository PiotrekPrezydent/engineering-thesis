using Dara.BuildingBlocks.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Domain.Clients;

public class Client : Entity, IAggregateRoot
{
    public User? User { get; private set; }
    
    public ClientName Name { get; private set; }
    
    public ClientToken Token { get; private set; }
    
    public bool IsActive { get; private set; }

    public Client(ClientName name, ClientToken token)
    {
        Id = Guid.NewGuid();
        Name = name;
        Token = token;
        IsActive = true;
    }

    public void AssingUser(User user)
    {
        if (User is null)
        {
            User = user;
        }
        else
        {
            //throw exception
        }
    }

    public void RemoveUser(User user)
    {
        if (User is null)
        {
            //throw exception
        }
        else
        {
            User = null;
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
    public void ChangeIsActive(bool isActive)
    {
        IsActive = isActive;
    }
}