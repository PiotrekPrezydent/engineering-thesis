using System.Text.Json;
using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;

public class ProposedArgumentsMustBeJsonObjectRule : IBuisnessRule
{
    private readonly string? _argumentsJson;

    public ProposedArgumentsMustBeJsonObjectRule(string? argumentsJson)
    {
        _argumentsJson = argumentsJson;
    }

    public string Message => "Arguments must be one json object with an entry for each argument";
    public bool IsBroken()
    {
        if (_argumentsJson is null)
            return true;

        try
        {
            using var document = JsonDocument.Parse(_argumentsJson);
            return document.RootElement.ValueKind != JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return true;
        }
    }
}
