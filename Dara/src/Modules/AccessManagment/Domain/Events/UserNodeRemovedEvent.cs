using Dara.Core.Domain.Events;

namespace Dara.Modules.AccessManagment.Domain.Events;

public class UserNodeRemovedEvent : BaseDomainEvent
{
    public Guid UserId { get; }
    public Guid DeviceId { get; }
    
    public UserNodeRemovedEvent(Guid userId, Guid deviceId) : base()
    {
        UserId = userId;
        DeviceId = deviceId;
    }
}