using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Clients.Events;

namespace Dara.Modules.AccessManagment.Application.Domain;

public class ClientUserAddedEventHandler : IDomainEventHandler<ClientUserAddedEvent>
{
    public async Task HandleAsync(ClientUserAddedEvent domainEvent)
    {
        Console.WriteLine("client user added");
    }
}