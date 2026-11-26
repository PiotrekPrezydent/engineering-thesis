namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;

public interface IOutboxMessageProcessor
{
    public Task ProcessSingleMessageAsync(Guid messageId, CancellationToken cancellationToken);

    // call in a new scope after ProcessSingleMessageAsync threw, so nothing tracked by the failed attempt is saved
    Task RecordFailureAsync(Guid messageId, Exception error, CancellationToken ct);
}
