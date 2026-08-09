using System.Threading.Channels;
using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Apps.API.Processing;

public class HubNotificationsProcessor : BackgroundService
{
    private readonly Channel<IIntegrationEvent> _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HubNotificationsProcessor> _logger;
    
    public HubNotificationsProcessor(Channel<IIntegrationEvent> channel, IServiceProvider serviceProvider, ILogger<HubNotificationsProcessor> logger)
    {
        _channel = channel;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting notifications backgroud worker...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await foreach (var integrationEvent in _channel.Reader.ReadAllAsync(stoppingToken))
                {
                    await DispatchEventAsync((dynamic)integrationEvent, stoppingToken);
                }
                
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
    
    async Task  DispatchEventAsync<T>(T integrationEvent, CancellationToken cancellationToken) where T : IIntegrationEvent
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IHubNotificationHandler<T>>();
        await handler.HandleAsync(integrationEvent,  cancellationToken);
    }
}