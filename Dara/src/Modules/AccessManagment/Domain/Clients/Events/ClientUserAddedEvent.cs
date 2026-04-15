using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Domain.Clients.Events;

public class ClientUserAddedEvent(Client Client, User User) : BaseDomainEvent
{
    
}