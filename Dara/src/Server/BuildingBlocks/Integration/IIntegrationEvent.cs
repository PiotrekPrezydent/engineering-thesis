namespace Dara.Server.BuildingBlocks.Integration;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}