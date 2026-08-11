using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.BuildingBlocks.Application.Events;

public interface IDomainEventNotification
{
    Guid NotificationId { get; }
}

public interface IDomainEventNotification<out TEvent> : IDomainEventNotification
{
    TEvent DomainEvent { get; }

    public virtual static string Test(IDomainEvent e)
    {
        return e.GetType().Name;
    }
}