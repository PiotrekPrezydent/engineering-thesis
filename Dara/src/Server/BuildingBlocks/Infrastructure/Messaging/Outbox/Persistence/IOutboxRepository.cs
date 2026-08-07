namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;

public interface IOutboxRepository
{
    Task<IReadOnlyList<OutboxMessage>> GetPendingMessagesAsync(CancellationToken ct);
    
    Task MarkAsCompletedAsync(Guid messageId, CancellationToken ct);
    
    Task AddAndSaveAsync(OutboxMessage message, CancellationToken ct);
    
    Task AddAsync(OutboxMessage message, CancellationToken ct);
}