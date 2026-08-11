using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Identity.Integration;

public record NewClientCreatedIntegrationEvent : IntegrationEventBase
{
    public Guid CreatedClientId { get; }
    
    public NewClientCreatedIntegrationEvent(Guid id, DateTime occurredOn, Guid createdClientId) : base(id, occurredOn)
    {
        CreatedClientId = createdClientId;
    }
}