using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public class OutboxProcessorService : BackgroundService
{
    private TimeSpan _pollingInterval = TimeSpan.FromSeconds(2);
    private readonly IModuleCompositionRoot _compositionRoot;
    private readonly ILogger<OutboxProcessorService> _logger;
    
    public OutboxProcessorService(IModuleCompositionRoot compositionRoot, ILogger<OutboxProcessorService> logger)
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
                var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
                
                await processor.DispatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox");
            }
        }
    }

    public OutboxProcessorService WithInterval(TimeSpan interval)
    {
        _pollingInterval = interval;
        return this;
    }
}