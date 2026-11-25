using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;

// messages of the proposed call rules go back to the model, so they say what to do instead
public class ProposedCallMustUseKnownToolRule : IBuisnessRule
{
    private readonly IReadOnlyList<ToolAlias> _tools;
    private readonly string? _alias;

    public ProposedCallMustUseKnownToolRule(IReadOnlyList<ToolAlias> tools, string? alias)
    {
        _tools = tools;
        _alias = alias;
    }

    public string Message => $"Unknown tool {_alias}, use only the tools you were given";
    public bool IsBroken()
    {
        return !_tools.Any(t => string.Equals(t.Alias, _alias, StringComparison.OrdinalIgnoreCase));
    }
}
