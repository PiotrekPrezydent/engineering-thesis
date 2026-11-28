using System.Collections.Concurrent;
using Dara.Server.Apps.API.Common.FunctionCalls;

namespace Dara.Server.SystemTests.ClientServer;

// the results of function calls the host passed on, in order
public class RecordingFunctionCallResultPort : IFunctionCallResultPort
{
    private readonly ConcurrentQueue<FunctionCallResult> _results = new();

    public IReadOnlyList<FunctionCallResult> Results => _results.ToList();

    public Task AcceptAsync(FunctionCallResult result, CancellationToken cancellationToken)
    {
        _results.Enqueue(result);
        return Task.CompletedTask;
    }
}
