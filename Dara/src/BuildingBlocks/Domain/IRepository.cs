namespace Dara.BuildingBlocks.Domain;

public interface IRepository <TAggregateRoot> where TAggregateRoot : IAggregateRoot
{
    public Task<TAggregateRoot> GetByGuidAsync(Guid guid);
    
    public Task<IEnumerable<TAggregateRoot>> GetAllAsync();
    
    public Task AddAsync(TAggregateRoot aggregateRoot);
    
    public Task RemoveAsync(TAggregateRoot aggregateRoot);
    
}