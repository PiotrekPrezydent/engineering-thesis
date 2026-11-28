namespace Dara.Server.BuildingBlocks.Domain.Exceptions;

// a repository found no entity with the id; the http host answers 404, a missing entity and one the caller may not
// see look the same
public class EntityNotFoundException : BaseDomainException
{
    public string Entity { get; }
    public object Id { get; }

    public EntityNotFoundException(string entity, object id) : base($"{entity} {id} was not found")
    {
        Entity = entity;
        Id = id;
    }
}
