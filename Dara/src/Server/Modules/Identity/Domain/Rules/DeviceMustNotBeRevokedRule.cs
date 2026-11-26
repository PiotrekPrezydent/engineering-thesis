using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Rules;

// a revoked device gets no login code; the handler refuses it before, so this guards the invariant
public class DeviceMustNotBeRevokedRule : IBuisnessRule
{
    private readonly bool _isRevoked;

    public DeviceMustNotBeRevokedRule(bool isRevoked)
    {
        _isRevoked = isRevoked;
    }

    public string Message => "A revoked device gets no login code";

    public bool IsBroken()
    {
        return _isRevoked;
    }
}
