using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.AccessManagment.Domain.Clients.Events;

public class ClientCreatedEvent : BaseDomainEvent
{
    public Client Client { get; }
    public ClientCreatedEvent(Client client)
    {
        Client = client;
    }
}