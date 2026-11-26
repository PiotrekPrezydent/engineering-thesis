using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

// one per module: runs up to InternalCommandsParallelism commands at once, each in its own scope, so a long command
// takes one slot and holds back neither the other commands nor the outbox and the inbox of the module
public class InternalCommandsBackgroundWorker : BackgroundService
{
    private readonly IModuleCompositionRoot _compositionRoot;
    private readonly InternalCommandQueueSignal _signal;
    private readonly MessageProcessingOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<InternalCommandsBackgroundWorker> _logger;

    // touched only by the loop; a finished command only signals the loop
    private readonly Dictionary<Guid, (RunningInternalCommand Command, Task Task)> _running = new();

    public InternalCommandsBackgroundWorker(IModuleCompositionRoot compositionRoot, InternalCommandQueueSignal signal, MessageProcessingOptions options, TimeProvider timeProvider, ILogger<InternalCommandsBackgroundWorker> logger)
    {
        if (options.InternalCommandsParallelism < 1)
            throw new ArgumentOutOfRangeException(nameof(options), "InternalCommandsParallelism must be at least 1.");

        _compositionRoot = compositionRoot;
        _signal = signal;
        _options = options;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SERVICE STARTED");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var sleep = _options.IdleTimeout;

                try
                {
                    sleep = await StartReadyCommandsAsync(stoppingToken);
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "Error reading internal commands");
                    sleep = Min(TimeSpan.FromSeconds(1), _options.IdleTimeout);
                }

                await WaitAsync(sleep, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }

        // no new commands after the stop; the running ones finish, an unfinished one has no ProcessedDate and returns
        await Task.WhenAll(_running.Values.Select(r => r.Task));
    }

    // starts the ready commands and returns how long the loop may sleep without a signal
    private async Task<TimeSpan> StartReadyCommandsAsync(CancellationToken stoppingToken)
    {
        foreach (var finished in _running.Where(r => r.Value.Task.IsCompleted).Select(r => r.Key).ToList())
            _running.Remove(finished);

        var freeSlots = _options.InternalCommandsParallelism - _running.Count;
        if (freeSlots <= 0)
            return _options.IdleTimeout;

        IReadOnlyList<PendingInternalCommand> pending;
        await using (var fetchScope = _compositionRoot.CreateAsyncScope())
        {
            var repository = fetchScope.ServiceProvider.GetRequiredService<IInternalCommandRepository>();
            pending = await repository.GetPendingCommandsAsync(_options.InternalCommandsLookahead, stoppingToken);
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var selection = InternalCommandSelector.Select(
            pending,
            _running.Values.Select(r => r.Command).ToList(),
            now,
            freeSlots,
            _options.ConcurrencyLimits);

        foreach (var command in selection.Ready)
        {
            var running = new RunningInternalCommand(command.Id, command.OrderingKey, command.ConcurrencyGroup);
            var task = Task.Run(() => RunCommandAsync(command.Id, stoppingToken), CancellationToken.None);
            _running.Add(command.Id, (running, task));
        }

        if (selection.NextAttemptAt == null)
            return _options.IdleTimeout;

        return Min(selection.NextAttemptAt.Value - now, _options.IdleTimeout);
    }

    private async Task RunCommandAsync(Guid commandId, CancellationToken stoppingToken)
    {
        try
        {
            await using var scope = _compositionRoot.CreateAsyncScope();
            var processor = scope.ServiceProvider.GetRequiredService<IInternalCommandProcessor>();
            await processor.ProcessAsync(commandId, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // the host stops; the command was not executed and returns at the next start
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal command {CommandId} failed", commandId);
            await RecordFailureAsync(commandId, ex);
        }
        finally
        {
            _signal.Notify();
        }
    }

    // a new scope, so nothing tracked by the failed attempt (for example a command it queued) is saved with the error
    private async Task RecordFailureAsync(Guid commandId, Exception error)
    {
        try
        {
            await using var scope = _compositionRoot.CreateAsyncScope();
            var processor = scope.ServiceProvider.GetRequiredService<IInternalCommandProcessor>();
            await processor.RecordFailureAsync(commandId, error, CancellationToken.None);
        }
        catch (Exception ex)
        {
            // the command stays ready and is tried again at once; a failure to store the error is rare
            _logger.LogError(ex, "Could not record the failure of internal command {CommandId}", commandId);
        }
    }

    private async Task WaitAsync(TimeSpan sleep, CancellationToken stoppingToken)
    {
        if (sleep <= TimeSpan.Zero)
            return;

        using var timeoutCts = new CancellationTokenSource(sleep, _timeProvider);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, timeoutCts.Token);

        try
        {
            await _signal.WaitAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
        {
        }
    }

    private static TimeSpan Min(TimeSpan a, TimeSpan b) => a < b ? a : b;
}
