using System.Text.Json;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;

public class OutboxMessageProcessor : IOutboxMessageProcessor
{
    private readonly DbContext _context;
    private readonly IHandlersResolver _handlersResolver;
    private readonly IOutboxMessagesTypeMapper _typeMapper;
    
    private readonly ILogger<OutboxMessageProcessor> _logger;


    public OutboxMessageProcessor(DbContext context, IHandlersResolver handlersResolver, IOutboxMessagesTypeMapper typeMapper,  ILogger<OutboxMessageProcessor> logger)
    {
        _context = context;
        _handlersResolver = handlersResolver;
        _typeMapper = typeMapper;
        
        _logger = logger;
    }

    public async Task ProcessSingleMessageAsync(Guid messageId, CancellationToken stoppingToken)
    {
        var message = await _context.Set<OutboxMessage>().FindAsync([messageId], stoppingToken);
        if (message == null || message.ProcessedDate != null)
            return;

        _logger.LogDebug($"STARTING PROCESSING MESSAGE: {messageId} \n\tWITH TYPE: {message.Type} \n\tWITH CONTENT: {message.Content}");
        
        var type = _typeMapper.GetTypeForMessageWithTypeName(message.Type);
        var notification = JsonSerializer.Deserialize(message.Content, type) as IDomainEventNotification;
        
        await DispatchDomainEventNotificationAsync((dynamic)notification!);
        message.ProcessedDate = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(stoppingToken);
        
        _logger.LogDebug($"PROCESSED MESSAGE {messageId} IN {(message.ProcessedDate! - message.OccurredOn).Value.TotalSeconds}");
    }
    
    async Task DispatchDomainEventNotificationAsync<TDomainEventNotification>(TDomainEventNotification notification) where TDomainEventNotification : IDomainEventNotification
    {
        var handlers = _handlersResolver.GetDomainEventNotificationHandlers<TDomainEventNotification>();
        foreach (var handler in handlers)
            await handler.HandleAsync(notification);
    }
}