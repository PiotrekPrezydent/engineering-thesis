namespace Dara.Server.BuildingBlocks.Domain.Events;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccuredOn { get; }
}
