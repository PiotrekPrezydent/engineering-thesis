using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Authorization.Application;
using Dara.Server.Modules.Authorization.Application.Definitions.SynchronizeDefinitions;
using Dara.Server.Modules.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dara.Server.Modules.Authorization.IntegrationTests;

// the authorization module alone with its inbox and internal commands loops and the definition of a module "Test" in
// the services of the host; the events of the modules are published on the bus by the tests
public class AuthorizationModuleHost : IAsyncDisposable
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private readonly ServiceProvider _services;
    private readonly IReadOnlyList<IHostedService> _workers;

    public InMemoryEventBus EventBus { get; } = new();

    public AuthorizationCompositionRoot Root { get; } = new();

    private AuthorizationModuleHost()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IModuleAuthorizationDefinition>(TestDefinition.Instance);
        Root.Initialize(services, EventBus);

        _services = services.BuildServiceProvider();
        _workers = _services.GetServices<IHostedService>().ToList();
    }

    // started like the generic host starts it: StartingAsync of every lifecycle service (the synchronization of the
    // definitions), then StartAsync of every hosted service
    public static async Task<AuthorizationModuleHost> StartAsync(CancellationToken cancellationToken)
    {
        var host = new AuthorizationModuleHost();
        foreach (var worker in host._workers.OfType<IHostedLifecycleService>())
            await worker.StartingAsync(cancellationToken);

        foreach (var worker in host._workers)
            await worker.StartAsync(cancellationToken);

        return host;
    }

    // a later start with other definitions
    public Task SynchronizeAsync(params IModuleAuthorizationDefinition[] definitions)
    {
        return ExecuteAsync(new SynchronizeDefinitionsCommand([Root.AuthorizationDefinition, .. definitions]));
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

// a module "Test" with one scope: Viewer reads, Editor edits (and reads, its subordinate), Manager passes access on
// to every role but the owner, Owner may do everything and manages every role
public sealed record TestDefinition(IReadOnlyList<RoleDefinition> Roles) : IModuleAuthorizationDefinition
{
    public static readonly ScopeDefinition Item = new("Test.Item");
    public static readonly PermissionDefinition Read = new("test.item.read", Item);
    public static readonly PermissionDefinition Edit = new("test.item.edit", Item);
    public static readonly PermissionDefinition ManageAccess = new(Item.ManageAccessPermissionCode, Item);

    public static readonly RoleDefinition Viewer = new("Test.Viewer", Item, [Read]);
    public static readonly RoleDefinition Editor = new("Test.Editor", Item, [Edit]) { Subordinates = [Viewer] };
    public static readonly RoleDefinition Manager = new("Test.Manager", Item, [ManageAccess])
    {
        Subordinates = [Viewer],
        ManageableRoleCodes = ["Test.Manager", "Test.Editor", "Test.Viewer"]
    };
    // a child scope: one part of an item has its own id; the owner of the item gets PartOwner on each part with it
    public static readonly ScopeDefinition Part = new("Test.Item.Part");
    public static readonly PermissionDefinition UsePart = new("test.item.part.use", Part);
    public static readonly PermissionDefinition ManagePartAccess = new(Part.ManageAccessPermissionCode, Part);
    public static readonly RoleDefinition PartOwner = new("Test.PartOwner", Part, [UsePart, ManagePartAccess])
    {
        ManageableRoleCodes = ["Test.PartOwner"]
    };

    public static readonly RoleDefinition Owner = new("Test.Owner", Item, [])
    {
        Subordinates = [Manager, Editor, PartOwner],
        ManageableRoleCodes = ["Test.Owner", "Test.Manager", "Test.Editor", "Test.Viewer"]
    };

    public static readonly TestDefinition Instance = new([Owner, Manager, Editor, Viewer, PartOwner]);

    public string ModuleName => "Test";
    public IEnumerable<ScopeDefinition> GetScopes() => [Item, Part];
    public IEnumerable<PermissionDefinition> GetPermissions() => [Read, Edit, ManageAccess, UsePart, ManagePartAccess];
    public IEnumerable<RoleDefinition> GetDefaultRoles() => Roles;
}
