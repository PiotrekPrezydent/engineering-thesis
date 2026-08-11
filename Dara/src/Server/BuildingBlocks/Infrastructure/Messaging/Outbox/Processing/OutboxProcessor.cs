using System.Text.Json;
using Dara.Server.BuildingBlocks.Application.Events;
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
    private readonly IOutboxMessagesTypeMapper _outboxMessagesTypeMapper;

    public OutboxProcessor(ILoggerFactory logger, IHandlersResolver handlersResolver, IOutboxMessagesTypeMapper outboxMessagesTypeMapper, IOutboxRepository outboxRepository, DbContext context)
    {
        _handlersResolver = handlersResolver;
        _outboxMessagesTypeMapper = outboxMessagesTypeMapper;
        _outboxRepository = outboxRepository;
        _logger = logger.CreateLogger("OUTBOX PROCESSOR :::: " + context.GetType().Name);
    }

    public async Task ProcessOutboxAsync(CancellationToken cancellationToken)
    {
        var messages = await _outboxRepository.GetPendingMessagesAsync(cancellationToken);
        
        foreach (var message in messages)
        {
            _logger.LogInformation($"STARTING PROCESSING OUTBOX MESSAGE {message.Type} **DATA** {message.Content}");
            var type = _outboxMessagesTypeMapper.GetTypeForMessageWithTypeName(message.Type);
            var notification = JsonSerializer.Deserialize(message.Content, type) as IDomainEventNotification;
            
            await DispatchDomainEventNotificationAsync((dynamic)notification!);
            
            await _outboxRepository.MarkAsCompletedAsync(message.Id, cancellationToken);
            _logger.LogInformation($"PROCESSED OUTBOX MESSAGE {message.Type} IN {(message.ProcessedDate! - message.OccurredOn).Value.TotalSeconds}");
        }
    }
    
    async Task DispatchDomainEventNotificationAsync<TDomainEventNotification>(TDomainEventNotification notification) where TDomainEventNotification : IDomainEventNotification
    {
        var handlers = _handlersResolver.GetDomainEventNotificationHandlers<TDomainEventNotification>();
        foreach (var handler in handlers)
            await handler.HandleAsync(notification);
    }
}