using Dara.Core.Domain.Events;

namespace Dara.Modules.AccessManagment.Domain.Events;

public class UserCreatedEvent : BaseDomainEvent
{
    public Guid UserId { get; }

    public UserCreatedEvent(Guid userId) : base()
    {
        UserId = userId;
    }
}