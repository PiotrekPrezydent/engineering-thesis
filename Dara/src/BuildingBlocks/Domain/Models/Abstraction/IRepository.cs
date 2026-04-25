namespace Dara.BuildingBlocks.Domain.Models.Abstraction;

public interface IRepository <TAggregateRoot,TId> where TAggregateRoot : IAggregateRoot<TId> where TId : IEntityId
{
    public Task<TAggregateRoot> GetByIdAsync(TId id);
    
    public Task<IEnumerable<TAggregateRoot>> GetAllAsync();
    
    public Task AddAsync(TAggregateRoot aggregateRoot);
    
    public Task RemoveAsync(TAggregateRoot aggregateRoot);
    
    public Task SaveAsync(TAggregateRoot aggregateRoot);
}