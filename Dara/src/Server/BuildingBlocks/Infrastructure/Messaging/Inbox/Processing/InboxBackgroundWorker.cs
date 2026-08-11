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
            _logger.LogInformation("INBOX LOOP");
            try
            {
                using var scope = _compositionRoot.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IInboxProcessor>();
                await processor.ProcessInboxAsync(stoppingToken);
                
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
}