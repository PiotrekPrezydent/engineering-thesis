using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Clients.Events;

namespace Dara.Modules.AccessManagment.Application.Domain;

public class ClientUserRemovedEventHandler : IDomainEventHandler<ClientUserRemovedEvent>
{
    public async Task HandleAsync(ClientUserRemovedEvent domainEvent)
    {
        Console.WriteLine("client user removed");
    }
}