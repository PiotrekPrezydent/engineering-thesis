using Dara.BuildingBlocks.Domain.Events;

namespace Dara.Modules.AccessManagment.Domain.Events;

public class UserNodeAddedEvent : BaseDomainEvent
{
    public Guid UserId { get; }
    public Guid DeviceId { get; }
    
    public UserNodeAddedEvent(Guid userId, Guid deviceId) : base()
    {
        UserId = userId;
        DeviceId = deviceId;
    }
}