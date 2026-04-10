using Dara.Core.Domain.Events;

namespace Dara.Modules.AccessManagment.Domain.Events;

public class UserDeviceRemovedEvent : BaseDomainEvent
{
    public Guid UserId { get; }
    public Guid DeviceId { get; }
    
    public UserDeviceRemovedEvent(Guid userId, Guid deviceId) : base()
    {
        UserId = userId;
        DeviceId = deviceId;
    }
}