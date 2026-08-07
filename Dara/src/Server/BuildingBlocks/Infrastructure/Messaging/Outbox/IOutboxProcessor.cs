namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;

public interface IOutboxProcessor
{
    public Task ProcessOutboxAsync(CancellationToken cancellationToken);
}