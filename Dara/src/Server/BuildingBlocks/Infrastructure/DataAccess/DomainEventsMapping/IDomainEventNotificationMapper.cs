namespace Dara.Server.BuildingBlocks.Infrastructure.DataAccess.DomainEventsMapping;

public interface IDomainEventNotificationMapper
{
    public Type? GetNotificationTypeForDomainEvent(Type domainEventType);

    public bool HasNotificationForDomainEvent(Type domainEventType);
}