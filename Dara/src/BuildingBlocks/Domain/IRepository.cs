namespace Dara.BuildingBlocks.Domain;

public interface IRepository <in TAggregateRoot> where TAggregateRoot : IAggregateRoot
{
}