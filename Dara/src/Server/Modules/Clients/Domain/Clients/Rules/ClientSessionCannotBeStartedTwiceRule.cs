using Dara.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients.Rules;

public record ClientSessionCannotBeStartedTwiceRule : IBuisnessRule
{
    private readonly bool _isActive;
    internal ClientSessionCannotBeStartedTwiceRule(bool isActive)
    {
        _isActive = isActive;
    }

    public string Message => $"{nameof(ClientSessionCannotBeStartedTwiceRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return _isActive;
    }

}