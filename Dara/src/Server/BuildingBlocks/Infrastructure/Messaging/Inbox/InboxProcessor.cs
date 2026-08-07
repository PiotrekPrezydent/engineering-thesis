using System.Text.Json;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Scopes;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;

public class InboxProcessor : IInboxProcessor
{
    private readonly ModuleContext _context;
    private readonly IHandlersResolver _handlersResolver;
    private readonly IInboxTypeMapper _inboxTypeMapper;
    private readonly ILogger _logger;

    public InboxProcessor(ModuleContext context, IHandlersResolver handlersResolver, IInboxTypeMapper inboxTypeMapper, ILoggerFactory logger)
    {
        _context = context;
        _handlersResolver = handlersResolver;
        _inboxTypeMapper = inboxTypeMapper;
        _logger = logger.CreateLogger("INBOX :::: " +context.GetType().FullName);
    }

    public async Task ProcessInboxAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Inbox processor started");
        
        var messages = _context.InboxMessages.Where(e => e.ProcessedDate == null).ToList();
        foreach (var message in messages)
        {
            var type = _inboxTypeMapper.GetType(message.Type);
            var intergrationEvent = JsonSerializer.Deserialize(message.Content, type) as IIntegrationEvent;
            await DispatchIntegrationEventAsync((dynamic)intergrationEvent!);
            
            message.ProcessedDate = DateTime.Now;
            
            await _context.SaveChangesAsync(stoppingToken);
        }
    }

    async Task DispatchIntegrationEventAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent) where TIntegrationEvent : IIntegrationEvent
    {
        var handlers = _handlersResolver.GetIntegrationEventHandlers<TIntegrationEvent>();
        foreach (var handler in handlers)
            await handler.HandleAsync(integrationEvent);
    }
}