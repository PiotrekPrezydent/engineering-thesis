using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Infrastructure;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Communication.Application.Domain;

class ClientCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<Client>>
{
    public ClientCreatedEventHandler()
    {
    }

    public async Task HandleAsync(EntityCreatedEvent<Client> domainEvent)
    {
        Console.WriteLine($"{nameof(ClientCreatedEventHandler)}.{nameof(HandleAsync)}");
    }
}