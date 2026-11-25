using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Groups.Rules;

// without a prefix every group message would start an agent run
public class PromptMessagePrefixMustBeProvidedRule : IBuisnessRule
{
    private readonly string? _prefix;

    public PromptMessagePrefixMustBeProvidedRule(string? prefix)
    {
        _prefix = prefix;
    }

    public string Message => "Prompt message prefix must be provided and cannot contain white space";
    public bool IsBroken()
    {
        return string.IsNullOrEmpty(_prefix) || _prefix.Any(char.IsWhiteSpace);
    }
}
