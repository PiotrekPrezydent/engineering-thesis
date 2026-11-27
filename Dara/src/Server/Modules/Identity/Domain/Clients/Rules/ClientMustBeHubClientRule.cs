using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

// names, types, context, login codes and revocation are for clients of the hub; the panel client of an account has
// none of them and lives as long as the account
public class ClientMustBeHubClientRule : IBuisnessRule
{
    private readonly ClientKind _kind;

    public ClientMustBeHubClientRule(ClientKind kind)
    {
        _kind = kind;
    }

    public string Message => "The panel client of the account takes no change of a hub client";

    public bool IsBroken()
    {
        return _kind != ClientKind.Hub;
    }
}
