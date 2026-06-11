using Dara.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients.Rules;

public record ClientNameCannotBeEmptyRule : IBuisnessRule
{
    private readonly string? _name;
    internal ClientNameCannotBeEmptyRule(string name)
    {
        _name = name;
    }

    public string Message => $"{nameof(ClientNameCannotBeEmptyRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return string.IsNullOrWhiteSpace(_name);
    }

}