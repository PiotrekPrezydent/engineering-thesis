using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns;

// per-run tool names like BobsPhone_SaveFile, unique ignoring letter case (BobsPhone2_SaveFile)
public static class ToolAliasBuilder
{
    // the common part of the gemini, openai and anthropic limits, without . : and - known to some of them only
    public const int MaxLength = 64;
    private static readonly Regex AllowedAlias = new("^[A-Za-z][A-Za-z0-9_]{0,63}$");

    public static IReadOnlyList<ToolAlias> Build(IReadOnlyList<ParticipantSnapshot> participants)
    {
        var aliases = new List<ToolAlias>();
        var prefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var participant in participants)
        {
            // one number for all functions of the participant, so the model sees which tools are on one client
            var name = ParticipantPart(participant.Name);
            var number = 1;
            while (!prefixes.Add(name + NumberText(number)))
                number++;

            // the same function name from two plugins of one participant gets a number after the function part
            var functionParts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var functions = participant.Functions
                .OrderBy(f => f.Description.Name, StringComparer.Ordinal)
                .ThenBy(f => f.PluginId);

            foreach (var function in functions)
            {
                var functionPart = Unique(FunctionPart(function.Description.Name), functionParts);

                // a cut name can repeat another one, then a counter goes at the end
                var alias = Fit(name, number, functionPart, 1);
                for (var counter = 2; !taken.Add(alias); counter++)
                    alias = Fit(name, number, functionPart, counter);

                if (!AllowedAlias.IsMatch(alias))
                    throw new InvalidOperationException($"tool alias {alias} does not fit the provider limits");

                aliases.Add(new ToolAlias(alias, new ParticipantId(participant.ParticipantId), new FunctionId(function.FunctionId)));
            }
        }

        return aliases;
    }

    // "Bob's phone" -> "BobsPhone", empty or starting with a digit gets the prefix P
    private static string ParticipantPart(string name)
    {
        var part = JoinWords(name, keepUnderscore: false, capitalizeFirst: true);
        return part.Length == 0 || char.IsDigit(part[0]) ? "P" + part : part;
    }

    // the same, but _ stays and the first letter keeps its case, so a valid function name does not change
    private static string FunctionPart(string name)
    {
        var part = JoinWords(name, keepUnderscore: true, capitalizeFirst: false);
        return part.Length == 0 ? "F" : part;
    }

    private static string JoinWords(string text, bool keepUnderscore, bool capitalizeFirst)
    {
        var builder = new StringBuilder();
        var startOfWord = capitalizeFirst;

        foreach (var c in WithoutDiacritics(text))
        {
            if (char.IsAsciiLetterOrDigit(c) || (keepUnderscore && c == '_'))
            {
                builder.Append(startOfWord ? char.ToUpperInvariant(c) : c);
                startOfWord = false;
            }
            else if (char.IsWhiteSpace(c) || ((char.IsPunctuation(c) || char.IsSymbol(c)) && c is not '\'' and not '’'))
            {
                startOfWord = capitalizeFirst || builder.Length > 0;
            }

            // apostrophes and letters outside ascii are dropped inside the word: "Bob's" -> "Bobs"
        }

        return builder.ToString();
    }

    // "Łukasz Żółć" -> "Lukasz Zolc"; ł does not decompose, so it is replaced by hand
    private static string WithoutDiacritics(string text)
    {
        var decomposed = text.Replace('ł', 'l').Replace('Ł', 'L').Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                builder.Append(c);
        }

        return builder.ToString();
    }

    private static string Unique(string functionPart, HashSet<string> used)
    {
        var candidate = functionPart;
        for (var number = 2; !used.Add(candidate); number++)
            candidate = functionPart + number;

        return candidate;
    }

    // cuts the participant part first (to one letter), then the function part, never the numbers
    private static string Fit(string name, int number, string functionPart, int counter)
    {
        var numberText = NumberText(number);
        var counterText = NumberText(counter);

        var room = MaxLength - 1 - numberText.Length - counterText.Length;
        var functionLength = Math.Min(functionPart.Length, room - 1);
        var nameLength = Math.Min(name.Length, room - functionLength);

        return name[..nameLength] + numberText + "_" + functionPart[..functionLength] + counterText;
    }

    private static string NumberText(int number)
    {
        return number > 1 ? number.ToString(CultureInfo.InvariantCulture) : "";
    }
}
