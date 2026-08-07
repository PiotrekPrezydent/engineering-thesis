using System.Text.Json;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Scopes;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public class OutboxProcessor : IOutboxProcessor
{
    private readonly ModuleContext _context;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly IHandlersResolver _handlersResolver;
    private readonly IOutboxTypeMapper _outboxTypeMapper;

    public OutboxProcessor(ModuleContext context, ILogger<OutboxProcessor> logger, IHandlersResolver handlersResolver, IOutboxTypeMapper outboxTypeMapper)
    {
        _context = context;
        _logger = logger;
        _handlersResolver = handlersResolver;
        _outboxTypeMapper = outboxTypeMapper;
    }

    public async Task DispatchAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Outbox processor started");
        var messages = _context.OutboxMessages.Where(e => e.ProcessedDate == null);
        foreach (var msg in messages.ToList())
        {
            var type = _outboxTypeMapper.GetType(msg.Type);
            var domainEvent = JsonSerializer.Deserialize(msg.Content, type) as IDomainEvent;
            
            await HandleDomainNotification((dynamic)domainEvent!);
            
            msg.ProcessedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }

    }


    async Task HandleDomainNotification<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        var handlers = _handlersResolver.GetDomainEventNotificationHandlers<TDomainEvent>();
        foreach (var handler in handlers)
            await handler.HandleAsync(domainEvent);
    }
}