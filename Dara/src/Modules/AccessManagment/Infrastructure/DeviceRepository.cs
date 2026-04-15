using Dara.Modules.AccessManagment.Domain.Devices;

namespace Dara.Modules.AccessManagment.Infrastructure;

public class DeviceRepository : IDeviceRepository
{
    public Task<Device> FindByName(DeviceName name)
    {
        throw new NotImplementedException();
    }

    public Task<Device> FindById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task Add(Device device)
    {
        throw new NotImplementedException();
    }

    public Task Save(Device device)
    {
        throw new NotImplementedException();
    }
}