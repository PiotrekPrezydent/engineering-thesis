using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Groups.Rules;

public class PromptMustBeProvidedRule : IBuisnessRule
{
    private readonly string? _prompt;

    public PromptMustBeProvidedRule(string? prompt)
    {
        _prompt = prompt;
    }

    public string Message => "Prompt must be provided";
    public bool IsBroken()
    {
        return string.IsNullOrWhiteSpace(_prompt);
    }
}
