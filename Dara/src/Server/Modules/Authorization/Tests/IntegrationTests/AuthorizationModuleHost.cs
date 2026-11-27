using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Authorization.Application;
using Dara.Server.Modules.Authorization.Application.Roles.SynchronizePermissionsAndRoles;
using Dara.Server.Modules.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dara.Server.Modules.Authorization.IntegrationTests;

// the authorization module alone with its inbox and internal commands loops; the events of Identity are published
// on the bus by the tests
public class AuthorizationModuleHost : IAsyncDisposable
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private readonly ServiceProvider _services;
    private readonly IReadOnlyList<IHostedService> _workers;

    public InMemoryEventBus EventBus { get; } = new();

    public AuthorizationCompositionRoot Root { get; } = new();

    // a module "Test": Owner allows everything and is not default, Viewer is default and allows Read
    public static ModuleAuthorizationCatalog TestCatalog { get; } = new("Test",
        ["TestPermission.Edit", "TestPermission.Read"],
        [
            new ModuleRoleDefinition("TestRole.Owner", false, ["TestPermission.Edit", "TestPermission.Read"]),
            new ModuleRoleDefinition("TestRole.Viewer", true, ["TestPermission.Read"])
        ]);

    private AuthorizationModuleHost()
    {
        var services = new ServiceCollection();
        Root.Initialize(services, EventBus);

        _services = services.BuildServiceProvider();
        _workers = _services.GetServices<IHostedService>().ToList();
    }

    // started like the host starts it: the catalogs of the modules are synchronized before the first call
    public static async Task<AuthorizationModuleHost> StartAsync(CancellationToken cancellationToken)
    {
        var host = new AuthorizationModuleHost();
        foreach (var worker in host._workers)
            await worker.StartAsync(cancellationToken);

        await host.SynchronizeAsync(TestCatalog);
        return host;
    }

    public Task SynchronizeAsync(params ModuleAuthorizationCatalog[] otherCatalogs)
    {
        return ExecuteAsync(new SynchronizePermissionsAndRolesCommand([Root.AuthorizationCatalog, .. otherCatalogs]));
    }

    // every call has its own scope, like a call of the api
    public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        await using var scope = _services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IAuthorizationModule>().ExecuteCommandAsync(command);
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
    {
        await using var scope = _services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<IAuthorizationModule>().ExecuteQueryAsync<TQuery, TResult>(query);
    }

    // reads module state that the module api has no query for
    public async Task<TResult> ReadAsync<TResult>(Func<IReadModel, TResult> read)
    {
        await using var scope = Root.CreateAsyncScope();
        return read(scope.ServiceProvider.GetRequiredService<IReadModel>());
    }

    public async Task WaitUntilAsync(Func<IReadModel, bool> condition, string description)
    {
        using var timeoutCts = new CancellationTokenSource(Timeout);
        while (!await ReadAsync(condition))
        {
            try
            {
                await Task.Delay(20, timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"not met within {Timeout.TotalSeconds} s: {description}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var worker in _workers.Reverse())
            await worker.StopAsync(CancellationToken.None);

        await _services.DisposeAsync();
    }
}
