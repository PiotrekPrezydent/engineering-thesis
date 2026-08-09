using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
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
        using var scope = _compositionRoot.CreateScope();
        var module = scope.ServiceProvider.GetRequiredService<DbContext>();
        _logger = logger.CreateLogger("OUTBOX WORKER ::: " + module.GetType().Name);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("STARTED OUTBOX PROCESSOR SERVICE");
        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Stopping token called");
        });
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("OUTBOX LOOP");
            try
            {
                using var scope = _compositionRoot.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
                await processor.ProcessOutboxAsync(stoppingToken);
                
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