using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Devices;

namespace Dara.Modules.AccessManagment.Infrastructure;

public class DeviceRepository : IDeviceRepository
{
    private readonly List<Device> _devices;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    public DeviceRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        Console.WriteLine("CRETED NEW DEVICE REPO!!!");
        _devices = new();
        _domainEventDispatcher = domainEventDispatcher;
    }
    
    public async Task<Device> FindByName(DeviceName name)
    {
        return _devices.First(e => e.Name == name);
    }

    public async Task<Device> FindById(Guid id)
    {
        return _devices.First(e => e.Id == id);
    }

    public async Task Add(Device device)
    {
        Console.WriteLine($"INFRA: add device: {device.Id} - {device.Name}");
        _devices.Add(device);
        foreach (var domainEvent in device.DomainEvents)
        {
            await _domainEventDispatcher.DispatchAsync(domainEvent);
        }
        device.ClearDomainEvents();
    }

    public async Task Save(Device device)
    {
        var deviceToRemove =  _devices.First(e => e.Id == device.Id);
        _devices.Remove(deviceToRemove);
        _devices.Add(device);
        foreach (var domainEvent in device.DomainEvents)
        {
            await _domainEventDispatcher.DispatchAsync(domainEvent);
        }
        device.ClearDomainEvents();
    }
}