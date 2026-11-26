using System.Text.Json;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Integration;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;

public class InboxMessageProcessor : IInboxMessageProcessor
{
    private readonly DbContext _context;
    private readonly IHandlersResolver _handlersResolver;
    private readonly IInboxMessagesTypeMapper _typeMapper;
    private readonly MessageProcessingOptions _options;
    private readonly TimeProvider _timeProvider;
    
    private readonly ILogger<InboxMessageProcessor> _logger;

    public InboxMessageProcessor(DbContext context, IHandlersResolver handlersResolver, IInboxMessagesTypeMapper typeMapper, MessageProcessingOptions options, TimeProvider timeProvider, ILogger<InboxMessageProcessor> logger)
    {
        _context = context;
        _handlersResolver = handlersResolver;
        _typeMapper = typeMapper;
        _options = options;
        _timeProvider = timeProvider;

        _logger = logger;
    }

    public async Task ProcessSingleMessageAsync(Guid messageId, CancellationToken stoppingToken)
    {
        var message = await _context.Set<InboxMessage>().FindAsync([messageId], stoppingToken);
        if (message == null || message.ProcessedDate != null)
            return;
        
        _logger.LogDebug($"STARTING PROCESSING MESSAGE: {messageId} \n\tWITH TYPE: {message.Type} \n\tWITH CONTENT: {message.Content}");
        
        var type = _typeMapper.GetTypeForMessageWithTypeName(message.Type);
        var integrationEvent = JsonSerializer.Deserialize(message.Content, type) as IIntegrationEvent;
        
        await DispatchIntegrationEventAsync((dynamic)integrationEvent!);
        // one SaveChanges: ProcessedDate together with the internal commands the handlers queued in this scope
        message.MarkProcessed(_timeProvider.GetUtcNow().UtcDateTime);
        
        await _context.SaveChangesAsync(CancellationToken.None);
        
        _logger.LogDebug($"PROCESSED MESSAGE {messageId} IN {(message.ProcessedDate! - message.OccurredOn).Value.TotalSeconds}");
    }

    public async Task RecordFailureAsync(Guid messageId, Exception error, CancellationToken ct)
    {
        var message = await _context.Set<InboxMessage>().FindAsync([messageId], ct);
        if (message == null || message.ProcessedDate != null)
            return;

        message.RecordFailure(error, _timeProvider.GetUtcNow().UtcDateTime, _options.Retry);
        await _context.SaveChangesAsync(ct);

        _logger.LogWarning("MESSAGE {MessageId} {MessageType} FAILED {Attempts} TIME(S), NEXT ATTEMPT AT {NextAttemptAt:O}", messageId, message.Type, message.Attempts, message.NextAttemptAt);
    }

    async Task DispatchIntegrationEventAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent) where TIntegrationEvent : IIntegrationEvent
    {
        var handlers = _handlersResolver.GetIntegrationEventHandlers<TIntegrationEvent>();
        foreach (var handler in handlers)
            await handler.HandleAsync(integrationEvent);
    }
}