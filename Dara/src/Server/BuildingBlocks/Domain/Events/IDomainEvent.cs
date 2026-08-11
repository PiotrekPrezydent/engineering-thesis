namespace Dara.Server.BuildingBlocks.Domain.Events;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccuredOn { get; }
}
