using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing;

public class UnitOfWork : IUnitOfWork
{
    readonly IDomainEventsDispatcher _domainEventsDispatcher;
    readonly ModuleContext _context;

    public UnitOfWork(ModuleContext context, IDomainEventsDispatcher domainEventsDispatcher)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
        _context = context;
    }

    public async Task<int> CommitAsync()
    {

        var entities = _context.ChangeTracker.Entries<Entity>().Where(e => e.Entity.DomainEvents.Any()).ToList();
        var domainEvents = entities.SelectMany(e => e.Entity.DomainEvents);
        
        foreach (var domainEvent in domainEvents)
            await _domainEventsDispatcher.DispatchAsync((dynamic)domainEvent); //dynamic ensure that IDomainEvent is correct type for service provider
        
        foreach(var entity in entities)
            entity.Entity.ClearDomainEvents();
        
        await _context.SaveChangesAsync();

        return 0;
    }
}