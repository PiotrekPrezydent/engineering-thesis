using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;

// call N can use results of calls 1..N-1 only, so the plan never waits for itself
public class ResultReferenceMustPointToEarlierCallRule : IBuisnessRule
{
    private readonly int _callIndex;
    private readonly IReadOnlyList<CallArgument> _arguments;

    public ResultReferenceMustPointToEarlierCallRule(int callIndex, IReadOnlyList<CallArgument> arguments)
    {
        _callIndex = callIndex;
        _arguments = arguments;
    }

    public string Message => _callIndex == 1
        ? $"Call 1 is the first call, it cannot use {{{{result:N}}}} of another call ({Wrong()})"
        : $"Call {_callIndex} can use only {{{{result:1}}}} to {{{{result:{_callIndex - 1}}}}} ({Wrong()})";

    public bool IsBroken()
    {
        return Wrong() is not null;
    }

    private string? Wrong()
    {
        var wrong = _arguments.FirstOrDefault(a => a.ResultIndexes.Any(i => i < 1 || i >= _callIndex));
        if (wrong is null)
            return null;

        var index = wrong.ResultIndexes.First(i => i < 1 || i >= _callIndex);
        return $"{wrong.Name} uses {{{{result:{index}}}}}";
    }
}
