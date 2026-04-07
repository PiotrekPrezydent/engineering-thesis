using Dara.Core.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Device;

public class DeviceIdentity : Entity
{
    internal Guid UserId { get; set; }
    
    internal string DeviceName  { get; private set; }
    
    internal DeviceAuthToken AuthToken { get; private set; }

    internal DeviceIdentity(Guid userId, string deviceName, DeviceAuthToken authToken)
    {
        DeviceName = deviceName;
        AuthToken = authToken;
    }
}