using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

// a change of a client comes only from its own account; an id of a client of another account is refused
public class ClientMustBelongToUserRule : IBuisnessRule
{
    private readonly bool _belongs;

    public ClientMustBelongToUserRule(bool belongs)
    {
        _belongs = belongs;
    }

    public string Message => "The client does not belong to this account";

    public bool IsBroken()
    {
        return !_belongs;
    }
}