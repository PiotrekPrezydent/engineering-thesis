namespace Dara.BuildingBlocks.Domain.Business;

public interface IRepository <in TAggregateRoot> where TAggregateRoot : IAggregateRoot
{
}