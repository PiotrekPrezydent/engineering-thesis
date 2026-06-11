using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.Server.Modules.Clients.Domain.Clients.Rules;

public class ClientSessionCannotBeStartedTwiceRule : IBuisnessRule
{
    private bool _isActive;
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