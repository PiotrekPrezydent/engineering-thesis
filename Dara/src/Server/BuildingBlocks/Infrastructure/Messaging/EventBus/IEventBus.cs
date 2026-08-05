using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;

public interface IEventBus
{
    public void Subscribe<TEvent>(IIntegrationEventHandler<TEvent> handler) where TEvent : IIntegrationEvent;
    
    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : IIntegrationEvent;
}