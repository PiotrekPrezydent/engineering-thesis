namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;

public interface IOutboxRepository
{
    Task<IReadOnlyList<Guid>> GetPendingMessagesAsync(int batchSize, CancellationToken ct);
    
    Task AddAsync(OutboxMessage message, CancellationToken ct);
}