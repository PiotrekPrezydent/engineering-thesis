using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.Modules.Authorization.Application;
using Dara.Server.Modules.Authorization.Application.Definitions.SynchronizeDefinitions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dara.Server.Modules.Authorization.Infrastructure.Definitions;

// a hosted service of the host: every module has added its checked definition to the services of the host in
// Initialize, so before any hosted service starts (seeding, the inbox loops) Authorization synchronizes them all;
// an error stops the start
public class AuthorizationDefinitionsSynchronizer : IHostedLifecycleService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AuthorizationDefinitionsSynchronizer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var definitions = scope.ServiceProvider.GetServices<IModuleAuthorizationDefinition>().ToList();
        await scope.ServiceProvider.GetRequiredService<IAuthorizationModule>().ExecuteCommandAsync(new SynchronizeDefinitionsCommand(definitions));
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
