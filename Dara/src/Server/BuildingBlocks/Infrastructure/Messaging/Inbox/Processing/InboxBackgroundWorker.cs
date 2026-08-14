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
    private readonly ILogger<InboxBackgroundWorker> _logger;
    
    private int _inboxWorkerLoops = 0;
    private int _inboxProcessorCalls = 0;
    private int _inboxTimeoutCount = 0;

    public InboxBackgroundWorker(InboxQueueSignal inboxQueueSignal, IModuleCompositionRoot compositionRoot, ILogger<InboxBackgroundWorker> logger)
    {
        _inboxQueueSignal = inboxQueueSignal;
        _compositionRoot = compositionRoot;

        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SERVICE STARTED");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _inboxWorkerLoops++;
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
                            _inboxProcessorCalls++;
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
                    _inboxTimeoutCount++;
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    public override void Dispose()
    {
        _logger.LogDebug($"DISPOSING OUTBOX BACKGROUOND WORKER \n\tMAIN LOOPS: {_inboxWorkerLoops} \n\tPROCESSOR CALLS: {_inboxProcessorCalls} \n\tTIMEOUTS: {_inboxTimeoutCount}");
        base.Dispose();
    }
}