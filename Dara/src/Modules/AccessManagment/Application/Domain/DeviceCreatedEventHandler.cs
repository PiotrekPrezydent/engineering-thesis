using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Devices.Events;

namespace Dara.Modules.AccessManagment.Application.Domain;

public class DeviceCreatedEventHandler : IDomainEventHandler<DeviceCreatedEvent>
{
    public async Task HandleAsync(DeviceCreatedEvent domainEvent)
    {
        Console.WriteLine("device created");
    }
}