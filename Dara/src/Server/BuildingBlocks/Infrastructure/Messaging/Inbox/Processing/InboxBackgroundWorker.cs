using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Processing;

// processes the ready inbox messages one by one, each in its own scope; a failed message gets Attempts, LastError and
// NextAttemptAt in a new scope and waits for its retry, while the later messages go on
public class InboxBackgroundWorker : BackgroundService
{
    private readonly InboxQueueSignal _inboxQueueSignal;
    private readonly IModuleCompositionRoot _compositionRoot;
    private readonly MessageProcessingOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<InboxBackgroundWorker> _logger;

    private int _inboxWorkerLoops = 0;
    private int _inboxProcessorCalls = 0;
    private int _inboxTimeoutCount = 0;

    public InboxBackgroundWorker(InboxQueueSignal inboxQueueSignal, IModuleCompositionRoot compositionRoot, MessageProcessingOptions options, TimeProvider timeProvider, ILogger<InboxBackgroundWorker> logger)
    {
        _inboxQueueSignal = inboxQueueSignal;
        _compositionRoot = compositionRoot;
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
                _inboxWorkerLoops++;
                TimeSpan sleep;

                try
                {
                    sleep = await ProcessReadyMessagesAsync(stoppingToken);
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "Error processing inbox");
                    sleep = Min(TimeSpan.FromSeconds(1), _options.IdleTimeout);
                }

                await WaitAsync(sleep, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    // processes one batch and returns how long the loop may sleep without a signal
    private async Task<TimeSpan> ProcessReadyMessagesAsync(CancellationToken stoppingToken)
    {
        IReadOnlyList<Guid> pendingIds;
        await using (var fetchScope = _compositionRoot.CreateAsyncScope())
        {
            var repository = fetchScope.ServiceProvider.GetRequiredService<IInboxRepository>();
            pendingIds = await repository.GetPendingMessagesAsync(_options.BatchSize, _timeProvider.GetUtcNow().UtcDateTime, stoppingToken);
        }

        foreach (var messageId in pendingIds)
        {
            stoppingToken.ThrowIfCancellationRequested();
            await ProcessMessageAsync(messageId, stoppingToken);
        }

        // a full batch: more messages may be ready
        if (pendingIds.Count >= _options.BatchSize)
            return TimeSpan.Zero;

        DateTime? nextAttemptAt;
        await using (var fetchScope = _compositionRoot.CreateAsyncScope())
        {
            var repository = fetchScope.ServiceProvider.GetRequiredService<IInboxRepository>();
            nextAttemptAt = await repository.GetNextAttemptAtAsync(stoppingToken);
        }

        if (nextAttemptAt == null)
            return _options.IdleTimeout;

        return Min(nextAttemptAt.Value - _timeProvider.GetUtcNow().UtcDateTime, _options.IdleTimeout);
    }

    private async Task ProcessMessageAsync(Guid messageId, CancellationToken stoppingToken)
    {
        try
        {
            _inboxProcessorCalls++;
            await using var messageScope = _compositionRoot.CreateAsyncScope();
            var messageProcessor = messageScope.ServiceProvider.GetRequiredService<IInboxMessageProcessor>();
            await messageProcessor.ProcessSingleMessageAsync(messageId, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message {MessageId}, it waits for a retry.", messageId);
            await RecordFailureAsync(messageId, ex);
        }
    }

    // a new scope, so nothing tracked by the failed attempt (for example a command it queued) is saved with the error
    private async Task RecordFailureAsync(Guid messageId, Exception error)
    {
        try
        {
            await using var failureScope = _compositionRoot.CreateAsyncScope();
            var messageProcessor = failureScope.ServiceProvider.GetRequiredService<IInboxMessageProcessor>();
            await messageProcessor.RecordFailureAsync(messageId, error, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not record the failure of message {MessageId}", messageId);
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
            await _inboxQueueSignal.WaitAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
        {
            _inboxTimeoutCount++;
        }
    }

    private static TimeSpan Min(TimeSpan a, TimeSpan b) => a < b ? a : b;

    public override void Dispose()
    {
        _logger.LogDebug($"DISPOSING INBOX BACKGROUND WORKER \n\tMAIN LOOPS: {_inboxWorkerLoops} \n\tPROCESSOR CALLS: {_inboxProcessorCalls} \n\tTIMEOUTS: {_inboxTimeoutCount}");
        base.Dispose();
    }
}
