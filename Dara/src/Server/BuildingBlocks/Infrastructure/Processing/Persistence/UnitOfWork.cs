using System.Text.Json;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.Persistence;

public class UnitOfWork : IUnitOfWork
{
    readonly IOutboxTypeMapper _outboxTypeMapper;
    readonly IDomainEventsDispatcher _domainEventsDispatcher;
    readonly ModuleContext _context;

    public UnitOfWork(ModuleContext context, IDomainEventsDispatcher domainEventsDispatcher, IOutboxTypeMapper outboxTypeMapper)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
        _outboxTypeMapper = outboxTypeMapper;
        _context = context;
    }

    public async Task<int> CommitAsync()
    {
        var entities = _context.ChangeTracker.Entries<Entity>().Where(e => e.Entity.DomainEvents.Any()).ToList();
        var domainEvents = entities.SelectMany(e => e.Entity.DomainEvents).ToList();
        foreach(var entity in entities)
            entity.Entity.ClearDomainEvents();
        
        foreach (var domainEvent in domainEvents)
            await _domainEventsDispatcher.DispatchAsync((dynamic)domainEvent); //dynamic ensure that IDomainEvent is correct type for service provider

        foreach (var domainEvent in domainEvents)
        {
            if(!_outboxTypeMapper.CanHandleType(domainEvent.GetType()))
                continue;
            var type = _outboxTypeMapper.GetName(domainEvent.GetType());
            var data = JsonSerializer.Serialize(domainEvent,domainEvent.GetType());
            var message = new OutboxMessage(Guid.NewGuid(),DateTime.UtcNow,type,data); 
            
            _context.OutboxMessages.Add(message);
        }
        
        await _context.SaveChangesAsync();

        return 0;
    }

    void Test<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        Console.WriteLine("TEST"+domainEvent.GetType());
    }
}