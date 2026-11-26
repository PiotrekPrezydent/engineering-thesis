namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;

public interface IInboxRepository
{
    // unprocessed messages ready for an attempt now, the oldest first
    Task<IReadOnlyList<Guid>> GetPendingMessagesAsync(int batchSize, DateTime now, CancellationToken ct);

    // the earliest NextAttemptAt of an unprocessed message, null when none waits
    Task<DateTime?> GetNextAttemptAtAsync(CancellationToken ct);
    
    public Task AddAsync(InboxMessage message, CancellationToken ct);

    Task<bool> ExistsAsync(Guid messageId, CancellationToken ct);
}