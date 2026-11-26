using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Rules;

// the server gives the device id when the device is created; an empty one is a bug of the caller
public class DeviceSessionStartRequiresDeviceIdRule : IBuisnessRule
{
    private readonly Guid _deviceId;

    public DeviceSessionStartRequiresDeviceIdRule(Guid deviceId)
    {
        _deviceId = deviceId;
    }

    public string Message => "Device id must not be empty";

    public bool IsBroken()
    {
        return _deviceId == Guid.Empty;
    }
}
