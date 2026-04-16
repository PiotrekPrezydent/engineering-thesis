using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Domain.Clients.Events;

public class ClientUserRemovedEvent : BaseDomainEvent
{
    public Client Client { get; }
    public User User { get; }
    
    public ClientUserRemovedEvent(Client client, User user)
    {
        Client = client;
        User = user;
    }
}