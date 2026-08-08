namespace Dara.Server.BuildingBlocks.Integration;

public record IntegrationEventBase : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}