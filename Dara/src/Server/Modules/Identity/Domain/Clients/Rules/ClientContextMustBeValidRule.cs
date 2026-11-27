using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

// up to 20 entries, a key of 1 to 50 characters after trim and unique without regard to case, a value up to 500
// characters, no control characters
public class ClientContextMustBeValidRule : IBuisnessRule
{
    public const int MaxEntries = 20;
    public const int MaxKeyLength = 50;
    public const int MaxValueLength = 500;

    private readonly IReadOnlyDictionary<string, string>? _entries;

    public ClientContextMustBeValidRule(IReadOnlyDictionary<string, string>? entries)
    {
        _entries = entries;
    }

    public string Message => nameof(ClientContextMustBeValidRule);

    public bool IsBroken()
    {
        if (_entries is null || _entries.Count > MaxEntries)
            return true;

        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (rawKey, rawValue) in _entries)
        {
            var key = rawKey?.Trim();
            var value = rawValue?.Trim();
            if (string.IsNullOrEmpty(key) || key.Length > MaxKeyLength || key.Any(char.IsControl) || !keys.Add(key))
                return true;
            if (value is null || value.Length > MaxValueLength || value.Any(char.IsControl))
                return true;
        }

        return false;
    }
}
