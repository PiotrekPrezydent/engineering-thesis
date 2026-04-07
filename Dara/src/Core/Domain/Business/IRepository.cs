namespace Dara.Core.Domain.Business;

public interface IRepository <in TAggregateRoot> where TAggregateRoot : IAggregateRoot
{
}