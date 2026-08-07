namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;

public interface IOutboxProcessor
{
    public Task ProcessOutboxAsync(CancellationToken cancellationToken);
}