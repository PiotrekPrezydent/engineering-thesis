namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

public interface IInternalCommandRepository
{
    // unprocessed commands, the oldest first; also those waiting for a retry, because they hold back
    // later commands with the same ordering key
    Task<IReadOnlyList<PendingInternalCommand>> GetPendingCommandsAsync(int maxCount, CancellationToken ct);

    Task<InternalCommand?> GetAsync(Guid id, CancellationToken ct);
}

public sealed record PendingInternalCommand(Guid Id, DateTime NextAttemptAt, string? OrderingKey, string? ConcurrencyGroup);
