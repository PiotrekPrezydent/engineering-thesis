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
    
    private readonly ILogger _logger;


    public OutboxMessageProcessor(DbContext context, IHandlersResolver handlersResolver, IOutboxMessagesTypeMapper typeMapper,  ILoggerFactory loggerFactory)
    {
        _context = context;
        _handlersResolver = handlersResolver;
        _typeMapper = typeMapper;
        
        _logger = loggerFactory.CreateLogger("OUTBOX MESSAGE PROCESSOR :::: " + _context.GetModuleName());
    }

    public async Task ProcessSingleMessageAsync(Guid messageId, CancellationToken stoppingToken)
    {
        
        var message = await _context.Set<OutboxMessage>().FindAsync([messageId], stoppingToken);
        if (message == null || message.ProcessedDate != null)
            return;

        _logger.LogInformation($"STARTING PROCESSING OUTBOX MESSAGE {message.Type} **DATA** {message.Content}");
        
        var type = _typeMapper.GetTypeForMessageWithTypeName(message.Type);
        var notification = JsonSerializer.Deserialize(message.Content, type) as IDomainEventNotification;
        
        await DispatchDomainEventNotificationAsync((dynamic)notification!);
        message.ProcessedDate = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(stoppingToken);
        
        _logger.LogInformation($"PROCESSED OUTBOX MESSAGE {message.Type} IN {(message.ProcessedDate! - message.OccurredOn).Value.TotalSeconds}");
    }
    
    async Task DispatchDomainEventNotificationAsync<TDomainEventNotification>(TDomainEventNotification notification) where TDomainEventNotification : IDomainEventNotification
    {
        var handlers = _handlersResolver.GetDomainEventNotificationHandlers<TDomainEventNotification>();
        foreach (var handler in handlers)
            await handler.HandleAsync(notification);
    }
}