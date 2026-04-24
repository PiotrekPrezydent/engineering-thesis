using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Connections.Events;

namespace Dara.Modules.Communication.Application.Connections.DeleteConnection;

public class ConnectionDeletedEventHandler : IDomainEventHandler<ConnectionDeletedDomainEvent>
{
    public ConnectionDeletedEventHandler()
    {
    }
    
    public async Task HandleAsync(ConnectionDeletedDomainEvent domainEvent)
    {
        Console.WriteLine($"EVENT HANDLER WORKING FOR: {GetType().Name}");
    }
}