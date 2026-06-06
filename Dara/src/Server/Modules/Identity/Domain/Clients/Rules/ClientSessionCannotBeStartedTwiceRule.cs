using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

public class ClientSessionCannotBeStartedTwiceRule : IDomainRule
{
    private bool _hasId;
    internal ClientSessionCannotBeStartedTwiceRule(bool hasId)
    {
        _hasId = hasId;
    }

    public string Message => $"{nameof(ClientSessionCannotBeStartedTwiceRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return _hasId;
    }

}