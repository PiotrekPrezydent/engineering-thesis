using System.Threading.Channels;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

// wakes the internal commands worker: a command was stored, or a running command finished;
// many signals before the worker wakes count as one
public class InternalCommandQueueSignal
{
    private readonly Channel<bool> _channel = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
    {
        FullMode = BoundedChannelFullMode.DropWrite,
    });

    public void Notify() => _channel.Writer.TryWrite(true);

    public async Task WaitAsync(CancellationToken ct) => await _channel.Reader.ReadAsync(ct);
}
