using Dara.Server.Apps.API.Services.Seeding;

namespace Dara.Server.Apps.API.Services;

// runs module seeders after the host started, so outbox and inbox loops of modules already work
public class StartupSeedService : IHostedLifecycleService
{
    private readonly ILogger<StartupSeedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly SeedEnvironment _environment;

    public StartupSeedService(ILogger<StartupSeedService> logger, IServiceProvider serviceProvider, SeedEnvironment environment)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _environment = environment;
    }
    
    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        if (!_environment.Enabled)
        {
            _logger.LogInformation("Seed disabled");
            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        
        var data = new SeedData();
        foreach (var seeder in scope.ServiceProvider.GetServices<ISeeder>())
            await seeder.SeedAsync(data, cancellationToken);

        await scope.ServiceProvider.GetRequiredService<LogModulesDataService>().LogDataAsync();
    }
    
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}