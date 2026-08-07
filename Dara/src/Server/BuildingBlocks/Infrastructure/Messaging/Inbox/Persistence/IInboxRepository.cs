namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;

public interface IInboxRepository
{
    Task<IReadOnlyList<InboxMessage>> GetPendingMessagesAsync(CancellationToken ct);
    
    Task MarkAsCompletedAsync(Guid messageId, CancellationToken ct);
    
    Task SaveAsync(InboxMessage message, CancellationToken ct);
}