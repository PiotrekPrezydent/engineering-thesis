using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Domain.Clients.Events;

public class ClientUserAddedEvent : BaseDomainEvent
{
    public Client Client { get; }
    public User User { get; }
    
    public ClientUserAddedEvent(Client client, User user)
    {
        Client = client;
        User = user;
    }


    
}