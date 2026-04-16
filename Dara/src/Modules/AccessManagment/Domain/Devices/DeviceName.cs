using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Devices;

public class DeviceName : ValueObject
{
    public string Name { get; }
    
    public DeviceName(string name)
    {
        Name = name;
    }
}