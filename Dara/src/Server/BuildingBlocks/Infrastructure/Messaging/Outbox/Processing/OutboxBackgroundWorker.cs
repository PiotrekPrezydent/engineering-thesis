using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;

public class OutboxBackgroundWorker : BackgroundService
{
    private readonly OutboxQueueSignal _outboxQueueSignal;
    private readonly IModuleCompositionRoot _compositionRoot;
    private readonly ILogger<OutboxBackgroundWorker> _logger;
    
    public OutboxBackgroundWorker(IModuleCompositionRoot compositionRoot, ILogger<OutboxBackgroundWorker> logger, OutboxQueueSignal outboxQueueSignal)
    {
        _compositionRoot = compositionRoot;
        _logger = logger;
        _outboxQueueSignal = outboxQueueSignal;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("STARTED OUTBOX PROCESSOR SERVICE");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _compositionRoot.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
                await processor.ProcessOutboxAsync(stoppingToken);
                
                using var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(30));
                
                var waitSignalTask = _outboxQueueSignal.WaitAsync(stoppingToken);
                var waitTimerTask = periodicTimer.WaitForNextTickAsync(stoppingToken).AsTask();
                
                await Task.WhenAny(waitSignalTask, waitTimerTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}