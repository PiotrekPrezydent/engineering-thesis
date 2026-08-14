using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Dara.Shared.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;

public class OutboxBackgroundWorker : BackgroundService
{
    private readonly OutboxQueueSignal _outboxQueueSignal;
    private readonly IModuleCompositionRoot _compositionRoot;
    private readonly ILogger<OutboxBackgroundWorker> _logger;
    
    private int _outboxWorkerLoops = 0;
    private int _outboxProcessorCalls = 0;
    private int _outboxTimeoutCount = 0;
    
    public OutboxBackgroundWorker(IModuleCompositionRoot compositionRoot, ILogger<OutboxBackgroundWorker> logger, OutboxQueueSignal outboxQueueSignal)
    {
        _compositionRoot = compositionRoot;
        _outboxQueueSignal = outboxQueueSignal;

        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SERVICE STARTED");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _outboxWorkerLoops++;
            try
            {
                IReadOnlyList<Guid> pendingIds;
                await using (var fetchScope = _compositionRoot.CreateAsyncScope())
                {
                    var repository = fetchScope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                    pendingIds = await repository.GetPendingMessagesAsync(20, stoppingToken);
                }
                foreach (var messageId in pendingIds)
                {
                    await using (var messageScope = _compositionRoot.CreateAsyncScope())
                    {
                        try
                        {
                            _outboxProcessorCalls++;
                            var messageProcessor = messageScope.ServiceProvider.GetRequiredService<IOutboxMessageProcessor>();
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
                    await _outboxQueueSignal.WaitAsync(timeoutCts.Token);
                }
                catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
                {
                    _outboxTimeoutCount++;
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
        _logger.LogDebug($"DISPOSING OUTBOX BACKGROUOND WORKER \n\tMAIN LOOPS: {_outboxWorkerLoops} \n\tPROCESSOR CALLS: {_outboxProcessorCalls} \n\tTIMEOUTS: {_outboxTimeoutCount}");
        base.Dispose();
    }
}