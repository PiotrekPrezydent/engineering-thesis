using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.BuildingBlocks.Infrastructure.Common;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Mapping;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;
using Dara.Server.BuildingBlocks.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dara.Server.BuildingBlocks.UnitTests;

// a module of BuildingBlocks only: its own DbContext (ModuleContextBase loads the Domain assembly of a module by name),
// the internal commands, outbox and inbox services and test handlers driven by the Probe
public sealed class TestModule : IModuleCompositionRoot, IAsyncDisposable
{
    public static MessageProcessingOptions FastOptions { get; } = new()
    {
        IdleTimeout = TimeSpan.FromSeconds(1),
        Retry = new RetryPolicy(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(100)),
    };

    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

    private readonly ServiceProvider _provider;
    private readonly List<IHostedService> _started = [];

    public Probe Probe { get; }

    public TestModule(MessageProcessingOptions? options = null, Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        var databaseName = $"test-module-{Guid.NewGuid():N}";

        services.AddDbContext<TestDbContext>(o => o.UseInMemoryDatabase(databaseName));
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TestDbContext>());
        services.AddLogging();
        services.AddSingleton(options ?? FastOptions);
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IModuleCompositionRoot>(this);
        services.AddSingleton<Probe>();

        services.AddScoped<IHandlersResolver, HandlersResolver>();
        services.AddScoped<ICommandExecutor, CommandExecutor>();

        services.AddScoped<IInternalCommandScheduler, InternalCommandScheduler>();
        services.AddScoped<IInternalCommandProcessor, InternalCommandProcessor>();
        services.AddScoped<IInternalCommandRepository, InternalCommandRepository>();
        services.AddSingleton<IInternalCommandsTypeMapper>(InternalCommandsTypeMapper.FromAssembly(typeof(TestModule).Assembly));
        services.AddSingleton<InternalCommandQueueSignal>();
        services.AddSingleton<InternalCommandsBackgroundWorker>();

        var outboxMap = new BiDictionary<Type, string>();
        outboxMap.Add(typeof(TestNotification), nameof(TestNotification));
        services.AddSingleton<IOutboxMessagesTypeMapper>(new OutboxMessagesTypeMapper(outboxMap));
        services.AddScoped<IOutboxRepository, OutboxRepository<TestDbContext>>();
        services.AddTransient<IOutboxMessageProcessor, OutboxMessageProcessor>();
        services.AddSingleton<OutboxQueueSignal>();
        services.AddSingleton<OutboxBackgroundWorker>();

        var inboxMap = new BiDictionary<Type, string>();
        inboxMap.Add(typeof(TestIntegrationEvent), nameof(TestIntegrationEvent));
        services.AddSingleton<IInboxMessagesTypeMapper>(new InboxMessagesTypeMapper(inboxMap));
        services.AddScoped<IInboxRepository, InboxRepository<TestDbContext>>();
        services.AddTransient<IInboxMessageProcessor, InboxMessageProcessor>();
        services.AddSingleton<InboxQueueSignal>();

        services.AddScoped<ICommandHandler<EffectCommand>, EffectCommandHandler>();
        services.AddScoped<ICommandHandler<OrderedEffectCommand>, EffectCommandHandler>();
        services.AddScoped<ICommandHandler<GroupedEffectCommand>, EffectCommandHandler>();

        configure?.Invoke(services);

        _provider = services.BuildServiceProvider();
        Probe = _provider.GetRequiredService<Probe>();
    }

    public IServiceScope CreateScope() => _provider.CreateScope();

    public AsyncServiceScope CreateAsyncScope() => _provider.CreateAsyncScope();

    public void Initialize(IServiceCollection rootServices, IEventBus eventBus) => throw new NotSupportedException();

    public T GetRequiredService<T>() where T : notnull => _provider.GetRequiredService<T>();

    public async Task StartAsync<TWorker>() where TWorker : IHostedService
    {
        var worker = _provider.GetRequiredService<TWorker>();
        await worker.StartAsync(CancellationToken.None);
        _started.Add(worker);
    }

    // queues the commands in one scope and saves them with one SaveChanges, as a handler of a message does
    public async Task EnqueueAsync(params ICommand[] commands)
    {
        await using var scope = CreateAsyncScope();
        var scheduler = scope.ServiceProvider.GetRequiredService<IInternalCommandScheduler>();
        foreach (var command in commands)
            await scheduler.EnqueueAsync(command);

        await scope.ServiceProvider.GetRequiredService<TestDbContext>().SaveChangesAsync();
    }

    public async Task<T> ReadAsync<T>(Func<TestDbContext, Task<T>> query)
    {
        await using var scope = CreateAsyncScope();
        return await query(scope.ServiceProvider.GetRequiredService<TestDbContext>());
    }

    public Task<int> CountEffectsAsync(string name) => ReadAsync(db => db.Effects.CountAsync(e => e.Name == name));

    public Task<InternalCommand> GetCommandAsync(string name) =>
        ReadAsync(db => db.InternalCommands.AsNoTracking().SingleAsync(c => c.Data.Contains($"\"Name\":\"{name}\"")));

    public async Task WaitUntilAsync(Func<Task<bool>> condition, string description)
    {
        using var timeoutCts = new CancellationTokenSource(Timeout);
        while (!await condition())
        {
            try
            {
                await Task.Delay(10, timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"not met within {Timeout.TotalSeconds} s: {description}");
            }
        }
    }

    public Task WaitUntilProcessedAsync(string commandName) =>
        WaitUntilAsync(async () => (await GetCommandAsync(commandName)).ProcessedDate != null, $"command {commandName} processed");

    public async ValueTask DisposeAsync()
    {
        Probe.ReleaseAll();
        foreach (var worker in _started)
            await worker.StopAsync(CancellationToken.None);

        await _provider.DisposeAsync();
    }
}

public class TestDbContext : DbContext
{
    public DbSet<Effect> Effects { get; set; }
    public DbSet<InternalCommand> InternalCommands { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InternalCommandEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageEntityTypeConfiguration());
        modelBuilder.Entity<Effect>().HasKey(e => e.Id);
    }
}

// what a command handler changed; stored in the same SaveChanges as ProcessedDate of the command
public class Effect
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
}

public record EffectCommand(string Name) : ICommand;

public record OrderedEffectCommand(string Name, string Key) : IOrderedInternalCommand
{
    [JsonIgnore]
    public string OrderingKey => Key;
}

public record GroupedEffectCommand(string Name, string Group) : IConcurrencyLimitedInternalCommand
{
    [JsonIgnore]
    public string ConcurrencyGroup => Group;
}

// a command with a result cannot be queued
public record CommandWithResult(string Name) : ICommand<int>;

public record TestDomainEvent(string Name) : DomainEventBase;

public record TestNotification : DomainEventNotificationBase<TestDomainEvent>
{
    public TestNotification(Guid notificationId, TestDomainEvent domainEvent) : base(notificationId, domainEvent)
    {
    }
}

public record TestIntegrationEvent(Guid EventId, DateTime OccurredOn, string Name) : IntegrationEventBase(EventId, OccurredOn);

// shared state of the test handlers: failures to throw, gates to wait on, what ran and in which order
public class Probe
{
    private readonly ConcurrentDictionary<string, int> _failuresLeft = new();
    private readonly ConcurrentDictionary<string, int> _attempts = new();
    private readonly ConcurrentDictionary<string, TaskCompletionSource> _gates = new();
    private readonly ConcurrentDictionary<string, int> _runningInGroup = new();
    private readonly ConcurrentDictionary<string, int> _maxRunningInGroup = new();
    private readonly ConcurrentQueue<string> _started = new();
    private readonly ConcurrentQueue<string> _finished = new();

    public IReadOnlyList<string> Started => _started.ToList();
    public IReadOnlyList<string> Finished => _finished.ToList();

    public void FailFirst(string name, int times) => _failuresLeft[name] = times;

    public void Hold(string name) => _gates[name] = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

    public void Release(string name)
    {
        if (_gates.TryGetValue(name, out var gate))
            gate.TrySetResult();
    }

    public void ReleaseAll()
    {
        foreach (var gate in _gates.Values)
            gate.TrySetResult();
    }

    public int MaxRunningInGroup(string group) => _maxRunningInGroup.GetValueOrDefault(group);

    // throws while failures are left for the name, the message says which attempt failed
    public void ThrowIfFailing(string name)
    {
        var attempt = _attempts.AddOrUpdate(name, 1, (_, n) => n + 1);
        var left = _failuresLeft.GetValueOrDefault(name);
        if (left <= 0)
            return;

        _failuresLeft[name] = left - 1;
        throw new InvalidOperationException($"failure {attempt} of {name}");
    }

    public async Task RunAsync(string name, string? group, Func<Task> effect)
    {
        _started.Enqueue(name);
        if (group != null)
        {
            var running = _runningInGroup.AddOrUpdate(group, 1, (_, n) => n + 1);
            _maxRunningInGroup.AddOrUpdate(group, running, (_, max) => Math.Max(max, running));
        }

        try
        {
            if (_gates.TryGetValue(name, out var gate))
                await gate.Task;

            ThrowIfFailing(name);
            await effect();
            _finished.Enqueue(name);
        }
        finally
        {
            if (group != null)
                _runningInGroup.AddOrUpdate(group, 0, (_, n) => n - 1);
        }
    }
}

// stands for a command handler with its unit of work: the effect and ProcessedDate of the command in one SaveChanges
public class EffectCommandHandler :
    ICommandHandler<EffectCommand>,
    ICommandHandler<OrderedEffectCommand>,
    ICommandHandler<GroupedEffectCommand>
{
    private readonly TestDbContext _context;
    private readonly Probe _probe;

    public EffectCommandHandler(TestDbContext context, Probe probe)
    {
        _context = context;
        _probe = probe;
    }

    public Task HandleAsync(EffectCommand command) => _probe.RunAsync(command.Name, null, () => SaveEffectAsync(command.Name));

    public Task HandleAsync(OrderedEffectCommand command) => _probe.RunAsync(command.Name, null, () => SaveEffectAsync(command.Name));

    public Task HandleAsync(GroupedEffectCommand command) => _probe.RunAsync(command.Name, command.Group, () => SaveEffectAsync(command.Name));

    private async Task SaveEffectAsync(string name)
    {
        _context.Effects.Add(new Effect { Name = name });
        await _context.SaveChangesAsync();
    }
}

// the first handler of the notification queues a command, the second one fails while the Probe says so
public class QueueCommandOnNotificationHandler : IDomainEventNotificationHandler<TestNotification>
{
    private readonly IInternalCommandScheduler _scheduler;

    public QueueCommandOnNotificationHandler(IInternalCommandScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public Task HandleAsync(TestNotification notification) => _scheduler.EnqueueAsync(new EffectCommand(notification.DomainEvent.Name));
}

public class FlakyNotificationHandler : IDomainEventNotificationHandler<TestNotification>
{
    private readonly Probe _probe;

    public FlakyNotificationHandler(Probe probe)
    {
        _probe = probe;
    }

    public Task HandleAsync(TestNotification notification)
    {
        _probe.ThrowIfFailing("notification:" + notification.DomainEvent.Name);
        return Task.CompletedTask;
    }
}
