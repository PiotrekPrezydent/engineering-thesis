using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Clients.Events;

namespace Dara.Modules.AccessManagment.Application.Domain;

public class ClientCreatedEventHandler : IDomainEventHandler<ClientCreatedEvent>
{
    public async Task HandleAsync(ClientCreatedEvent domainEvent)
    {
        //Console.WriteLine($"Client created ID: {domainEvent.Client.Id} NAME: {domainEvent.Client.Name.Name}");
    }
}