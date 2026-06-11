using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients.Rules;

public record ClientNameCannotBeEmptyRule : IBuisnessRule
{
    private string _name;

    public ClientNameCannotBeEmptyRule(string name)
    {
        _name = name;
    }

    public string Message => nameof(ClientNameCannotBeEmptyRule);
    public bool IsBroken()
    {
        return string.IsNullOrEmpty(_name);
    }
}