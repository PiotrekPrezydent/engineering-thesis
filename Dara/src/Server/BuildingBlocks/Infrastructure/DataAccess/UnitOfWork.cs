using System.Text.Json;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess.DomainEventsMapping;
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
    private readonly IDomainEventNotificationMapper _domainEventNotificationMapper;
    private readonly IOutboxMessagesTypeMapper _outboxMessagesTypeMapper;
    private readonly IOutboxRepository _outboxRepository;
    private readonly OutboxQueueSignal _outboxQueueSignal;
    
    public UnitOfWork(DbContext context, IDomainEventsDispatcher domainEventsDispatcher, IOutboxMessagesTypeMapper outboxMessagesTypeMapper, IOutboxRepository outboxRepository, OutboxQueueSignal outboxQueueSignal, IDomainEventNotificationMapper domainEventNotificationMapper)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
        _outboxMessagesTypeMapper = outboxMessagesTypeMapper;
        _outboxRepository = outboxRepository;
        _outboxQueueSignal = outboxQueueSignal;
        _domainEventNotificationMapper = domainEventNotificationMapper;
        _context = context;
    }

    public async Task<int> CommitAsync()
    {
        var entities = _context.ChangeTracker.Entries<Entity>().Where(e => e.Entity.DomainEvents.Any()).ToList();
        var domainEvents = entities.SelectMany(e => e.Entity.DomainEvents).ToList();
        foreach(var entity in entities)
            entity.Entity.ClearDomainEvents();

        var notifications = new List<IDomainEventNotification<IDomainEvent>>();
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = _domainEventNotificationMapper.GetNotificationTypeForDomainEvent(domainEvent.GetType());
            if(notificationType == null)
                continue;
            
            var notification = Activator.CreateInstance(notificationType, domainEvent.EventId, domainEvent) as IDomainEventNotification<IDomainEvent>;
            if(notification == null)
                continue;
            
            notifications.Add(notification);
        }
        
        foreach (var domainEvent in domainEvents)
            await _domainEventsDispatcher.DispatchAsync((dynamic)domainEvent); //dynamic ensure that IDomainEvent is correct type for service provider
        
        foreach (var notification in notifications)
        {
            var type = _outboxMessagesTypeMapper.GetTypeNameForMessageWithType(notification.GetType());
            var data = JsonSerializer.Serialize(notification,notification.GetType());
            
            var message = new OutboxMessage(notification.NotificationId, notification.DomainEvent.OccuredOn,type,data);
            
            await _outboxRepository.AddAsync(message, CancellationToken.None);
        }
        
        await _context.SaveChangesAsync();
        //fix for too many outbox calls
        if(notifications.Any())
            _outboxQueueSignal.NotifyNewMessage();
        
        return 0;
    }
}


