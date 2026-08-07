using System.Threading.Channels;

namespace Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Processing;

public class OutboxQueueSignal
{
    private readonly Channel<bool> _channel = Channel.CreateUnbounded<bool>();
    
    public void NotifyNewMessage() => _channel.Writer.TryWrite(true);

    public async Task WaitAsync(CancellationToken ct) => await _channel.Reader.ReadAsync(ct);
}