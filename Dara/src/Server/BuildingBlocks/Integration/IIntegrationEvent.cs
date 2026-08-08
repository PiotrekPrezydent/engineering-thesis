namespace Dara.Server.BuildingBlocks.Integration;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}