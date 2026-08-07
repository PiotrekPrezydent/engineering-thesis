using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;

public class InboxProcessorService : BackgroundService
{    
    private TimeSpan _pollingInterval = TimeSpan.FromSeconds(2);
    private readonly IModuleCompositionRoot _compositionRoot;
    private readonly ILogger<InboxProcessorService> _logger;
    
    public InboxProcessorService(IModuleCompositionRoot compositionRoot, ILogger<InboxProcessorService> logger)
    {
        _compositionRoot = compositionRoot;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        _logger.LogInformation("STARTED OUTBOX PROCESSOR SERVICE");
        
        using var timer = new PeriodicTimer(_pollingInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _compositionRoot.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IInboxProcessor>();
                
                await processor.ProcessInboxAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox");
            }
        }
    }
}