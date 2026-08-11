namespace Dara.Server.BuildingBlocks.Infrastructure.DataAccess.DomainEventsMapping;

public class DomainEventNotificationMapper : IDomainEventNotificationMapper
{
    private readonly IDictionary<Type, Type> _map;

    public DomainEventNotificationMapper(IDictionary<Type, Type> map)
    {
        _map = map;
    }

    public Type? GetNotificationTypeForDomainEvent(Type domainEventType)
    {
        return _map.TryGetValue(domainEventType, out var notificationType) 
            ? notificationType 
            : null;
    }

    public bool HasNotificationForDomainEvent(Type domainEventType)
    {
        return _map.ContainsKey(domainEventType);
    }
}
