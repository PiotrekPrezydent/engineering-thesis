using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Infrastructure;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Infrastructure;
using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Infrastructure;
using Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Infrastructure;
using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dara.Server.SystemTests.Hosting;

// all server modules in memory like in the api host, without signalr, with an own event bus
public class ModulesHost : IAsyncDisposable
{
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(25);

    private readonly ServiceProvider _services;
    private readonly IReadOnlyList<IHostedService> _workers;

    public InMemoryEventBus EventBus { get; } = new();

    public IdentityCompositionRoot IdentityRoot { get; } = new();
    public ProfilesCompositionRoot ProfilesRoot { get; } = new();
    public GroupsCompositionRoot GroupsRoot { get; } = new();
    public PluginsCompositionRoot PluginsRoot { get; } = new();
    public OrchestrationCompositionRoot OrchestrationRoot { get; }

    public IIdentityModule Identity { get; }
    public IProfilesModule Profiles { get; }
    public IGroupsModule Groups { get; }
    public IPluginsModule Plugins { get; }
    public IOrchestrationModule Orchestration { get; }

    // without a scripted model a prompt fails before any provider call, because tests set no api key
    private ModulesHost(IChatClientProvider? chatClientProvider)
    {
        OrchestrationRoot = chatClientProvider is null
            ? new OrchestrationCompositionRoot()
            : new OrchestrationCompositionRoot([chatClientProvider], new ExecutionPlanGeneratorOptions(TimeoutSeconds: 5));

        var services = new ServiceCollection();

        // same order as in the api host
        IModuleCompositionRoot[] roots = [IdentityRoot, GroupsRoot, ProfilesRoot, PluginsRoot, OrchestrationRoot];
        foreach (var root in roots)
            root.Initialize(services, EventBus);

        _services = services.BuildServiceProvider();
        _workers = _services.GetServices<IHostedService>().ToList();

        Identity = _services.GetRequiredService<IIdentityModule>();
        Profiles = _services.GetRequiredService<IProfilesModule>();
        Groups = _services.GetRequiredService<IGroupsModule>();
        Plugins = _services.GetRequiredService<IPluginsModule>();
        Orchestration = _services.GetRequiredService<IOrchestrationModule>();
    }

    public static async Task<ModulesHost> StartAsync(CancellationToken cancellationToken, IChatClientProvider? chatClientProvider = null)
    {
        var host = new ModulesHost(chatClientProvider);
        foreach (var worker in host._workers)
            await worker.StartAsync(cancellationToken);

        return host;
    }

    // reads module state that the module api has no query for
    public static async Task<TResult> ReadAsync<TResult>(IModuleCompositionRoot module, Func<IReadModel, Task<TResult>> read)
    {
        await using var scope = module.CreateAsyncScope();
        return await read(scope.ServiceProvider.GetRequiredService<IReadModel>());
    }

    // polls the condition until it holds, fails with the description when the timeout passes
    public static async Task WaitUntilAsync(Func<Task<bool>> condition, string description, TimeSpan? timeout = null)
    {
        var limit = timeout ?? DefaultTimeout;
        using var timeoutCts = new CancellationTokenSource(limit);

        while (!await condition())
        {
            try
            {
                await Task.Delay(PollInterval, timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"not met within {limit.TotalSeconds} s: {description}");
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
