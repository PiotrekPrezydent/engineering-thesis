namespace Dara.BuildingBlocks.Domain.Models.Abstraction;

public interface IAggregateRoot<TId> where TId : IEntityId 
{
    TId Id { get; }
}