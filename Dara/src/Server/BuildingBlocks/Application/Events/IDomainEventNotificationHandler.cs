namespace Dara.Server.BuildingBlocks.Application.Events;

public interface IDomainEventNotificationHandler<in TDomainNotification> where TDomainNotification : IDomainEventNotification 
{
    public Task HandleAsync(TDomainNotification notification);
}