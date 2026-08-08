using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
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


    public InboxBackgroundWorker(InboxQueueSignal inboxQueueSignal, IModuleCompositionRoot compositionRoot, ILoggerFactory logger)
    {
        _inboxQueueSignal = inboxQueueSignal;
        _compositionRoot = compositionRoot;
        using var scope = _compositionRoot.CreateScope();
        var module = scope.ServiceProvider.GetRequiredService<DbContext>();
        _logger = logger.CreateLogger("INBOX WORKER ::: " + module.GetType().Name);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        _logger.LogInformation("STARTED INBOX WORKER SERVICE");
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