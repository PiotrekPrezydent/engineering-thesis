using System.Text.Json;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;

public class InboxBackgroundWorker : BackgroundService
{    
    private readonly InboxQueueSignal _inboxQueueSignal;
    private readonly IModuleCompositionRoot _compositionRoot;
    private readonly ILogger _logger;
    private int inboxLoops = 0;
    private int processorCalls = 0;

    public InboxBackgroundWorker(InboxQueueSignal inboxQueueSignal,IModuleCompositionRoot compositionRoot, ILoggerFactory logger)
    {
        _inboxQueueSignal = inboxQueueSignal;
        _compositionRoot = compositionRoot;
        
        _logger = logger.CreateLogger("INBOX WORKER");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        _logger.LogInformation("STARTED INBOX WORKER SERVICE");
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("INBOX LOOP : " + inboxLoops++);
            try
            {
                IReadOnlyList<Guid> pendingIds;
                await using (var fetchScope = _compositionRoot.CreateAsyncScope())
                {
                    var repository = fetchScope.ServiceProvider.GetRequiredService<IInboxRepository>();
                    pendingIds = await repository.GetPendingMessagesAsync(20, stoppingToken);
                }
                
                foreach (var messageId in pendingIds)
                {
                    await using (var messageScope = _compositionRoot.CreateAsyncScope())
                    {
                        try
                        {
                            _logger.LogInformation("Processor call: " + processorCalls++);
                            var messageProcessor = messageScope.ServiceProvider.GetRequiredService<IInboxMessageProcessor>();
                            await messageProcessor.ProcessSingleMessageAsync(messageId, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to process message {MessageId}. Skipping to next.", messageId);
                        }
                    }
                }
                
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));
                try
                {
                    await _inboxQueueSignal.WaitAsync(timeoutCts.Token);
                }
                catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("INBOX TIMEOUT");
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
    
    async Task DispatchIntegrationEventAsync<TIntegrationEvent>(IServiceProvider currentServiceProvider, TIntegrationEvent integrationEvent) where TIntegrationEvent : IIntegrationEvent
    {
        var handlers = currentServiceProvider.GetServices<IIntegrationEventHandler<TIntegrationEvent>>();
        foreach (var handler in handlers)
            await handler.HandleAsync(integrationEvent);
    }
}