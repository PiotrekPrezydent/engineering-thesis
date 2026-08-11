using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Identity.Integration;

public record NewUserCreatedIntegrationEvent : IntegrationEventBase
{
    public Guid CreatedUserId { get; }
    
    public NewUserCreatedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid createdUserId) : base(eventId, occurredOn)
    {
        CreatedUserId = createdUserId;
    }
}