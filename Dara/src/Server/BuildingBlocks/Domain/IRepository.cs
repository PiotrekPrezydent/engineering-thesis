using Dara.BuildingBlocks.Domain.Models;

namespace Dara.BuildingBlocks.Domain;

public interface IRepository<TAggregateRoot> where TAggregateRoot : Entity, IAggregateRoot;