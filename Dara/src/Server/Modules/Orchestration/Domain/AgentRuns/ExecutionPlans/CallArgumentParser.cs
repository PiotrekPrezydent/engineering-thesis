using System.Text.Json;
using System.Text.RegularExpressions;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;

// turns the json arguments of a proposed call into call arguments, finding {{result:N}} references
public static class CallArgumentParser
{
    private static readonly Regex WholeReference = new(@"^\s*\{\{result:(\d+)\}\}\s*$");
    private static readonly Regex Reference = new(@"\{\{result:(\d+)\}\}");

    // the arguments must be a json object, see ProposedArgumentsMustBeJsonObjectRule
    public static IReadOnlyList<CallArgument> Parse(string argumentsJson)
    {
        using var document = JsonDocument.Parse(argumentsJson);
        return document.RootElement.EnumerateObject().Select(p => ToArgument(p.Name, p.Value)).ToList();
    }

    // a reference somewhere in the json text, for a value argument it sits inside an object or an array
    public static bool ContainsReference(string json)
    {
        return Reference.IsMatch(json);
    }

    private static CallArgument ToArgument(string name, JsonElement value)
    {
        var json = value.GetRawText();
        if (value.ValueKind != JsonValueKind.String)
            return new CallArgument(name, CallArgumentKind.Value, json, []);

        var text = value.GetString()!;
        var whole = WholeReference.Match(text);
        if (whole.Success)
            return new CallArgument(name, CallArgumentKind.Result, json, [ToIndex(whole)]);

        var indexes = Reference.Matches(text).Select(ToIndex).Distinct().ToList();
        return new CallArgument(name, indexes.Count > 0 ? CallArgumentKind.Text : CallArgumentKind.Value, json, indexes);
    }

    // a number too big for int points to no call, so the reference rule rejects it
    private static int ToIndex(Match match)
    {
        return int.TryParse(match.Groups[1].Value, out var index) ? index : int.MaxValue;
    }
}
