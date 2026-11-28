using Dara.Shared.Contracts.FunctionCalls;

namespace Dara.Server.Apps.API.Common.FunctionCalls;

// the three outcomes of the contract and NotDelivered: the client was offline, the call never left the server
public enum FunctionCallResultOutcome
{
    Succeeded,
    Failed,
    NotExecuted,
    NotDelivered
}

public record FunctionCallResult(Guid CallId, Guid ClientId, FunctionCallResultOutcome Outcome, string? ResultJson, string? Error, DateTime ReceivedAt)
{
    public static FunctionCallResultOutcome From(FunctionCallOutcome outcome) => outcome switch
    {
        FunctionCallOutcome.Succeeded => FunctionCallResultOutcome.Succeeded,
        FunctionCallOutcome.Failed => FunctionCallResultOutcome.Failed,
        _ => FunctionCallResultOutcome.NotExecuted
    };
}

// where the result of a call goes: orchestration in TASK-ENG-013 (a command of the module that records the step);
// until then LoggingFunctionCallResultPort
public interface IFunctionCallResultPort
{
    Task AcceptAsync(FunctionCallResult result, CancellationToken cancellationToken);
}

public class LoggingFunctionCallResultPort : IFunctionCallResultPort
{
    private readonly ILogger<LoggingFunctionCallResultPort> _logger;

    public LoggingFunctionCallResultPort(ILogger<LoggingFunctionCallResultPort> logger)
    {
        _logger = logger;
    }

    public Task AcceptAsync(FunctionCallResult result, CancellationToken cancellationToken)
    {
        _logger.LogInformation("call {CallId} on client {ClientId}: {Outcome} {Error}", result.CallId, result.ClientId, result.Outcome, result.Error);
        return Task.CompletedTask;
    }
}
