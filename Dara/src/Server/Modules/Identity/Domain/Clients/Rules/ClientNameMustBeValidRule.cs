using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

// 1 to 50 characters after trim, no control characters; names need not be unique (orchestration numbers repeated
// tool prefixes)
public class ClientNameMustBeValidRule : IBuisnessRule
{
    public const int MaxLength = 50;

    private readonly string? _name;

    public ClientNameMustBeValidRule(string? name)
    {
        _name = name;
    }

    public string Message => nameof(ClientNameMustBeValidRule);

    public bool IsBroken()
    {
        var name = _name?.Trim();
        return string.IsNullOrEmpty(name) || name.Length > MaxLength || name.Any(char.IsControl);
    }
}
