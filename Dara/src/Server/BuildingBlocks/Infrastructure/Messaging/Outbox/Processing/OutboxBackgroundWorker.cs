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
    private readonly ILogger _logger;
    
    public OutboxBackgroundWorker(IModuleCompositionRoot compositionRoot, ILoggerFactory logger, OutboxQueueSignal outboxQueueSignal)
    {
        _compositionRoot = compositionRoot;
        _outboxQueueSignal = outboxQueueSignal;
        
        _logger = logger.CreateLogger("OUTBOX WORKER ::: " + _compositionRoot.GetModuleName());
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("STARTED OUTBOX PROCESSOR SERVICE");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("OUTBOX LOOP");
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
                    _logger.LogInformation("OUTBOX TIMEOUT");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}