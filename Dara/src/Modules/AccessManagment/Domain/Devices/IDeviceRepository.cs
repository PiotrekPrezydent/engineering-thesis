using Dara.BuildingBlocks.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Clients;

namespace Dara.Modules.AccessManagment.Domain.Devices;

public interface IDeviceRepository : IRepository<Device>
{
    public Task<Device> FindByName(DeviceName name);
    
    public Task<Device> FindById(Guid id);
    
    public Task Add(Device device);
    
    public Task Save(Device device);
}