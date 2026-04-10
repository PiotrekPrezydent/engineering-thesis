using Dara.Core.Domain.Events;

namespace Dara.Modules.AccessManagment.Domain.Events;

public class UserDeviceAddedEvent : BaseDomainEvent
{
    public Guid UserId { get; }
    public Guid DeviceId { get; }
    
    public UserDeviceAddedEvent(Guid userId, Guid deviceId) : base()
    {
        UserId = userId;
        DeviceId = deviceId;
    }
}