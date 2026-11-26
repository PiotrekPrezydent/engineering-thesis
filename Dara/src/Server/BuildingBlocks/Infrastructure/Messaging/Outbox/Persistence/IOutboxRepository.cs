namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;

public interface IOutboxRepository
{
    // unprocessed messages ready for an attempt now, the oldest first
    Task<IReadOnlyList<Guid>> GetPendingMessagesAsync(int batchSize, DateTime now, CancellationToken ct);

    // the earliest NextAttemptAt of an unprocessed message, null when none waits
    Task<DateTime?> GetNextAttemptAtAsync(CancellationToken ct);
    
    Task AddAsync(OutboxMessage message, CancellationToken ct);
}