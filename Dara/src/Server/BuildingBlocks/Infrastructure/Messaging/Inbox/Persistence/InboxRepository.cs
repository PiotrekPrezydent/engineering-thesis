using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;

public class InboxRepository<TDbcontext> : IInboxRepository where TDbcontext : DbContext, IInboxContext
{
    private readonly TDbcontext _dbContext;

    public InboxRepository(TDbcontext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<InboxMessage>> GetPendingMessagesAsync(CancellationToken ct)
    {
        return await _dbContext.Set<InboxMessage>()
            .Where(m => m.ProcessedDate == null)
            .OrderBy(m => m.OccurredOn)
            .ToListAsync(ct);
    }

    public async Task MarkAsCompletedAsync(Guid messageId, CancellationToken ct)
    {
        var message = await _dbContext.Set<InboxMessage>().FindAsync(new object[] { messageId }, ct);
        if (message != null)
        {
            message.ProcessedDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task SaveAsync(InboxMessage message, CancellationToken ct)
    {
        await _dbContext.Set<InboxMessage>().AddAsync(message, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}