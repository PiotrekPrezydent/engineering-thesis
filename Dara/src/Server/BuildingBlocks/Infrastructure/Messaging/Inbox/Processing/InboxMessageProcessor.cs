using System.Text.Json;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;

public class InboxMessageProcessor : IInboxMessageProcessor
{
    private readonly DbContext _context;
    private readonly IHandlersResolver _handlersResolver;
    private readonly IInboxMessagesTypeMapper _typeMapper;
    
    private readonly ILogger<InboxMessageProcessor> _logger;

    public InboxMessageProcessor(DbContext context, IHandlersResolver handlersResolver, IInboxMessagesTypeMapper typeMapper, ILogger<InboxMessageProcessor> logger)
    {
        _context = context;
        _handlersResolver = handlersResolver;
        _typeMapper = typeMapper;

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
        message.ProcessedDate = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(stoppingToken);
        
        _logger.LogDebug($"PROCESSED MESSAGE {messageId} IN {(message.ProcessedDate! - message.OccurredOn).Value.TotalSeconds}");
    }

    async Task DispatchIntegrationEventAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent) where TIntegrationEvent : IIntegrationEvent
    {
        var handlers = _handlersResolver.GetIntegrationEventHandlers<TIntegrationEvent>();
        foreach (var handler in handlers)
            await handler.HandleAsync(integrationEvent);
    }
}