using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;

public class InboxBackgroundWorker : BackgroundService
{    
    private readonly InboxQueueSignal _inboxQueueSignal;
    private readonly IModuleCompositionRoot _compositionRoot;
    private readonly ILogger<InboxBackgroundWorker> _logger;


    public InboxBackgroundWorker(InboxQueueSignal inboxQueueSignal, IModuleCompositionRoot compositionRoot, ILogger<InboxBackgroundWorker> logger)
    {
        _inboxQueueSignal = inboxQueueSignal;
        _compositionRoot = compositionRoot;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        _logger.LogInformation("STARTED INBOX PROCESSOR SERVICE");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _compositionRoot.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IInboxProcessor>();
                await processor.ProcessInboxAsync(stoppingToken);
                
                using var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(30));
                
                var waitSignalTask = _inboxQueueSignal.WaitAsync(stoppingToken);
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