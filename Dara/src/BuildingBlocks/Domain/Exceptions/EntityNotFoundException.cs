namespace Dara.BuildingBlocks.Domain.Exceptions;

public class EntityNotFoundInRepositoryException<TRepo,TEntity> : BaseDomainException where TEntity : Entity, IAggregateRoot where TRepo : IRepository<TEntity>
{
    private TRepo _repository;
    private object _searchCriteria;
    public EntityNotFoundInRepositoryException(TRepo repository, object searchCriteria, string message = nameof(EntityNotFoundInRepositoryException<TRepo,TEntity>)) : base(message)
    {
        _repository = repository;
        _searchCriteria = searchCriteria;
        // message += $"{typeof(TEntity).Name}, could not be found in {typeof(TRepo).Name} repository using {_searchCriteria} of type {_searchCriteria.GetType().Name}";
        // message += _searchCriteria + " of type " + _searchCriteria.GetType().Name;
        
    }

    protected override string DomainExcetpionState()
    {
        string ret = "";

        ret += $"REPO_TYPE:\t[ {_repository.GetType().Name }]\n";
        ret += $"SEARCH_TYPE:\t[ {_searchCriteria.GetType().Name} ]\n";
        ret += $"SAERCH_VAL:\t[ {_searchCriteria} ]\n";
        ret += $"REPO:\n{_repository.GetAllAsync().Result.Select(e => "\t" + e.ToString() + "\n")}]\n";
        
        return ret;
    }
}