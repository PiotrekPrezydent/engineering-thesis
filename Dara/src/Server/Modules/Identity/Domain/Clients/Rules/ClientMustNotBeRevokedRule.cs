using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

// a revoked client takes no change, gets no code and cannot sign in again
public class ClientMustNotBeRevokedRule : IBuisnessRule
{
    private readonly bool _isRevoked;

    public ClientMustNotBeRevokedRule(bool isRevoked)
    {
        _isRevoked = isRevoked;
    }

    public string Message => "The client is revoked";

    public bool IsBroken()
    {
        return _isRevoked;
    }
}