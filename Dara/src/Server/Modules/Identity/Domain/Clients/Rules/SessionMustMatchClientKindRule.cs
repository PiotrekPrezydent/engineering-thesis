using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

// refresh tokens are for a session of a client of the hub, sign-ins of browsers for the session of the panel client
public class SessionMustMatchClientKindRule : IBuisnessRule
{
    private readonly ClientKind _kind;
    private readonly ClientKind _expected;

    public SessionMustMatchClientKindRule(ClientKind kind, ClientKind expected)
    {
        _kind = kind;
        _expected = expected;
    }

    public string Message => $"The session of a {_kind} client takes no change of a {_expected} session";

    public bool IsBroken()
    {
        return _kind != _expected;
    }
}
