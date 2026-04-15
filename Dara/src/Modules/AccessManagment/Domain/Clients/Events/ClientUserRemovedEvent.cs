using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Domain.Clients.Events;

public class ClientUserRemovedEvent(Client Client, User User) : BaseDomainEvent
{
    
}