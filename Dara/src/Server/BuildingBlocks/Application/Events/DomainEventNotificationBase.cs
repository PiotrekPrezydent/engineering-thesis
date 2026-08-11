using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.BuildingBlocks.Application.Events;

public record DomainEventNotificationBase<TEvent> : IDomainEventNotification<TEvent> where TEvent : IDomainEvent
{
    public Guid NotificationId { get; }
    public TEvent DomainEvent { get; }
    
    public DomainEventNotificationBase(Guid notificationId, TEvent domainEvent)
    {
        NotificationId = notificationId;
        DomainEvent = domainEvent;
    }
}