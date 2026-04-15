using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.AccessManagment.Domain.Clients.Events;

public class ClientCreatedEvent(Client Client) : BaseDomainEvent
{
}