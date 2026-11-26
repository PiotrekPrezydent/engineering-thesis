using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Infrastructure;
using Dara.Server.Modules.Plugins.Integration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dara.Server.Modules.Plugins.IntegrationTests;

// the plugins module alone with its outbox and inbox loops, it keeps every state it publishes
public class PluginsModuleHost : IAsyncDisposable
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(25);

    private readonly ServiceProvider _services;
    private readonly IReadOnlyList<IHostedService> _workers;
    private readonly PublishedStates _published = new();

    public InMemoryEventBus EventBus { get; } = new();
    // retries in milliseconds, so a test with a failing handler does not wait seconds
    public static readonly MessageProcessingOptions ProcessingOptions = new()
    {
        Retry = new RetryPolicy(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(200)),
    };

    public PluginsCompositionRoot Root { get; } = new() { ProcessingOptions = ProcessingOptions };

    public IReadOnlyList<PluginFunctionsChangedIntegrationEvent> Published => _published.Snapshot();

    private PluginsModuleHost()
    {
        var services = new ServiceCollection();
        Root.Initialize(services, EventBus);
        EventBus.Subscribe(_published);

        _services = services.BuildServiceProvider();
        _workers = _services.GetServices<IHostedService>().ToList();
    }

    public static async Task<PluginsModuleHost> StartAsync(CancellationToken cancellationToken)
    {
        var host = new PluginsModuleHost();
        foreach (var worker in host._workers)
            await worker.StartAsync(cancellationToken);

        return host;
    }

    // every call has its own scope, like a call of the hub
    public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        await using var scope = _services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IPluginsModule>().ExecuteCommandAsync(command);
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
    {
        await using var scope = _services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<IPluginsModule>().ExecuteQueryAsync<TQuery, TResult>(query);
    }

    // reads module state that the module api has no query for
    public async Task<TResult> ReadAsync<TResult>(Func<IReadModel, Task<TResult>> read)
    {
        await using var scope = Root.CreateAsyncScope();
        return await read(scope.ServiceProvider.GetRequiredService<IReadModel>());
    }

    public async Task<PluginFunctionsChangedIntegrationEvent> WaitForPublishedAsync(Func<PluginFunctionsChangedIntegrationEvent, bool> match, string description)
    {
        PluginFunctionsChangedIntegrationEvent? found = null;
        await WaitUntilAsync(() =>
        {
            found = Published.FirstOrDefault(match);
            return Task.FromResult(found is not null);
        }, description);

        return found!;
    }

    // polls the condition until it holds, fails with the description when the timeout passes
    public static async Task WaitUntilAsync(Func<Task<bool>> condition, string description)
    {
        using var timeoutCts = new CancellationTokenSource(Timeout);

        while (!await condition())
        {
            try
            {
                await Task.Delay(PollInterval, timeoutCts.Token);
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

    // the outbox loop publishes on its own thread
    private class PublishedStates : IIntegrationEventHandler<PluginFunctionsChangedIntegrationEvent>
    {
        private readonly List<PluginFunctionsChangedIntegrationEvent> _events = new();
        private readonly Lock _lock = new();

        public Task HandleAsync(PluginFunctionsChangedIntegrationEvent integrationEvent)
        {
            lock (_lock)
                _events.Add(integrationEvent);

            return Task.CompletedTask;
        }

        public IReadOnlyList<PluginFunctionsChangedIntegrationEvent> Snapshot()
        {
            lock (_lock)
                return _events.ToList();
        }
    }
}
