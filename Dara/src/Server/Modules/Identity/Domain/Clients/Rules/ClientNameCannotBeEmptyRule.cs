using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

public class ClientNameCannotBeEmptyRule : IDomainRule
{
    public string? Name { get; }
    internal ClientNameCannotBeEmptyRule(string name)
    {
        Name = name;
    }

    public string Message => $"{nameof(ClientNameCannotBeEmptyRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return string.IsNullOrWhiteSpace(Name);
    }

}