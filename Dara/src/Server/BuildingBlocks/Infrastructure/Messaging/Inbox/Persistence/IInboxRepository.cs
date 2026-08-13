namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;

public interface IInboxRepository
{
    Task<IReadOnlyList<Guid>> GetPendingMessagesAsync(int batchSize, CancellationToken ct);
    
    public Task AddAsync(InboxMessage message, CancellationToken ct);
}