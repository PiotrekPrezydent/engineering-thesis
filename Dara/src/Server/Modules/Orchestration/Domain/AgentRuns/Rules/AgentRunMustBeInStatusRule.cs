using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;

// every step of a run is allowed in one status only, so a repeated or late command cannot move the run back
public class AgentRunMustBeInStatusRule : IBuisnessRule
{
    private readonly AgentRunStatus _current;
    private readonly AgentRunStatus _expected;
    private readonly string _action;

    public AgentRunMustBeInStatusRule(AgentRunStatus current, AgentRunStatus expected, string action)
    {
        _current = current;
        _expected = expected;
        _action = action;
    }

    public string Message => $"Cannot {_action}: the agent run is {_current.Value}, it must be {_expected.Value}";
    public bool IsBroken()
    {
        return _current != _expected;
    }
}
