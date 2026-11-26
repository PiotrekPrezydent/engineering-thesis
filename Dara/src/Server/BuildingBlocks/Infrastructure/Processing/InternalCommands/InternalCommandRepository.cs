using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

public class InternalCommandRepository : IInternalCommandRepository
{
    private readonly DbContext _context;

    public InternalCommandRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PendingInternalCommand>> GetPendingCommandsAsync(int maxCount, CancellationToken ct)
    {
        return await _context.Set<InternalCommand>()
            .AsNoTracking()
            .Where(c => c.ProcessedDate == null)
            .OrderBy(c => c.EnqueuedAt)
            .Take(maxCount)
            .Select(c => new PendingInternalCommand(c.Id, c.NextAttemptAt, c.OrderingKey, c.ConcurrencyGroup))
            .ToListAsync(ct);
    }

    public async Task<InternalCommand?> GetAsync(Guid id, CancellationToken ct)
    {
        return await _context.Set<InternalCommand>().FindAsync([id], ct);
    }
}
