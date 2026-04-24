using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients.Events;

namespace Dara.Modules.Communication.Application.Clients.DeleteClient;

public class ClientDeletedEventHandler : IDomainEventHandler<ClientDeletedDomainEvent>
{
    public ClientDeletedEventHandler()
    {
    }
    
    public async Task HandleAsync(ClientDeletedDomainEvent domainEvent)
    {
        Console.WriteLine($"EVENT HANDLER WORKING FOR: {GetType().Name}");
    }
}