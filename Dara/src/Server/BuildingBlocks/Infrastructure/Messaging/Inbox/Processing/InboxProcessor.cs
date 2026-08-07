using System.Text.Json;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;

public class InboxProcessor : IInboxProcessor
{
    private readonly IInboxRepository _inboxRepository;
    private readonly IHandlersResolver _handlersResolver;
    private readonly IInboxTypeMapper _inboxTypeMapper;
    private readonly ILogger _logger;

    public InboxProcessor(IHandlersResolver handlersResolver, IInboxTypeMapper inboxTypeMapper, ILoggerFactory logger, IInboxRepository inboxRepository)
    {
        _handlersResolver = handlersResolver;
        _inboxTypeMapper = inboxTypeMapper;
        _inboxRepository = inboxRepository;
        _logger = logger.CreateLogger("INBOX :::: " +_inboxRepository.GetType().FullName);
    }

    public async Task ProcessInboxAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Inbox processor started");
        
        var messages = await _inboxRepository.GetPendingMessagesAsync(stoppingToken);
        foreach (var message in messages)
        {
            var type = _inboxTypeMapper.GetType(message.Type);
            var integrationEvent = JsonSerializer.Deserialize(message.Content, type) as IIntegrationEvent;
            await DispatchIntegrationEventAsync((dynamic)integrationEvent!);
            
            await _inboxRepository.MarkAsCompletedAsync(message.Id, stoppingToken);
        }
    }

    async Task DispatchIntegrationEventAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent) where TIntegrationEvent : IIntegrationEvent
    {
        var handlers = _handlersResolver.GetIntegrationEventHandlers<TIntegrationEvent>();
        foreach (var handler in handlers)
            await handler.HandleAsync(integrationEvent);
    }
}