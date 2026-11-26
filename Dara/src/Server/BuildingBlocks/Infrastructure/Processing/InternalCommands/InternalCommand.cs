using Dara.Server.BuildingBlocks.Infrastructure.Messaging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

// a command of the module queued by IInternalCommandScheduler and executed by InternalCommandsBackgroundWorker
public class InternalCommand : RetryableMessage
{
    public Guid Id { get; private set; }

    // class name of the command from IInternalCommandsTypeMapper
    public string Type { get; private set; }

    // the command serialized with System.Text.Json
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Data { get; private set; }

    // order of the queue; unique and increasing in a process
    public DateTime EnqueuedAt { get; private set; }

    // commands with the same key run one after another (IOrderedInternalCommand), null = no order
    public string? OrderingKey { get; private set; }

    // commands of one group share its limit (IConcurrencyLimitedInternalCommand), null = no group
    public string? ConcurrencyGroup { get; private set; }

    public InternalCommand(Guid id, DateTime enqueuedAt, string type, string data, string? orderingKey, string? concurrencyGroup)
        : base(enqueuedAt)
    {
        Id = id;
        EnqueuedAt = enqueuedAt;
        Type = type;
        Data = data;
        OrderingKey = orderingKey;
        ConcurrencyGroup = concurrencyGroup;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private InternalCommand() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}
