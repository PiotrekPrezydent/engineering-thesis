using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Users.Rules;

// the roles of a client change only through its own account; an id of a client of another account is refused
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
