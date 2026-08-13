namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;

public interface IOutboxMessageProcessor
{
    public Task ProcessSingleMessageAsync(Guid messageId, CancellationToken cancellationToken);
}