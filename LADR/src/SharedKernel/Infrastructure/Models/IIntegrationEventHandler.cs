using LADR.SharedKernel.Integration.Models;

namespace LADR.SharedKernel.Infrastructure.Models;

public interface IIntegrationEventHandler <in TEvent> where TEvent : IntegrationEvent
{
    Task HandleAsync(TEvent domainEvent);
}