using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration;
using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Infrastructure;
using Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;
using Dara.Server.Modules.Orchestration.Integration;
using Dara.Server.Modules.Orchestration.IntegrationTests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dara.Server.Modules.Orchestration.IntegrationTests;

// the orchestration module alone with its outbox and inbox loops and a scripted model, it keeps every event it publishes
public class OrchestrationModuleHost : IAsyncDisposable
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(25);

    private readonly ServiceProvider _services;
    private readonly IReadOnlyList<IHostedService> _workers;
    private readonly PublishedEvents _published = new();

    public InMemoryEventBus EventBus { get; } = new();
    public OrchestrationCompositionRoot Root { get; }
    public ScriptedChatClientProvider Model { get; }

    public IReadOnlyList<IIntegrationEvent> Published => _published.Snapshot();

    private OrchestrationModuleHost(ScriptedChatClientProvider model)
    {
        Model = model;
        Root = new OrchestrationCompositionRoot([model], new ExecutionPlanGeneratorOptions(TimeoutSeconds: 5));

        var services = new ServiceCollection();
        Root.Initialize(services, EventBus);
        EventBus.Subscribe<AgentRunStartedIntegrationEvent>(_published);
        EventBus.Subscribe<ExecutionPlanCreatedIntegrationEvent>(_published);
        EventBus.Subscribe<ExecutionPlanGenerationFailedIntegrationEvent>(_published);
        EventBus.Subscribe<ExecutionPlanApprovedIntegrationEvent>(_published);
        EventBus.Subscribe<AgentRunRejectedIntegrationEvent>(_published);

        _services = services.BuildServiceProvider();
        _workers = _services.GetServices<IHostedService>().ToList();
    }

    public static async Task<OrchestrationModuleHost> StartAsync(CancellationToken cancellationToken, params ScriptedRound[] rounds)
    {
        var host = new OrchestrationModuleHost(new ScriptedChatClientProvider(rounds));
        foreach (var worker in host._workers)
            await worker.StartAsync(cancellationToken);

        return host;
    }

    // every call has its own scope, like a call of the hub
    public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        await using var scope = _services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IOrchestrationModule>().ExecuteCommandAsync(command);
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
    {
        await using var scope = _services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<IOrchestrationModule>().ExecuteQueryAsync<TQuery, TResult>(query);
    }

    // reads module state that the module api has no query for
    public async Task<TResult> ReadAsync<TResult>(Func<IReadModel, Task<TResult>> read)
    {
        await using var scope = Root.CreateAsyncScope();
        return await read(scope.ServiceProvider.GetRequiredService<IReadModel>());
    }

    public async Task<TEvent> WaitForPublishedAsync<TEvent>(Func<TEvent, bool> match, string description) where TEvent : IIntegrationEvent
    {
        TEvent? found = default;
        await WaitUntilAsync(() =>
        {
            found = Published.OfType<TEvent>().FirstOrDefault(match);
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
    private class PublishedEvents :
        IIntegrationEventHandler<AgentRunStartedIntegrationEvent>,
        IIntegrationEventHandler<ExecutionPlanCreatedIntegrationEvent>,
        IIntegrationEventHandler<ExecutionPlanGenerationFailedIntegrationEvent>,
        IIntegrationEventHandler<ExecutionPlanApprovedIntegrationEvent>,
        IIntegrationEventHandler<AgentRunRejectedIntegrationEvent>
    {
        private readonly List<IIntegrationEvent> _events = new();
        private readonly Lock _lock = new();

        public Task HandleAsync(AgentRunStartedIntegrationEvent integrationEvent) => Add(integrationEvent);
        public Task HandleAsync(ExecutionPlanCreatedIntegrationEvent integrationEvent) => Add(integrationEvent);
        public Task HandleAsync(ExecutionPlanGenerationFailedIntegrationEvent integrationEvent) => Add(integrationEvent);
        public Task HandleAsync(ExecutionPlanApprovedIntegrationEvent integrationEvent) => Add(integrationEvent);
        public Task HandleAsync(AgentRunRejectedIntegrationEvent integrationEvent) => Add(integrationEvent);

        public IReadOnlyList<IIntegrationEvent> Snapshot()
        {
            lock (_lock)
                return _events.ToList();
        }

        private Task Add(IIntegrationEvent integrationEvent)
        {
            lock (_lock)
                _events.Add(integrationEvent);

            return Task.CompletedTask;
        }
    }
}
