using System.Text.Json;
using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;

public class InboxProcessor : IInboxProcessor
{
    private readonly IInboxRepository _inboxRepository;
    private readonly IHandlersResolver _handlersResolver;
    private readonly IInboxTypeMapper _inboxTypeMapper;
    private readonly ILogger _logger;

    public InboxProcessor(IHandlersResolver handlersResolver, IInboxTypeMapper inboxTypeMapper, ILoggerFactory logger, IInboxRepository inboxRepository, DbContext module)
    {
        _handlersResolver = handlersResolver;
        _inboxTypeMapper = inboxTypeMapper;
        _inboxRepository = inboxRepository;
        _logger = logger.CreateLogger("INBOX PROCESSOR :::: " +module.GetType().Name);
    }

    public async Task ProcessInboxAsync(CancellationToken stoppingToken)
    {
        var messages = await _inboxRepository.GetPendingMessagesAsync(stoppingToken);
        
        foreach (var message in messages)
        {
            var type = _inboxTypeMapper.GetType(message.Type);
            var integrationEvent = JsonSerializer.Deserialize(message.Content, type) as IIntegrationEvent;
            await DispatchIntegrationEventAsync((dynamic)integrationEvent!);
            
            await _inboxRepository.MarkAsCompletedAsync(message.Id, stoppingToken);
            _logger.LogInformation($"PROCESSED INBOX MESSAGE {message.Type} IN {(message.ProcessedDate! - message.OccurredOn).Value.TotalSeconds}");
        }
    }

    async Task DispatchIntegrationEventAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent) where TIntegrationEvent : IIntegrationEvent
    {
        var handlers = _handlersResolver.GetIntegrationEventHandlers<TIntegrationEvent>();
        foreach (var handler in handlers)
            await handler.HandleAsync(integrationEvent);
    }
}