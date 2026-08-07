using System.Text.Json;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;

public class OutboxProcessor : IOutboxProcessor
{
    private readonly ILogger _logger;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IHandlersResolver _handlersResolver;
    private readonly IOutboxTypeMapper _outboxTypeMapper;

    public OutboxProcessor(ILoggerFactory logger, IHandlersResolver handlersResolver, IOutboxTypeMapper outboxTypeMapper, IOutboxRepository outboxRepository)
    {
        _handlersResolver = handlersResolver;
        _outboxTypeMapper = outboxTypeMapper;
        _outboxRepository = outboxRepository;
        _logger = logger.CreateLogger("OUTBOX :::: " + _outboxRepository.GetType().FullName);
    }

    public async Task ProcessOutboxAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Outbox processor started");
        
        var messages = await _outboxRepository.GetPendingMessagesAsync(cancellationToken);
        _logger.LogInformation("STARTED MESSAGES::: " + messages.Count);
        foreach (var msg in messages)
        {
            var type = _outboxTypeMapper.GetType(msg.Type);
            var domainEvent = JsonSerializer.Deserialize(msg.Content, type) as IDomainEvent;
            
            await DispatchDomainEventNotificationAsync((dynamic)domainEvent!);
            
            await _outboxRepository.MarkAsCompletedAsync(msg.Id, cancellationToken);
        }
    }
    
    async Task DispatchDomainEventNotificationAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        var handlers = _handlersResolver.GetDomainEventNotificationHandlers<TDomainEvent>();
        foreach (var handler in handlers)
            await handler.HandleAsync(domainEvent);
    }
}