using System.Text.Json;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;

public class OutboxProcessor : IOutboxProcessor
{
    private readonly ILogger _logger;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IHandlersResolver _handlersResolver;
    private readonly IOutboxTypeMapper _outboxTypeMapper;

    public OutboxProcessor(ILoggerFactory logger, IHandlersResolver handlersResolver, IOutboxTypeMapper outboxTypeMapper, IOutboxRepository outboxRepository, DbContext context)
    {
        _handlersResolver = handlersResolver;
        _outboxTypeMapper = outboxTypeMapper;
        _outboxRepository = outboxRepository;
        _logger = logger.CreateLogger("OUTBOX PROCESSOR :::: " + context.GetType().Name);
    }

    public async Task ProcessOutboxAsync(CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var messages = await _outboxRepository.GetPendingMessagesAsync(cancellationToken);
        _logger.LogInformation("Outbox processor started  PENDING MESSAGES: " +  messages.Count + " ID " + id);
        foreach (var msg in messages)
        {
            var type = _outboxTypeMapper.GetType(msg.Type);
            var domainEvent = JsonSerializer.Deserialize(msg.Content, type) as IDomainEvent;
            
            await DispatchDomainEventNotificationAsync((dynamic)domainEvent!);
            
            await _outboxRepository.MarkAsCompletedAsync(msg.Id, cancellationToken);
        }
        _logger.LogInformation("Outbox processor ENDED " +  " ID " + id);
    }
    
    async Task DispatchDomainEventNotificationAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        var handlers = _handlersResolver.GetDomainEventNotificationHandlers<TDomainEvent>();
        foreach (var handler in handlers)
            await handler.HandleAsync(domainEvent);
    }
}