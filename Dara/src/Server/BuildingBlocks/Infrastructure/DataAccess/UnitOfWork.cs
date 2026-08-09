using System.Text.Json;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;
    private readonly IOutboxTypeMapper _outboxTypeMapper;
    private readonly IOutboxRepository _outboxRepository;
    private readonly OutboxQueueSignal _outboxQueueSignal;
    
    public UnitOfWork(DbContext context, IDomainEventsDispatcher domainEventsDispatcher, IOutboxTypeMapper outboxTypeMapper, IOutboxRepository outboxRepository, OutboxQueueSignal outboxQueueSignal)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
        _outboxTypeMapper = outboxTypeMapper;
        _outboxRepository = outboxRepository;
        _outboxQueueSignal = outboxQueueSignal;
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
            var message = new OutboxMessage(domainEvent.Id,domainEvent.OccuredOn,type,data);
            
            await _outboxRepository.AddAsync(message, CancellationToken.None);
        }
        
        await _context.SaveChangesAsync();
        _outboxQueueSignal.NotifyNewMessage();
        
        return 0;
    }
}