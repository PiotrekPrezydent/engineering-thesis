using LADR.SharedKernel.Integration.Models;

namespace LADR.SharedKernel.Infrastructure.Models;

public interface IIntegrationEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IntegrationEvent;
}