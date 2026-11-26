using System.Text.Json;
using Dara.Server.BuildingBlocks.Application.Commands;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

// adds the command to the tracked DbContext of the scope and does not save it: the command is stored together
// with the change that queued it (ProcessedDate of an outbox or inbox message, or the changes of a command handler)
public class InternalCommandScheduler : IInternalCommandScheduler
{
    // the last EnqueuedAt in ticks, shared by all schedulers of the process, so the order of the queue has no ties
    private static long _lastEnqueuedTicks;

    private readonly DbContext _context;
    private readonly IInternalCommandsTypeMapper _typeMapper;
    private readonly InternalCommandQueueSignal _signal;
    private readonly TimeProvider _timeProvider;

    private bool _signalsAfterSave;

    public InternalCommandScheduler(DbContext context, IInternalCommandsTypeMapper typeMapper, InternalCommandQueueSignal signal, TimeProvider timeProvider)
    {
        _context = context;
        _typeMapper = typeMapper;
        _signal = signal;
        _timeProvider = timeProvider;
    }

    public Task EnqueueAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();
        var typeName = _typeMapper.GetTypeNameForCommandWithType(commandType);
        var data = JsonSerializer.Serialize(command, commandType);

        var orderingKey = (command as IOrderedInternalCommand)?.OrderingKey;
        var concurrencyGroup = (command as IConcurrencyLimitedInternalCommand)?.ConcurrencyGroup;

        var internalCommand = new InternalCommand(Guid.NewGuid(), NextEnqueuedAt(), typeName, data, orderingKey, concurrencyGroup);
        _context.Set<InternalCommand>().Add(internalCommand);

        // a signal before the save would wake a worker that finds nothing and sleeps until its timeout
        if (!_signalsAfterSave)
        {
            _context.SavedChanges += OnSavedChanges;
            _signalsAfterSave = true;
        }

        return Task.CompletedTask;
    }

    private void OnSavedChanges(object? sender, SavedChangesEventArgs e)
    {
        _signal.Notify();
    }

    private DateTime NextEnqueuedAt()
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime.Ticks;

        while (true)
        {
            var last = Interlocked.Read(ref _lastEnqueuedTicks);
            var next = Math.Max(now, last + 1);

            if (Interlocked.CompareExchange(ref _lastEnqueuedTicks, next, last) == last)
                return new DateTime(next, DateTimeKind.Utc);
        }
    }
}
